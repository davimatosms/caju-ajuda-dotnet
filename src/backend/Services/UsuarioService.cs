using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Usuario> RegisterClienteAsync(UsuarioCreateDto usuarioDto)
    {
        // 1. Verificar se o e-mail já existe
        var existingUser = await _usuarioRepository.GetByEmailAsync(usuarioDto.Email);
        if (existingUser != null)
        {
            // Lança uma exceção que o Controller tratará depois
            throw new Exception("Este e-mail já está cadastrado.");
        }

        // 2. Criptografar a senha
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha);

        // 3. Criar a nova entidade Usuario
        var novoUsuario = new Usuario
        {
            Nome = usuarioDto.Nome,
            Email = usuarioDto.Email,
            Senha = passwordHash,
            Role = Role.CLIENTE, // Todo novo cadastro via este método é um cliente
            Enabled = true
        };

        // 4. Salvar o novo usuário no banco de dados
        await _usuarioRepository.AddAsync(novoUsuario);

        return novoUsuario;
    }

    public async Task<Usuario?> AuthenticateAsync(LoginDto loginDto)
    {
        var user = await _usuarioRepository.GetByEmailAsync(loginDto.Email);

        // Se o usuário não existe ou a senha está incorreta (após verificação do hash), retorna nulo
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, user.Senha))
        {
            return null;
        }

        return user;
    }
}