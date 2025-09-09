using CajuAjuda.Backend.Exceptions;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;
using System.Security.Cryptography;

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
            Enabled = true, // O utilizador começa desativado
            VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)) // Gera um token seguro
        };

        await _usuarioRepository.AddAsync(novoUsuario);

        // Envia o e-mail de verificação
        var verificationLink = $"https://localhost:7113/api/auth/verify?token={novoUsuario.VerificationToken}";
        var emailBody = $"<h1>Bem-vindo ao Caju Ajuda!</h1><p>Por favor, clique no link abaixo para verificar seu e-mail e ativar sua conta:</p><a href='{verificationLink}'>Verificar E-mail</a>";
        
        // await _emailService.SendEmailAsync(novoUsuario.Email, "Verifique seu e-mail - Caju Ajuda", emailBody);

        return novoUsuario;
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

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _usuarioRepository.GetByVerificationTokenAsync(token);

        if (user == null)
        {
            return false;
        }

        user.Enabled = true;
        user.VerificationToken = null; // Limpa o token após o uso

        await _usuarioRepository.UpdateAsync(user);
        return true;
    }
}