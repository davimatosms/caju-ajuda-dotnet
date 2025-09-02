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

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("tecnicos")]
    public async Task<IActionResult> GetAllTecnicos()
    {
        var tecnicos = await _adminService.GetAllTecnicosAsync();
        return Ok(tecnicos);
    }
    
    [HttpPut("tecnicos/{id}")]
    public async Task<IActionResult> UpdateTecnico(long id, [FromBody] TecnicoUpdateDto tecnicoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var tecnicoAtualizado = await _adminService.UpdateTecnicoAsync(id, tecnicoDto);
            return Ok(tecnicoAtualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPatch("tecnicos/{id}/status")]
    public async Task<IActionResult> ToggleTecnicoStatus(long id)
    {
        try
        {
            var novoStatus = await _adminService.ToggleTecnicoStatusAsync(id);
            return Ok(new { message = $"Status do técnico {id} atualizado.", enabled = novoStatus });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [HttpPost("tecnicos/{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(long id)
    {
        try
        {
            var novaSenha = await _adminService.ResetPasswordAsync(id);
            return Ok(new { message = $"Senha do técnico {id} redefinida com sucesso.", temporaryPassword = novaSenha });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}