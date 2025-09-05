using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Exceptions;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CajuAjuda.Backend.Services;

public class AdminService : IAdminService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly CajuAjudaDbContext _context;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IUsuarioRepository usuarioRepository, CajuAjudaDbContext context, ILogger<AdminService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TecnicoResponseDto>> GetAllTecnicosAsync()
    {
        _logger.LogInformation("Buscando todos os técnicos.");
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
        _logger.LogInformation("Tentando atualizar o técnico com ID: {TecnicoId}", id);
        
        var tecnico = await _usuarioRepository.GetByIdAsync(id);
        if (tecnico == null || tecnico.Role != Role.TECNICO)
        {
            _logger.LogWarning("Técnico com ID: {TecnicoId} não encontrado para atualização.", id);
            throw new NotFoundException("Técnico não encontrado.");
        }

        var existingUserWithEmail = await _usuarioRepository.GetByEmailAsync(tecnicoUpdateDto.Email);
        if (existingUserWithEmail != null && existingUserWithEmail.Id != id)
        {
            _logger.LogWarning("Tentativa de atualizar técnico {TecnicoId} com e-mail já existente: {Email}", id, tecnicoUpdateDto.Email);
            throw new BusinessRuleException("O e-mail informado já está em uso por outra conta.");
        }

        tecnico.Nome = tecnicoUpdateDto.Nome;
        tecnico.Email = tecnicoUpdateDto.Email;

        await _usuarioRepository.UpdateAsync(tecnico);
        
        _logger.LogInformation("Técnico com ID: {TecnicoId} atualizado com sucesso.", id);
        
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
        _logger.LogInformation("Tentando alterar o status do técnico com ID: {TecnicoId}", id);
        
        var tecnico = await _usuarioRepository.GetByIdAsync(id);
        if (tecnico == null || tecnico.Role != Role.TECNICO)
        {
            _logger.LogWarning("Técnico com ID: {TecnicoId} não encontrado para alteração de status.", id);
            throw new NotFoundException("Técnico não encontrado.");
        }

        tecnico.Enabled = !tecnico.Enabled;
        await _usuarioRepository.UpdateAsync(tecnico);
        
        _logger.LogInformation("Status do técnico com ID: {TecnicoId} alterado para {Status}", id, tecnico.Enabled);
        return tecnico.Enabled;
    }

    public async Task<string> ResetPasswordAsync(long id)
    {
        _logger.LogInformation("Tentando redefinir a senha do técnico com ID: {TecnicoId}", id);
        
        var tecnico = await _usuarioRepository.GetByIdAsync(id);
        if (tecnico == null || tecnico.Role != Role.TECNICO)
        {
            _logger.LogWarning("Técnico com ID: {TecnicoId} não encontrado para redefinição de senha.", id);
            throw new NotFoundException("Técnico não encontrado.");
        }

        var novaSenha = GenerateRandomPassword();
        tecnico.Senha = BCrypt.Net.BCrypt.HashPassword(novaSenha);
        
        await _usuarioRepository.UpdateAsync(tecnico);
        
        _logger.LogWarning("Senha do técnico com ID: {TecnicoId} foi redefinida com sucesso. Esta é uma ação de segurança sensível.", id);

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