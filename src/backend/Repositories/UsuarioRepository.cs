using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly CajuAjudaDbContext _context;

    public UsuarioRepository(CajuAjudaDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<Usuario?> GetByIdAsync(long id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> GetByVerificationTokenAsync(string token)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.VerificationToken == token);
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    // NOVO MÉTODO ADICIONADO AQUI
    public async Task<IEnumerable<Usuario>> GetAllByRoleAsync(Role role)
    {
        return await _context.Usuarios
            .Where(u => u.Role == role)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }
}