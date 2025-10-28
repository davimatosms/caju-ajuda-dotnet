using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks; // Necessário para Task
using Microsoft.AspNetCore.Authorization; // Opcional: Se quiser autorizar o Hub

namespace CajuAjuda.Backend.Hubs;

// [Authorize] // Descomente se quiser que apenas usuários autenticados (com token JWT válido) possam conectar ao Hub
public class NotificacaoHub : Hub
{
    private readonly ILogger<NotificacaoHub> _logger;

    // Opcional: Injetar ILogger para registrar eventos
    public NotificacaoHub(ILogger<NotificacaoHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Método chamado pelo cliente (React Native) para entrar em um grupo (sala) específico de um chamado.
    /// O nome do método aqui ("JoinRoom") deve corresponder exatamente ao que o cliente chama via invoke.
    /// </summary>
    /// <param name="roomName">O nome da sala (ex: "chamado_123").</param>
    public async Task JoinRoom(string roomName)
    {
        // Adiciona a conexão atual (identificada por Context.ConnectionId) ao grupo SignalR especificado.
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        _logger.LogInformation("Cliente SignalR {ConnectionId} entrou na sala: {RoomName}", Context.ConnectionId, roomName);

        // Opcional: Enviar uma mensagem de confirmação de volta apenas para o cliente que acabou de entrar.
        // await Clients.Caller.SendAsync("ConfirmationMessage", $"Você entrou na sala {roomName}");
    }

    /// <summary>
    /// Opcional: Método chamado pelo cliente para sair explicitamente de um grupo.
    /// Geralmente não é necessário, pois o SignalR remove a conexão do grupo automaticamente ao desconectar.
    /// </summary>
    /// <param name="roomName">O nome da sala a sair.</param>
    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        _logger.LogInformation("Cliente SignalR {ConnectionId} saiu da sala: {RoomName}", Context.ConnectionId, roomName);
    }

    /// <summary>
    /// Chamado automaticamente quando um cliente se conecta.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Cliente SignalR conectado: {ConnectionId}", Context.ConnectionId);
        // Você poderia adicionar lógica aqui se necessário, como registrar a conexão
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Chamado automaticamente quando um cliente se desconecta.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // O SignalR remove automaticamente a conexão de todos os grupos dos quais ela fazia parte.
        _logger.LogWarning(exception, "Cliente SignalR desconectado: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}