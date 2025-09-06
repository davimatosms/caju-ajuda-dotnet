using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IDashboardService _dashboardService;

    public AdminController(IAdminService adminService, IDashboardService dashboardService)
    {
        _adminService = adminService;
        _dashboardService = dashboardService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        var metrics = await _dashboardService.GetDashboardMetricsAsync();
        return Ok(metrics);
    }

    // --- ENDPOINTS DE TÉCNICOS ---
    [HttpGet("tecnicos")]
    public async Task<IActionResult> GetAllTecnicos()
    {
        var tecnicos = await _adminService.GetAllTecnicosAsync();
        return Ok(tecnicos);
    }
    
    [HttpPut("tecnicos/{id}")]
    public async Task<IActionResult> UpdateTecnico(long id, [FromBody] TecnicoUpdateDto tecnicoDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var tecnicoAtualizado = await _adminService.UpdateTecnicoAsync(id, tecnicoDto);
        return Ok(tecnicoAtualizado);
    }
    
    [HttpPatch("tecnicos/{id}/status")]
    public async Task<IActionResult> ToggleTecnicoStatus(long id)
    {
        var novoStatus = await _adminService.ToggleTecnicoStatusAsync(id);
        return Ok(new { message = $"Status do técnico {id} atualizado.", enabled = novoStatus });
    }
    
    [HttpPost("tecnicos/{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(long id)
    {
        var novaSenha = await _adminService.ResetPasswordAsync(id);
        return Ok(new { message = $"Senha do técnico {id} redefinida com sucesso.", temporaryPassword = novaSenha });
    }

    // --- ENDPOINTS NOVOS DE CLIENTES ---
    [HttpGet("clientes")] // Rota: GET api/admin/clientes
    public async Task<IActionResult> GetAllClientes()
    {
        var clientes = await _adminService.GetAllClientesAsync();
        return Ok(clientes);
    }

    [HttpPatch("clientes/{id}/status")] // Rota: PATCH api/admin/clientes/123/status
    public async Task<IActionResult> ToggleClienteStatus(long id)
    {
        var novoStatus = await _adminService.ToggleClienteStatusAsync(id);
        return Ok(new { message = $"Status do cliente {id} atualizado.", enabled = novoStatus });
    }
}