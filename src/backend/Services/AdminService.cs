using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Services;

public class AdminService : IAdminService
{
    private readonly CajuAjudaDbContext _context;

    public AdminService(CajuAjudaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TecnicoResponseDto>> GetAllTecnicosAsync()
    {
        var tecnicos = await _context.Usuarios
            .Where(u => u.Role == Role.TECNICO)
            .Select(u => new TecnicoResponseDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Enabled = u.Enabled
            })
            .ToListAsync();

        return tecnicos;
    }
}