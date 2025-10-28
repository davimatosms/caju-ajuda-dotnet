using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Repositories;

public class AnexoRepository : IAnexoRepository
{
    private readonly CajuAjudaDbContext _context;

    public AnexoRepository(CajuAjudaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Anexo anexo)
    {
        _context.Anexos.Add(anexo);
        await _context.SaveChangesAsync();
    }

    public async Task<Anexo?> GetByIdAsync(long id)
    {
        return await _context.Anexos
            .Include(a => a.Chamado)
            .ThenInclude(c => c.Cliente)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Anexo>> GetByChamadoIdAsync(long chamadoId)
    {
        return await _context.Anexos
            .Where(a => a.ChamadoId == chamadoId)
            .OrderBy(a => a.Id)
            .ToListAsync();
    }
}