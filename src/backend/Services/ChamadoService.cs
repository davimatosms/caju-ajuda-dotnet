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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services
{
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
        private readonly IAIService _aiService;

        public ChamadoService(
            CajuAjudaDbContext context,
            IChamadoRepository chamadoRepository,
            IUsuarioRepository usuarioRepository,
            IFileStorageService fileStorageService,
            IAnexoRepository anexoRepository,
            IMensagemRepository mensagemRepository,
            IHubContext<NotificacaoHub> hubContext,
            ILogger<ChamadoService> logger,
            IAIService aiService)
        {
            _context = context;
            _chamadoRepository = chamadoRepository;
            _usuarioRepository = usuarioRepository;
            _fileStorageService = fileStorageService;
            _anexoRepository = anexoRepository;
            _mensagemRepository = mensagemRepository;
            _hubContext = hubContext;
            _logger = logger;
            _aiService = aiService;
        }

        public async Task<Chamado> CreateAsync(ChamadoCreateDto chamadoDto, string clienteEmail)
        {
            var cliente = await _usuarioRepository.GetByEmailAsync(clienteEmail);
            if (cliente == null || cliente.Role != Role.CLIENTE)
            {
                throw new BusinessRuleException("Cliente não encontrado ou utilizador não autorizado para criar chamados.");
            }

            _logger.LogInformation("🎯 Criando novo chamado. Iniciando análise de IA...");

            // 1. Definir prioridade usando IA
            var prioridadeDefinidaPelaIA = await _aiService.DefinirPrioridadeAsync(chamadoDto.Titulo, chamadoDto.Descricao);
            _logger.LogInformation("✅ Prioridade definida: {Prioridade}", prioridadeDefinidaPelaIA);

            // 2. Gerar sugestão de solução usando IA
            _logger.LogInformation("🤖 Gerando sugestão de solução com IA...");
            var sugestaoIA = await _aiService.SugerirSolucaoAsync(chamadoDto.Titulo, chamadoDto.Descricao);
            _logger.LogInformation("✅ Sugestão de solução gerada ({Length} caracteres)", sugestaoIA.Length);

            var novoChamado = new Chamado
            {
                Titulo = chamadoDto.Titulo,
                Descricao = chamadoDto.Descricao,
                Prioridade = prioridadeDefinidaPelaIA,
                Status = StatusChamado.ABERTO,
                DataCriacao = DateTime.UtcNow,
                ClienteId = cliente.Id,
                SugestaoIA = sugestaoIA // Mantém para referência futura
            };
            await _chamadoRepository.AddAsync(novoChamado);

            // 3. Criar mensagem automática da IA com a sugestão
            _logger.LogInformation("🔍 Buscando usuário da IA (ia@cajuajuda.com)...");
            var aiUsuario = await _usuarioRepository.GetByEmailAsync("ia@cajuajuda.com");
            
            if (aiUsuario == null)
            {
                _logger.LogWarning("⚠️ ATENÇÃO: Usuário da IA não encontrado no banco de dados! A mensagem automática NÃO será criada.");
                _logger.LogWarning("💡 Solução: Recrie o banco de dados para criar o usuário 'ia@cajuajuda.com'");
            }
            else if (string.IsNullOrEmpty(sugestaoIA))
            {
                _logger.LogWarning("⚠️ Sugestão da IA está vazia. Mensagem não será criada.");
            }
            else
            {
                _logger.LogInformation("✅ Usuário da IA encontrado! Criando mensagem automática...");
                var mensagemIA = new Mensagem
                {
                    Texto = $"🤖 **Olá! Sou a Assistente IA do Caju Ajuda.**\n\nAnalisei sua solicitação e tenho algumas sugestões que podem ajudar:\n\n{sugestaoIA}\n\n💡 *Enquanto isso, um técnico irá analisar seu caso em breve. Caso precise de mais informações, fique à vontade para escrever aqui!*",
                    ChamadoId = novoChamado.Id,
                    AutorId = aiUsuario.Id,
                    DataEnvio = DateTime.UtcNow,
                    LidoPeloCliente = false,
                    IsNotaInterna = false
                };
                await _mensagemRepository.AddAsync(mensagemIA);
                _logger.LogInformation("✅ Mensagem da IA criada com sucesso no banco de dados");
            }

            _logger.LogInformation("📢 Enviando notificação de novo chamado via SignalR...");
            await _hubContext.Clients.All.SendAsync("NovoChamadoRecebido", new { 
                id = novoChamado.Id, 
                titulo = novoChamado.Titulo, 
                clienteNome = cliente.Nome,
                temSugestaoIA = !string.IsNullOrEmpty(sugestaoIA)
            });
            
            return novoChamado;
        }

        public async Task<IEnumerable<ChamadoListResponseDto>> GetChamadosByClienteEmailAsync(string clienteEmail)
        {
            var cliente = await _usuarioRepository.GetByEmailAsync(clienteEmail);
            if (cliente == null) throw new NotFoundException("Cliente não encontrado.");

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
            if (chamado == null) throw new NotFoundException($"Chamado com ID {chamadoId} não encontrado.");

            if (userRole != "TECNICO" && userRole != "ADMIN" && chamado.Cliente.Email != userEmail)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para visualizar este chamado.");
            }

            if (userRole == "CLIENTE") await _mensagemRepository.MarkMessagesAsReadByClienteAsync(chamadoId);

            var mensagensParaMostrar = (userRole == "CLIENTE")
                ? chamado.Mensagens.Where(m => !m.IsNotaInterna)
                : chamado.Mensagens;

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
                SugestaoIA = chamado.SugestaoIA,
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
            var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
            if (chamado == null) throw new NotFoundException($"Chamado com ID {chamadoId} não encontrado.");

            if (chamado.Status == StatusChamado.FECHADO || chamado.Status == StatusChamado.CANCELADO)
            {
                throw new BusinessRuleException("Não é possível alterar o status de um chamado que já foi fechado ou cancelado.");
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
            if (chamado == null) throw new NotFoundException($"Chamado com ID {chamadoId} não encontrado.");
            if (chamado.Cliente.Email != userEmail) throw new UnauthorizedAccessException("Você não tem permissão para adicionar anexos a este chamado.");

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

        public async Task<List<Anexo>> GetAnexosByChamadoIdAsync(long chamadoId, string userEmail, string userRole)
        {
            var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
            if (chamado == null) throw new NotFoundException($"Chamado com ID {chamadoId} não encontrado.");

            // Verifica permissão: Admin/Técnico pode ver todos, Cliente só pode ver seus próprios
            bool isAdminOrTecnico = userRole == "ADMIN" || userRole == "TECNICO";
            bool isClienteDoChamado = chamado.Cliente.Email == userEmail;

            if (!isAdminOrTecnico && !isClienteDoChamado)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para visualizar os anexos deste chamado.");
            }

            return await _anexoRepository.GetByChamadoIdAsync(chamadoId);
        }

        public async Task<Anexo?> GetAnexoByIdAsync(long anexoId, string userEmail, string userRole)
        {
            var anexo = await _anexoRepository.GetByIdAsync(anexoId);
            if (anexo == null) throw new NotFoundException($"Anexo com ID {anexoId} não encontrado.");

            // Verifica permissão
            bool isAdminOrTecnico = userRole == "ADMIN" || userRole == "TECNICO";
            bool isClienteDoChamado = anexo.Chamado.Cliente.Email == userEmail;

            if (!isAdminOrTecnico && !isClienteDoChamado)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para acessar este anexo.");
            }

            return anexo;
        }

        public async Task AssignChamadoAsync(long chamadoId, string tecnicoEmail)
        {
            var tecnico = await _usuarioRepository.GetByEmailAsync(tecnicoEmail);
            if (tecnico == null || tecnico.Role == Role.CLIENTE) throw new BusinessRuleException("Utilizador técnico não encontrado ou inválido.");

            var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
            if (chamado == null) throw new NotFoundException($"Chamado com ID {chamadoId} não encontrado.");

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
            if (cliente == null) throw new NotFoundException("Utilizador cliente não encontrado.");

            var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
            if (chamado == null) throw new NotFoundException("Chamado não encontrado.");
            if (chamado.ClienteId != cliente.Id) throw new UnauthorizedAccessException("Você não tem permissão para avaliar este chamado.");
            if (chamado.Status != StatusChamado.FECHADO) throw new BusinessRuleException("Só é possível avaliar chamados que já foram fechados.");
            if (chamado.NotaAvaliacao.HasValue) throw new BusinessRuleException("Este chamado já foi avaliado anteriormente.");

            chamado.NotaAvaliacao = avaliacaoDto.Nota;
            chamado.ComentarioAvaliacao = avaliacaoDto.Comentario;
            await _chamadoRepository.UpdateAsync(chamado);
        }

        public async Task MergeChamadosAsync(long chamadoDuplicadoId, long chamadoPrincipalId, string tecnicoEmail)
        {
            if (chamadoDuplicadoId == chamadoPrincipalId) throw new BusinessRuleException("Não é possível mesclar um chamado nele mesmo.");

            var tecnico = await _usuarioRepository.GetByEmailAsync(tecnicoEmail);
            if (tecnico == null) throw new NotFoundException("Técnico não encontrado.");

            var chamadoDuplicado = await _context.Chamados.Include(c => c.Mensagens).Include(c => c.Anexos).FirstOrDefaultAsync(c => c.Id == chamadoDuplicadoId);
            var chamadoPrincipal = await _chamadoRepository.GetByIdAsync(chamadoPrincipalId);

            if (chamadoDuplicado == null || chamadoPrincipal == null) throw new NotFoundException("Um ou ambos os chamados não foram encontrados.");
            if (chamadoDuplicado.ClienteId != chamadoPrincipal.ClienteId) throw new BusinessRuleException("Só é possível mesclar chamados do mesmo cliente.");

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

                foreach (var mensagem in chamadoDuplicado.Mensagens.ToList())
                {
                    mensagem.ChamadoId = chamadoPrincipal.Id;
                }
                foreach (var anexo in chamadoDuplicado.Anexos.ToList())
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

        public async Task<IEnumerable<ChamadoResponseDto>> GetChamadosAtribuidosAsync(string tecnicoEmail)
        {
            var tecnico = await _usuarioRepository.GetByEmailAsync(tecnicoEmail);
            if (tecnico == null)
            {
                throw new NotFoundException("Técnico não encontrado.");
            }

            var chamados = await _chamadoRepository.GetByTecnicoIdAsync(tecnico.Id);

            return chamados.Select(c => new ChamadoResponseDto
            {
                Id = c.Id,
                Titulo = c.Titulo,
                NomeCliente = c.Cliente.Nome,
                NomeTecnicoResponsavel = c.TecnicoResponsavel?.Nome,
                Status = c.Status,
                Prioridade = c.Prioridade,
                DataCriacao = c.DataCriacao
            });
        }

        public async Task<IEnumerable<ChamadoResponseDto>> GetChamadosDisponiveisAsync()
        {
            var chamados = await _chamadoRepository.GetNaoAtribuidosAsync();

            return chamados.Select(c => new ChamadoResponseDto
            {
                Id = c.Id,
                Titulo = c.Titulo,
                NomeCliente = c.Cliente.Nome,
                NomeTecnicoResponsavel = null,
                Status = c.Status,
                Prioridade = c.Prioridade,
                DataCriacao = c.DataCriacao
            });
        }
    }
}