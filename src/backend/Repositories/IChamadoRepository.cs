using CajuAjuda.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Repositories
{
    public interface IChamadoRepository
    {
        Task<(IEnumerable<Chamado> chamados, int totalCount)> GetAllAsync(int pageNumber, int pageSize, StatusChamado? status, PrioridadeChamado? prioridade);
        Task<Chamado?> GetByIdAsync(long id);
        Task AddAsync(Chamado chamado);
        Task UpdateAsync(Chamado chamado);
        Task<IEnumerable<Chamado>> GetByClienteIdAsync(long clienteId);

        Task<IEnumerable<Chamado>> GetByTecnicoIdAsync(long tecnicoId);
        Task<IEnumerable<Chamado>> GetNaoAtribuidosAsync();
    }
}