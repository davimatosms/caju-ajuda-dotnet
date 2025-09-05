using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services;

public interface IRespostaProntaService
{
    Task<IEnumerable<RespostaPronta>> GetAllAsync();
}