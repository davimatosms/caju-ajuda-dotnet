using CajuAjuda.Backend.Services.Dtos;

namespace CajuAjuda.Backend.Services;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardMetricsAsync();
}