using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Repositories;

public interface IRespostaProntaRepository
{
    Task<IEnumerable<RespostaPronta>> GetAllAsync();
}