using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByIdAsync(long id);
    Task AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task<Usuario?> GetByVerificationTokenAsync(string token);
}