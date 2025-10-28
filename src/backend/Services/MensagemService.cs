using CajuAjuda.Backend.Exceptions;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Adicionado para logging
using CajuAjuda.Backend.Hubs;      // Adicionado - Namespace do seu Hub
using Microsoft.AspNetCore.SignalR; // Adicionado - Namespace do SignalR Core

namespace CajuAjuda.Backend.Services
{
    public class MensagemService : IMensagemService
    {
        private readonly IMensagemRepository _mensagemRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IChamadoRepository _chamadoRepository;
        private readonly IHubContext<NotificacaoHub> _hubContext; // <- Injeção do HubContext
        private readonly ILogger<MensagemService> _logger;         // <- Injeção do Logger

        // Construtor atualizado para receber IHubContext e ILogger
        public MensagemService(
            IMensagemRepository mensagemRepository,
            IUsuarioRepository usuarioRepository,
            IChamadoRepository chamadoRepository,
            IHubContext<NotificacaoHub> hubContext, // <- Adicionado
            ILogger<MensagemService> logger)        // <- Adicionado
        {
            _mensagemRepository = mensagemRepository;
            _usuarioRepository = usuarioRepository;
            _chamadoRepository = chamadoRepository;
            _hubContext = hubContext; // <- Atribuído
            _logger = logger;         // <- Atribuído
        }

        public async Task<MensagemResponseDto> AddMensagemAsync(long chamadoId, MensagemCreateDto mensagemDto, string userEmail, string userRole)
        {
            _logger.LogInformation("Iniciando AddMensagemAsync para chamado {ChamadoId} por {UserEmail}", chamadoId, userEmail);

            var autor = await _usuarioRepository.GetByEmailAsync(userEmail);
            if (autor == null)
            {
                _logger.LogWarning("Autor não encontrado para email {UserEmail} ao adicionar mensagem.", userEmail);
                throw new NotFoundException("Utilizador autor não encontrado.");
            }

            var chamado = await _chamadoRepository.GetByIdAsync(chamadoId); // Assume que GetByIdAsync inclui o Cliente
            if (chamado == null)
            {
                _logger.LogWarning("Chamado {ChamadoId} não encontrado ao adicionar mensagem.", chamadoId);
                throw new NotFoundException("Chamado não encontrado.");
            }

            // --- Validação de Permissão ---
            bool isAdminOrTecnico = autor.Role == Role.ADMIN || autor.Role == Role.TECNICO;
            bool isClienteDoChamado = autor.Role == Role.CLIENTE && chamado.ClienteId == autor.Id;

            if (!isAdminOrTecnico && !isClienteDoChamado)
            {
                _logger.LogWarning("Acesso não autorizado negado para {UserEmail} no chamado {ChamadoId}.", userEmail, chamadoId);
                throw new UnauthorizedAccessException("Você não tem permissão para adicionar mensagens a este chamado.");
            }

            // --- Validação de Status do Chamado ---
            if (chamado.Status == StatusChamado.FECHADO || chamado.Status == StatusChamado.CANCELADO) // Adicionado CANCELADO
            {
                _logger.LogWarning("Tentativa de adicionar mensagem ao chamado {ChamadoId} com status {Status}.", chamadoId, chamado.Status);
                throw new BusinessRuleException("Não é possível adicionar mensagens a um chamado fechado ou cancelado.");
            }

            // --- Criação da Mensagem ---
            var novaMensagem = new Mensagem
            {
                Texto = mensagemDto.Texto,
                DataEnvio = DateTime.UtcNow,
                AutorId = autor.Id,
                ChamadoId = chamadoId,
                IsNotaInterna = false, // AddMensagemAsync é sempre para mensagens visíveis
                // Define LidoPeloCliente: Se quem enviou foi Cliente, o técnico precisa ler (false). Se foi técnico/admin, o cliente precisa ler (false).
                LidoPeloCliente = (autor.Role == Role.CLIENTE), // Simplificado: Se for cliente, já marca como lido por ele mesmo.
                // TODO: Futuramente, implementar lógica de "LidoPeloTecnico" se necessário.
                Autor = autor // Atribui o objeto autor para uso posterior no SignalR
            };

            // --- Atualização de Status do Chamado (Regra de Negócio) ---
            // Se um técnico/admin responde, muda para Aguardando Cliente
            // Se o cliente responde, volta para Em Andamento (se não estava Aberto)
            // if (isAdminOrTecnico && chamado.Status != StatusChamado.FECHADO && chamado.Status != StatusChamado.CANCELADO)
            // {
            //     chamado.Status = StatusChamado.AGUARDANDO_CLIENTE; // Removido Status AGUARDANDO_CLIENTE
            // }
            // else if (isClienteDoChamado && chamado.Status != StatusChamado.ABERTO && chamado.Status != StatusChamado.FECHADO && chamado.Status != StatusChamado.CANCELADO)
            // {
            //     chamado.Status = StatusChamado.EM_ANDAMENTO;
            // }

            // Salva a mensagem no banco de dados ANTES de enviar pelo SignalR
            await _mensagemRepository.AddAsync(novaMensagem);
            _logger.LogInformation("Mensagem {MensagemId} salva no banco para o chamado {ChamadoId}.", novaMensagem.Id, chamadoId);

            // Se o status do chamado foi alterado, salva a alteração
            // if (_context.Entry(chamado).State == EntityState.Modified) // Verifica se houve mudança no objeto chamado
            // {
            //     await _chamadoRepository.UpdateAsync(chamado);
            //     _logger.LogInformation("Status do chamado {ChamadoId} atualizado para {Status}.", chamadoId, chamado.Status);
            // }


            // --- ENVIO VIA SIGNALR ---
            try
            {
                string roomName = $"chamado_{chamadoId}";

                // Cria um DTO específico para o SignalR, para não expor a entidade completa
                var mensagemParaSignalR = new MensagemResponseDto
                {
                    Id = novaMensagem.Id,
                    Texto = novaMensagem.Texto,
                    DataEnvio = novaMensagem.DataEnvio,
                    AutorNome = novaMensagem.Autor.Nome, // Usa o objeto Autor carregado
                    AutorId = novaMensagem.AutorId,
                    IsNotaInterna = novaMensagem.IsNotaInterna
                };

                // IMPORTANTE: O nome do método "ReceiveNewMessage" DEVE ser o mesmo
                // que está sendo ouvido no hook useSignalR do React Native.
                await _hubContext.Clients.Group(roomName).SendAsync("ReceiveNewMessage", mensagemParaSignalR);
                _logger.LogInformation("Mensagem {MensagemId} enviada via SignalR para a sala: {RoomName}", novaMensagem.Id, roomName);
            }
            catch (Exception ex)
            {
                // Loga um erro se o envio SignalR falhar, mas não lança a exceção para não
                // reverter a operação principal (salvar a mensagem).
                _logger.LogError(ex, "Falha ao enviar mensagem via SignalR para a sala do chamado {ChamadoId}.", chamadoId);
            }
            // --- FIM DO ENVIO ---

            // Retorna o DTO da mensagem criada para a resposta da API REST
            return new MensagemResponseDto
            {
                Id = novaMensagem.Id,
                Texto = novaMensagem.Texto,
                DataEnvio = novaMensagem.DataEnvio,
                AutorNome = autor.Nome, // Retorna o nome do autor
                AutorId = autor.Id,
                IsNotaInterna = novaMensagem.IsNotaInterna
            };
        }

