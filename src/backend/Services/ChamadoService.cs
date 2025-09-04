using CajuAjuda.Backend.Helpers;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Http;

namespace CajuAjuda.Backend.Services;

public class ChamadoService : IChamadoService
{
    private readonly IChamadoRepository _chamadoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAnexoRepository _anexoRepository;
    private readonly IMensagemRepository _mensagemRepository;

    public ChamadoService(
        IChamadoRepository chamadoRepository, 
        IUsuarioRepository usuarioRepository, 
        IFileStorageService fileStorageService, 
        IAnexoRepository anexoRepository,
        IMensagemRepository mensagemRepository)
    {
        _chamadoRepository = chamadoRepository;
        _usuarioRepository = usuarioRepository;
        _fileStorageService = fileStorageService;
        _anexoRepository = anexoRepository;
        _mensagemRepository = mensagemRepository;
    }

    public async Task<Chamado> CreateAsync(ChamadoCreateDto chamadoDto, string clienteEmail)
    {
        var cliente = await _usuarioRepository.GetByEmailAsync(clienteEmail);
        if (cliente == null || cliente.Role != Role.CLIENTE)
        {
            throw new Exception("Cliente não encontrado ou usuário não autorizado para criar chamados.");
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
        return novoChamado;
    }

    public async Task<IEnumerable<ChamadoListResponseDto>> GetChamadosByClienteEmailAsync(string clienteEmail)
    {
        var cliente = await _usuarioRepository.GetByEmailAsync(clienteEmail);
        if (cliente == null)
        {
            throw new Exception("Cliente não encontrado.");
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
            return null;
        }

        if (userRole != "TECNICO" && userRole != "ADMIN" && chamado.Cliente.Email != userEmail)
        {
            throw new UnauthorizedAccessException("Você não tem permissão para visualizar este chamado.");
        }

        if (userRole == "CLIENTE")
        {
            await _mensagemRepository.MarkMessagesAsReadByClienteAsync(chamadoId);
        }

        return new ChamadoDetailResponseDto
        {
            Id = chamado.Id,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            NomeCliente = chamado.Cliente.Nome,
            Status = chamado.Status,
            Prioridade = chamado.Prioridade,
            DataCriacao = chamado.DataCriacao,
            DataFechamento = chamado.DataFechamento,
            Mensagens = chamado.Mensagens.Select(m => new MensagemResponseDto
            {
                Id = m.Id,
                Texto = m.Texto,
                DataEnvio = m.DataEnvio,
                AutorNome = m.Autor.Nome,
                AutorId = m.Autor.Id
            }).OrderBy(m => m.DataEnvio).ToList()
        };
    }

    public async Task UpdateChamadoStatusAsync(long chamadoId, StatusChamado novoStatus)
    {
        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            throw new KeyNotFoundException("Chamado não encontrado.");
        }
        if (chamado.Status == StatusChamado.FECHADO || chamado.Status == StatusChamado.CANCELADO)
        {
            throw new Exception("Não é possível alterar o status de um chamado que já foi fechado ou cancelado.");
        }
        chamado.Status = novoStatus;
        if (novoStatus == StatusChamado.FECHADO)
        {
            chamado.DataFechamento = DateTime.UtcNow;
        }
        await _chamadoRepository.UpdateAsync(chamado);
    }
    
    public async Task<Anexo> AddAnexoAsync(long chamadoId, IFormFile file, string userEmail)
    {
        var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
        if (chamado == null)
        {
            throw new KeyNotFoundException("Chamado não encontrado.");
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
}