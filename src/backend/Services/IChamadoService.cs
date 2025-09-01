using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

public interface IChamadoService
{
    Task<Chamado> CreateAsync(ChamadoCreateDto chamadoDto, string clienteEmail);
    Task<IEnumerable<Chamado>> GetChamadosByClienteEmailAsync(string clienteEmail);
    Task<IEnumerable<ChamadoResponseDto>> GetAllChamadosAsync();
    Task<ChamadoDetailResponseDto?> GetChamadoByIdAsync(long chamadoId, string userEmail, string userRole);
        Task UpdateChamadoStatusAsync(long chamadoId, StatusChamado novoStatus);
}