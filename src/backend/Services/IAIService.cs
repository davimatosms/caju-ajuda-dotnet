using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services;

public interface IAIService
{
    Task<PrioridadeChamado> DefinirPrioridadeAsync(string titulo, string descricao);
}