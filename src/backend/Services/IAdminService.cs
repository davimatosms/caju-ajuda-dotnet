using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Services;

public interface IAdminService
{
    Task<DashboardResponseDto> GetDashboardMetricsAsync();
    Task<IEnumerable<TecnicoResponseDto>> GetAllTecnicosAsync();
    Task<TecnicoResponseDto> UpdateTecnicoAsync(long id, TecnicoUpdateDto tecnicoDto); 
    Task<bool> ToggleTecnicoStatusAsync(long id);
    Task<string> ResetPasswordAsync(long id);
    Task<IEnumerable<ClienteResponseDto>> GetAllClientesAsync();
    Task<bool> ToggleClienteStatusAsync(long id);
    Task<Usuario> CreateTecnicoAsync(TecnicoCreateDto tecnicoDto);
}