using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

public interface IAdminService
{
    Task<IEnumerable<TecnicoResponseDto>> GetAllTecnicosAsync();
    Task<TecnicoResponseDto> UpdateTecnicoAsync(long id, TecnicoUpdateDto tecnicoUpdateDto);
    Task<bool> ToggleTecnicoStatusAsync(long id);
    Task<string> ResetPasswordAsync(long id);
}