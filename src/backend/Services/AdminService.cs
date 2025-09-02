using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Services;

public class AdminService : IAdminService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly CajuAjudaDbContext _context;

    public AdminService(IUsuarioRepository usuarioRepository, CajuAjudaDbContext context)
    {
        _usuarioRepository = usuarioRepository;
        _context = context;
    }

    public async Task<IEnumerable<TecnicoResponseDto>> GetAllTecnicosAsync()
    {
        return await _context.Usuarios
            .Where(u => u.Role == Role.TECNICO)
            .Select(u => new TecnicoResponseDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Enabled = u.Enabled
            })
            .ToListAsync();
    }

    public async Task<TecnicoResponseDto> UpdateTecnicoAsync(long id, TecnicoUpdateDto tecnicoUpdateDto)
    {
        var tecnico = await _usuarioRepository.GetByIdAsync(id);
        if (tecnico == null || tecnico.Role != Role.TECNICO)
        {
            throw new KeyNotFoundException("Técnico não encontrado.");
        }

        var existingUserWithEmail = await _usuarioRepository.GetByEmailAsync(tecnicoUpdateDto.Email);
        if (existingUserWithEmail != null && existingUserWithEmail.Id != id)
        {
            throw new Exception("O e-mail informado já está em uso por outra conta.");
        }

        tecnico.Nome = tecnicoUpdateDto.Nome;
        tecnico.Email = tecnicoUpdateDto.Email;

        await _usuarioRepository.UpdateAsync(tecnico);

        return new TecnicoResponseDto
        {
            Id = tecnico.Id,
            Nome = tecnico.Nome,
            Email = tecnico.Email,
            Enabled = tecnico.Enabled
        };
    }

    public async Task<bool> ToggleTecnicoStatusAsync(long id)
    {
        var tecnico = await _usuarioRepository.GetByIdAsync(id);
        if (tecnico == null || tecnico.Role != Role.TECNICO)
        {
            throw new KeyNotFoundException("Técnico não encontrado.");
        }

        tecnico.Enabled = !tecnico.Enabled;
        await _usuarioRepository.UpdateAsync(tecnico);
        
        return tecnico.Enabled;
    }

    public async Task<string> ResetPasswordAsync(long id)
    {
        var tecnico = await _usuarioRepository.GetByIdAsync(id);
        if (tecnico == null || tecnico.Role != Role.TECNICO)
        {
            throw new KeyNotFoundException("Técnico não encontrado.");
        }

        // Gera uma nova senha aleatória e segura
        var novaSenha = GenerateRandomPassword();
        tecnico.Senha = BCrypt.Net.BCrypt.HashPassword(novaSenha);
        
        await _usuarioRepository.UpdateAsync(tecnico);

        // Futuramente, esta senha seria enviada por e-mail em vez de ser retornada na API.
        // Por agora, retornamos para facilitar os testes.
        return novaSenha;
    }

    private static string GenerateRandomPassword(int length = 12)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        var random = new Random();
        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = validChars[random.Next(validChars.Length)];
        }
        return new string(chars);
    }
}