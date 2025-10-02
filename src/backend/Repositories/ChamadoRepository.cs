// CajuAjuda.Backend/Repositories/ChamadoRepository.cs

using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Repositories
{
    public class ChamadoRepository : IChamadoRepository
    {
        private readonly CajuAjudaDbContext _context;

        public ChamadoRepository(CajuAjudaDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Chamado chamado)
        {
            _context.Chamados.Add(chamado);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Chamado>> GetByClienteIdAsync(long clienteId)
        {
            return await _context.Chamados
                .Where(c => c.ClienteId == clienteId)
                .OrderByDescending(c => c.DataCriacao)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Chamado> Chamados, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, StatusChamado? status, PrioridadeChamado? prioridade)
        {
            var query = _context.Chamados.Include(c => c.Cliente).AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            if (prioridade.HasValue)
            {
                query = query.Where(c => c.Prioridade == prioridade.Value);
            }

            var totalCount = await query.CountAsync();

            var chamados = await query
                .OrderByDescending(c => c.DataCriacao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (chamados, totalCount);
        }

        public async Task<Chamado?> GetByIdAsync(long id)
        {
            return await _context.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.TecnicoResponsavel) 
                .Include(c => c.Mensagens)
                    .ThenInclude(m => m.Autor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Chamado chamado)
        {
            _context.Chamados.Update(chamado);
            await _context.SaveChangesAsync();
        }
    }
}