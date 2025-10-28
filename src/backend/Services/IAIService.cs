using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services;

public interface IAIService
{
    Task<PrioridadeChamado> DefinirPrioridadeAsync(string titulo, string descricao);
    Task<string> SugerirSolucaoAsync(string titulo, string descricao, List<Mensagem>? historicoMensagens = null);
    Task<string> AnalisarContextoEResponderAsync(long chamadoId, string novaMensagem, List<Mensagem> historicoMensagens);
}