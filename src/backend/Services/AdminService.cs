using CajuAjuda.Backend.Exceptions;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services;

public class AdminService : IAdminService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IDashboardService _dashboardService;

    public AdminService(IUsuarioRepository usuarioRepository, IDashboardService dashboardService)
    {
        _usuarioRepository = usuarioRepository;
        _dashboardService = dashboardService;
    }

    public async Task<Usuario> CreateTecnicoAsync(TecnicoCreateDto tecnicoDto)
    {
        var existingUser = await _usuarioRepository.GetByEmailAsync(tecnicoDto.Email);
        if (existingUser != null)
        {
            throw new BusinessRuleException("Este e-mail já está cadastrado.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(tecnicoDto.Senha);

        var novoTecnico = new Usuario
        {
            Nome = tecnicoDto.Nome,
            Email = tecnicoDto.Email,
            Senha = passwordHash,
            Role = Role.TECNICO,
            Enabled = true
        };

        await _usuarioRepository.AddAsync(novoTecnico);
        return novoTecnico;
    }

    public async Task<IEnumerable<ClienteResponseDto>> GetAllClientesAsync()
    {
        var clientes = await _usuarioRepository.GetAllByRoleAsync(Role.CLIENTE);
        return clientes.Select(c => new ClienteResponseDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Email = c.Email,
            Enabled = c.Enabled
        });
    }

    public async Task<IEnumerable<TecnicoResponseDto>> GetAllTecnicosAsync()
    {
        var tecnicos = await _usuarioRepository.GetAllByRoleAsync(Role.TECNICO);
        return tecnicos.Select(t => new TecnicoResponseDto
        {
            Id = t.Id,
            Nome = t.Nome,
            Email = t.Email,
            Enabled = t.Enabled
        });
    }

    public Task<DashboardResponseDto> GetDashboardMetricsAsync()
    {
        return _dashboardService.GetDashboardMetricsAsync();
    }

    public async Task<string> ResetPasswordAsync(long id)
    {
        var novaSenha = "NovaSenha@123";
        var usuario = await _usuarioRepository.GetByIdAsync(id);
        if (usuario == null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }
        usuario.Senha = BCrypt.Net.BCrypt.HashPassword(novaSenha);
        await _usuarioRepository.UpdateAsync(usuario);
        return novaSenha;
    }

    public async Task<bool> ToggleClienteStatusAsync(long id)
    {
        var cliente = await _usuarioRepository.GetByIdAsync(id);
        if (cliente == null || cliente.Role != Role.CLIENTE)
        {
            throw new NotFoundException("Cliente não encontrado.");
        }
        cliente.Enabled = !cliente.Enabled;
        await _usuarioRepository.UpdateAsync(cliente);
        return cliente.Enabled;
    }

    public async Task<bool> ToggleTecnicoStatusAsync(long id)
    {
        var tecnico = await _usuarioRepository.GetByIdAsync(id);
        if (tecnico == null || tecnico.Role != Role.TECNICO)
        {
            throw new NotFoundException("Técnico não encontrado.");
        }
        tecnico.Enabled = !tecnico.Enabled;
        await _usuarioRepository.UpdateAsync(tecnico);
        return tecnico.Enabled;
    }

    
    public async Task<TecnicoResponseDto> UpdateTecnicoAsync(long id, TecnicoUpdateDto tecnicoDto)
    {
        var tecnico = await _usuarioRepository.GetByIdAsync(id);
        if (tecnico == null || tecnico.Role != Role.TECNICO)
        {
            throw new NotFoundException("Técnico não encontrado.");
        }

        // Verifica se o novo e-mail já está em uso por outro usuário
        var existingUserWithEmail = await _usuarioRepository.GetByEmailAsync(tecnicoDto.Email);
        if (existingUserWithEmail != null && existingUserWithEmail.Id != id)
        {
            throw new BusinessRuleException("O e-mail informado já está em uso por outra conta.");
        }

        tecnico.Nome = tecnicoDto.Nome;
        tecnico.Email = tecnicoDto.Email;

        await _usuarioRepository.UpdateAsync(tecnico);

        return new TecnicoResponseDto
        {
            Id = tecnico.Id,
            Nome = tecnico.Nome,
            Email = tecnico.Email,
            Enabled = tecnico.Enabled
        };
    }
}