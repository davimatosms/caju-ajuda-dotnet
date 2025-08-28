using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly CajuAjudaDbContext _context;

    // O DbContext é injetado aqui pelo sistema de injeção de dependência do .NET
    public UsuarioRepository(CajuAjudaDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        // Usa LINQ para buscar o primeiro usuário que corresponde ao e-mail
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(Usuario usuario)
    {
        // Adiciona o novo usuário ao DbContext
        _context.Usuarios.Add(usuario);
        // Salva as mudanças no banco de dados
        await _context.SaveChangesAsync();
    }
}