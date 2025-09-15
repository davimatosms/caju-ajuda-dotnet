using CajuAjuda.Backend.Exceptions;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEmailService _emailService;

    public UsuarioService(IUsuarioRepository usuarioRepository, IEmailService emailService)
    {
        _usuarioRepository = usuarioRepository;
        _emailService = emailService;
    }

    public async Task<Usuario?> AuthenticateAsync(LoginDto loginDto)
    {
        var user = await _usuarioRepository.GetByEmailAsync(loginDto.Email);

        if (user == null || !user.Enabled || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, user.Senha))
        {
            return null;
        }

        return user;
    }

    public async Task<Usuario> RegisterClienteAsync(UsuarioCreateDto usuarioDto)
    {
        var existingUser = await _usuarioRepository.GetByEmailAsync(usuarioDto.Email);
        if (existingUser != null)
        {
            throw new BusinessRuleException("Este e-mail já está cadastrado.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha);

        var novoUsuario = new Usuario
        {
            Nome = usuarioDto.Nome,
            Email = usuarioDto.Email,
            Senha = passwordHash,
            Role = Role.CLIENTE,
            Enabled = true, // Mudamos para true para facilitar o desenvolvimento sem verificação de email
            VerificationToken = null 
        };

        await _usuarioRepository.AddAsync(novoUsuario);
        return novoUsuario;
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _usuarioRepository.GetByVerificationTokenAsync(token);

        if (user == null)
        {
            return false;
        }

        user.Enabled = true;
        user.VerificationToken = null;

        await _usuarioRepository.UpdateAsync(user);
        return true;
    }
    
    // --- NOVOS MÉTODOS PARA O PERFIL ---

    public async Task<PerfilResponseDto> GetPerfilAsync(string userEmail)
    {
        var user = await _usuarioRepository.GetByEmailAsync(userEmail);
        if (user == null) throw new NotFoundException("Usuário não encontrado.");

        return new PerfilResponseDto
        {
            Nome = user.Nome,
            Email = user.Email
        };
    }

    public async Task UpdatePerfilAsync(string userEmail, PerfilUpdateDto perfilDto)
    {
        var user = await _usuarioRepository.GetByEmailAsync(userEmail);
        if (user == null) throw new NotFoundException("Usuário não encontrado.");

        if (user.Email != perfilDto.Email)
        {
            var existingUserWithEmail = await _usuarioRepository.GetByEmailAsync(perfilDto.Email);
            if (existingUserWithEmail != null)
            {
                throw new BusinessRuleException("O e-mail informado já está em uso por outra conta.");
            }
        }
        
        user.Nome = perfilDto.Nome;
        user.Email = perfilDto.Email;
        await _usuarioRepository.UpdateAsync(user);
    }

    public async Task UpdateSenhaAsync(string userEmail, SenhaUpdateDto senhaDto)
    {
        var user = await _usuarioRepository.GetByEmailAsync(userEmail);
        if (user == null) throw new NotFoundException("Usuário não encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(senhaDto.SenhaAtual, user.Senha))
        {
            throw new BusinessRuleException("A senha atual está incorreta.");
        }

        user.Senha = BCrypt.Net.BCrypt.HashPassword(senhaDto.NovaSenha);
        await _usuarioRepository.UpdateAsync(user);
    }
}