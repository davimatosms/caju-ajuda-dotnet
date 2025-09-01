using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email); // Busca um usuário pelo e-mail
    Task AddAsync(Usuario usuario);               // Adiciona um novo usuário
    // Futuramente, adicionaremos aqui métodos como Update, Delete, GetById, etc.
}