using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Repositories;

public interface IChamadoRepository
{
    Task AddAsync(Chamado chamado);
    Task<IEnumerable<Chamado>> GetByClienteIdAsync(long clienteId);
    Task<(IEnumerable<Chamado> Chamados, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, StatusChamado? status, PrioridadeChamado? prioridade);
    Task<Chamado?> GetByIdAsync(long id);
    Task UpdateAsync(Chamado chamado);
}