        // Método para adicionar nota interna (NÃO envia SignalR)
        public async Task<MensagemResponseDto> AddNotaInternaAsync(long chamadoId, MensagemCreateDto notaDto, string tecnicoEmail)
        {
            _logger.LogInformation("Iniciando AddNotaInternaAsync para chamado {ChamadoId} por {TecnicoEmail}", chamadoId, tecnicoEmail);

            var autor = await _usuarioRepository.GetByEmailAsync(tecnicoEmail);
            if (autor == null || autor.Role == Role.CLIENTE)
            {
                _logger.LogWarning("Acesso não autorizado negado para {TecnicoEmail} ao adicionar nota interna no chamado {ChamadoId}.", tecnicoEmail, chamadoId);
                throw new UnauthorizedAccessException("Apenas técnicos ou administradores podem adicionar notas internas.");
            }

            var chamado = await _chamadoRepository.GetByIdAsync(chamadoId);
            if (chamado == null)
            {
                _logger.LogWarning("Chamado {ChamadoId} não encontrado ao adicionar nota interna.", chamadoId);
                throw new NotFoundException("Chamado não encontrado.");
            }

            var novaNota = new Mensagem
            {
                Texto = notaDto.Texto,
                DataEnvio = DateTime.UtcNow,
                AutorId = autor.Id,
                ChamadoId = chamadoId,
                IsNotaInterna = true, // Marcado como nota interna
                LidoPeloCliente = false // Notas internas nunca são lidas pelo cliente
            };

            await _mensagemRepository.AddAsync(novaNota);
            _logger.LogInformation("Nota interna {MensagemId} salva no banco para o chamado {ChamadoId}.", novaNota.Id, chamadoId);

            // Retorna o DTO da nota criada
            return new MensagemResponseDto
            {
                Id = novaNota.Id,
                Texto = novaNota.Texto,
                DataEnvio = novaNota.DataEnvio,
                AutorNome = autor.Nome,
                AutorId = autor.Id,
                IsNotaInterna = novaNota.IsNotaInterna
            };
        }
    }
}