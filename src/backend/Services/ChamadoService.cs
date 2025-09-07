using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Exceptions;
using CajuAjuda.Backend.Helpers;
using CajuAjuda.Backend.Hubs;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CajuAjuda.Backend.Services;

public class ChamadoService : IChamadoService
{
    private readonly CajuAjudaDbContext _context;
    private readonly IChamadoRepository _chamadoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAnexoRepository _anexoRepository;
    private readonly IMensagemRepository _mensagemRepository;
    private readonly IHubContext<NotificacaoHub> _hubContext;
    private readonly ILogger<ChamadoService> _logger;

    public ChamadoService(
        CajuAjudaDbContext context,
        IChamadoRepository chamadoRepository, 
        IUsuarioRepository usuarioRepository, 
        IFileStorageService fileStorageService, 
        IAnexoRepository anexoRepository,
        IMensagemRepository mensagemRepository,
        IHubContext<NotificacaoHub> hubContext,
        ILogger<ChamadoService> logger)
    {
        _context = context;
        _chamadoRepository = chamadoRepository;
        _usuarioRepository = usuarioRepository;
        _fileStorageService = fileStorageService;
        _anexoRepository = anexoRepository;
        _mensagemRepository = mensagemRepository;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<Chamado> CreateAsync(ChamadoCreateDto chamadoDto, string clienteEmail)
    {
        _logger.LogInformation("Tentativa de criação de chamado pelo cliente: {ClienteEmail}", clienteEmail);
        
        var cliente = await _usuarioRepository.GetByEmailAsync(clienteEmail);
        if (cliente == null || cliente.Role != Role.CLIENTE)
        {
            throw new BusinessRuleException("Cliente não encontrado ou utilizador não autorizado para criar chamados.");
        }
        var novoChamado = new Chamado
        {
            Titulo = chamadoDto.Titulo,
            Descricao = chamadoDto.Descricao,
            Prioridade = chamadoDto.Prioridade,
            Status = StatusChamado.ABERTO,
            DataCriacao = DateTime.UtcNow,
            ClienteId = cliente.Id
        };
        await _chamadoRepository.AddAsync(novoChamado);
        
        _logger.LogInformation("Chamado {ChamadoId} criado com sucesso para o cliente {ClienteId}", novoChamado.Id, cliente.Id);
        
        await _hubContext.Clients.All.SendAsync("NovoChamadoRecebido", new 
        { 
            id = novoChamado.Id, 
            titulo = novoChamado.Titulo,
            clienteNome = cliente.Nome
        });
        return novoChamado;
    }

    public async Task<IEnumerable<ChamadoListResponseDto>> GetChamadosByClienteEmailAsync(string clienteEmail)
    {
        var cliente = await _usuarioRepository.GetByEmailAsync(clienteEmail);
        if (cliente == null)
        {
            throw new NotFoundException("Cliente não encontrado.");
        }
        var chamados = await _chamadoRepository.GetByClienteIdAsync(cliente.Id);
        return chamados.Select(c => new ChamadoListResponseDto
        {
            Id = c.Id,
            Titulo = c.Titulo,
            Status = c.Status,
            Prioridade = c.Prioridade,
            DataCriacao = c.DataCriacao,
            HasUnreadMessages = c.Mensagens.Any(m => !m.LidoPeloCliente && m.Autor.Role != Role.CLIENTE)
        });
    }

    public async Task<PagedList<ChamadoResponseDto>> GetAllChamadosAsync(int pageNumber, int pageSize, StatusChamado? status, PrioridadeChamado? prioridade)
    {
        var (chamados, totalCount) = await _chamadoRepository.GetAllAsync(pageNumber, pageSize, status, prioridade);
        var chamadosDto = chamados.Select(c => new ChamadoResponseDto
        {
            Id = c.Id,
            Titulo = c.Titulo,
            NomeCliente = c.Cliente.Nome,
            NomeTecnicoResponsavel = c.TecnicoResponsavel?.Nome,
            Status = c.Status,
            Prioridade = c.Prioridade,
            DataCriacao = c.DataCriacao
        }).ToList();
        return new PagedList<ChamadoResponseDto>(chamadosDto, totalCount, pageNumber, pageSize);
    }

    public async Task<ChamadoDetailResponseDto?> GetChamadoByIdAsync(long chamadoId, string userEmail, string userRole)
    {
        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            throw new NotFoundException($"Chamado com ID {chamadoId} não encontrado.");
        }

        if (userRole != "TECNICO" && userRole != "ADMIN" && chamado.Cliente.Email != userEmail)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para visualizar este chamado.");
        }

