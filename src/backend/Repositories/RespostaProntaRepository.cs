using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Repositories;

public class RespostaProntaRepository : IRespostaProntaRepository
{
    private readonly CajuAjudaDbContext _context;

    public RespostaProntaRepository(CajuAjudaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RespostaPronta>> GetAllAsync()
    {
        return await _context.RespostasProntas.ToListAsync();
    }
}