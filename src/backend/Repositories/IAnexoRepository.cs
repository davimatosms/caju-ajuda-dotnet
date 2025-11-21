using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Repositories;

public interface IAnexoRepository
{
    Task AddAsync(Anexo anexo);
    Task<Anexo?> GetByIdAsync(long id);
    Task<List<Anexo>> GetByChamadoIdAsync(long chamadoId);
}