        if (userRole == "CLIENTE")
        {
            await _mensagemRepository.MarkMessagesAsReadByClienteAsync(chamadoId);
        }
        
        var mensagensParaMostrar = chamado.Mensagens.AsQueryable();
        
        if (userRole == "CLIENTE")
        {
            mensagensParaMostrar = mensagensParaMostrar.Where(m => !m.IsNotaInterna);
        }

        return new ChamadoDetailResponseDto
        {
            Id = chamado.Id,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            NomeCliente = chamado.Cliente.Nome,
            NomeTecnicoResponsavel = chamado.TecnicoResponsavel?.Nome,
            Status = chamado.Status,
            Prioridade = chamado.Prioridade,
            DataCriacao = chamado.DataCriacao,
            DataFechamento = chamado.DataFechamento,
            NotaAvaliacao = chamado.NotaAvaliacao,
            ComentarioAvaliacao = chamado.ComentarioAvaliacao,
            Mensagens = mensagensParaMostrar.Select(m => new MensagemResponseDto
            {
                Id = m.Id,
                Texto = m.Texto,
                DataEnvio = m.DataEnvio,
                AutorNome = m.Autor.Nome,
                AutorId = m.Autor.Id,
                IsNotaInterna = m.IsNotaInterna
            }).OrderBy(m => m.DataEnvio).ToList()
        };
    }

    public async Task UpdateChamadoStatusAsync(long chamadoId, StatusChamado novoStatus)
    {
        _logger.LogInformation("Tentando atualizar o status do chamado ID: {ChamadoId} para {NovoStatus}", chamadoId, novoStatus);
        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            _logger.LogWarning("Tentativa de atualizar status de um chamado não existente. ID: {ChamadoId}", chamadoId);
            throw new NotFoundException($"Chamado com ID {chamadoId} não encontrado.");
        }
        if (chamado.Status == StatusChamado.FECHADO || chamado.Status == StatusChamado.CANCELADO)
        {
            _logger.LogWarning("Tentativa de alterar status de um chamado já finalizado. ID: {ChamadoId}", chamadoId);
            throw new BusinessRuleException("Não é possível alterar o status de um chamado que já foi fechado ou cancelado.");
        }
        chamado.Status = novoStatus;
        if (novoStatus == StatusChamado.FECHADO)
        {
            chamado.DataFechamento = DateTime.UtcNow;
        }
        await _chamadoRepository.UpdateAsync(chamado);
        _logger.LogInformation("Status do chamado ID: {ChamadoId} atualizado com sucesso.", chamadoId);
    }
    
    public async Task<Anexo> AddAnexoAsync(long chamadoId, IFormFile file, string userEmail)
    {
        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            throw new NotFoundException($"Chamado com ID {chamadoId} não encontrado.");
        }
        if (chamado.Cliente.Email != userEmail)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para adicionar anexos a este chamado.");
        }
        var uniqueFileName = await _fileStorageService.SaveFileAsync(file);
        var anexo = new Anexo
        {
            NomeArquivo = file.FileName,
            NomeUnico = uniqueFileName,
            TipoArquivo = file.ContentType,
            ChamadoId = chamadoId
        };
        await _anexoRepository.AddAsync(anexo);
        return anexo;
    }

    public async Task AssignChamadoAsync(long chamadoId, string tecnicoEmail)
    {
        var tecnico = await _usuarioRepository.GetByEmailAsync(tecnicoEmail);
        if (tecnico == null || tecnico.Role == Role.CLIENTE)
        {
            throw new BusinessRuleException("Utilizador técnico não encontrado ou inválido.");
        }

        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            throw new NotFoundException($"Chamado com ID {chamadoId} não encontrado.");
        }

        if (chamado.Status == StatusChamado.FECHADO || chamado.Status == StatusChamado.CANCELADO)
        {
            throw new BusinessRuleException("Não é possível atribuir um chamado fechado ou cancelado.");
        }

        chamado.TecnicoResponsavelId = tecnico.Id;
        
        if (chamado.Status == StatusChamado.ABERTO)
        {
            chamado.Status = StatusChamado.EM_ANDAMENTO;
        }

        await _chamadoRepository.UpdateAsync(chamado);
    }

    public async Task AvaliarChamadoAsync(long chamadoId, AvaliacaoDto avaliacaoDto, string clienteEmail)
    {
        var cliente = await _usuarioRepository.GetByEmailAsync(clienteEmail);
        if (cliente == null)
        {
            throw new NotFoundException("Utilizador cliente não encontrado.");
        }
        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            throw new NotFoundException("Chamado não encontrado.");
        }
        if (chamado.ClienteId != cliente.Id)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para avaliar este chamado.");
        }
        if (chamado.Status != StatusChamado.FECHADO)
        {
            throw new BusinessRuleException("Só é possível avaliar chamados que já foram fechados.");
        }
        if (chamado.NotaAvaliacao.HasValue)
        {
            throw new BusinessRuleException("Este chamado já foi avaliado anteriormente.");
        }
        chamado.NotaAvaliacao = avaliacaoDto.Nota;
        chamado.ComentarioAvaliacao = avaliacaoDto.Comentario;
        await _chamadoRepository.UpdateAsync(chamado);
    }
    
    public async Task MergeChamadosAsync(long chamadoDuplicadoId, long chamadoPrincipalId, string tecnicoEmail)
    {
        if (chamadoDuplicadoId == chamadoPrincipalId)
        {
            throw new BusinessRuleException("Não é possível mesclar um chamado nele mesmo.");
        }

        var tecnico = await _usuarioRepository.GetByEmailAsync(tecnicoEmail);
        if (tecnico == null) throw new NotFoundException("Técnico não encontrado.");

        var chamadoDuplicado = await _context.Chamados.Include(c => c.Mensagens).Include(c => c.Anexos).FirstOrDefaultAsync(c => c.Id == chamadoDuplicadoId);
        var chamadoPrincipal = await _chamadoRepository.GetByIdAsync(chamadoPrincipalId);

        if (chamadoDuplicado == null || chamadoPrincipal == null)
        {
            throw new NotFoundException("Um ou ambos os chamados não foram encontrados.");
        }

        if (chamadoDuplicado.ClienteId != chamadoPrincipal.ClienteId)
        {
            throw new BusinessRuleException("Só é possível mesclar chamados do mesmo cliente.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var notaDeMerge = new Mensagem
            {
                Texto = $"Este chamado foi mesclado com o chamado #{chamadoDuplicado.Id}: '{chamadoDuplicado.Titulo}'. O conteúdo anterior foi movido para cá.",
                AutorId = tecnico.Id,
                ChamadoId = chamadoPrincipal.Id,
                IsNotaInterna = true
            };
            await _mensagemRepository.AddAsync(notaDeMerge);

            foreach (var mensagem in chamadoDuplicado.Mensagens)
            {
                mensagem.ChamadoId = chamadoPrincipal.Id;
            }
            
            foreach (var anexo in chamadoDuplicado.Anexos)
            {
                anexo.ChamadoId = chamadoPrincipal.Id;
            }
            
            chamadoDuplicado.Status = StatusChamado.MESCLADO;
            chamadoDuplicado.DataFechamento = DateTime.UtcNow;
            chamadoDuplicado.ChamadoPrincipalId = chamadoPrincipal.Id;

            await _context.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao tentar mesclar o chamado {DuplicadoId} no chamado {PrincipalId}", chamadoDuplicadoId, chamadoPrincipalId);
            await transaction.RollbackAsync();
            throw;
        }
    }
}




