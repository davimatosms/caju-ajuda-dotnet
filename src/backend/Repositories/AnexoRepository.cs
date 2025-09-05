using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;

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
}