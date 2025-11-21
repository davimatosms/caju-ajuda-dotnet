using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks; // Necessário para Task
using Microsoft.AspNetCore.Authorization; // Opcional: Se quiser autorizar o Hub
using System.Security.Claims; // Para acessar ClaimTypes
using System.IdentityModel.Tokens.Jwt; // Para JwtRegisteredClaimNames

namespace CajuAjuda.Backend.Hubs;

// [Authorize] // Descomente se quiser que apenas usu�rios autenticados (com token JWT v�lido) possam conectar ao Hub
public class NotificacaoHub : Hub
{
    private readonly ILogger<NotificacaoHub> _logger;

    // Opcional: Injetar ILogger para registrar eventos
    public NotificacaoHub(ILogger<NotificacaoHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// M�todo chamado pelo cliente (React Native) para entrar em um grupo (sala) espec�fico de um chamado.
    /// O nome do m�todo aqui ("JoinRoom") deve corresponder exatamente ao que o cliente chama via invoke.
    /// </summary>
    /// <param name="roomName">O nome da sala (ex: "chamado_123").</param>
    public async Task JoinRoom(string roomName)
    {
        // Adiciona a conexão atual (identificada por Context.ConnectionId) ao grupo SignalR especificado.
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        
        // Obter informações do usuário do contexto
        var userName = Context.User?.Identity?.Name 
            ?? Context.User?.FindFirst(ClaimTypes.Email)?.Value 
            ?? Context.User?.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? "Anônimo";
        
        _logger.LogInformation("[SignalR] 👤 Cliente {ConnectionId} ({UserName}) entrou na sala: {RoomName}", 
            Context.ConnectionId, userName, roomName);

        // Opcional: Enviar uma mensagem de confirmação de volta apenas para o cliente que acabou de entrar.
        // await Clients.Caller.SendAsync("ConfirmationMessage", $"Você entrou na sala {roomName}");
    }

    /// <summary>
    /// Opcional: M�todo chamado pelo cliente para sair explicitamente de um grupo.
    /// Geralmente n�o � necess�rio, pois o SignalR remove a conex�o do grupo automaticamente ao desconectar.
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
        // Voc� poderia adicionar l�gica aqui se necess�rio, como registrar a conex�o
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Chamado automaticamente quando um cliente se desconecta.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // O SignalR remove automaticamente a conex�o de todos os grupos dos quais ela fazia parte.
        _logger.LogWarning(exception, "Cliente SignalR desconectado: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}