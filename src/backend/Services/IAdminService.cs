using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

public interface IAdminService
{
    Task<IEnumerable<TecnicoResponseDto>> GetAllTecnicosAsync();
}