using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Repositories;

public interface IChamadoRepository
{
    Task AddAsync(Chamado chamado);
    Task<IEnumerable<Chamado>> GetByClienteIdAsync(long clienteId);
    Task<IEnumerable<Chamado>> GetAllAsync();
    Task<Chamado?> GetByIdAsync(long id);
    Task UpdateAsync(Chamado chamado); 
}