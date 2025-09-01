using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "TECNICO,ADMIN")] // Protege todos os endpoints para Técnicos e Admins
public class TecnicoController : ControllerBase
{
    private readonly IChamadoService _chamadoService;
    private readonly IMensagemService _mensagemService;

    public TecnicoController(IChamadoService chamadoService, IMensagemService mensagemService)
    {
        _chamadoService = chamadoService;
        _mensagemService = mensagemService;
    }

    [HttpGet("chamados")] // Rota: GET api/tecnico/chamados
    public async Task<IActionResult> GetAllChamados()
    {
        try
        {
            var chamados = await _chamadoService.GetAllChamadosAsync();
            return Ok(chamados);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro interno ao buscar os chamados.", error = ex.Message });
        }
    }
    
    [HttpGet("chamados/{id}")] // Rota: GET api/tecnico/chamados/123
    public async Task<IActionResult> GetChamadoById(long id)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (userEmail == null || userRole == null)
        {
            return Unauthorized();
        }

        try
        {
            var chamado = await _chamadoService.GetChamadoByIdAsync(id, userEmail, userRole);
            if (chamado == null)
            {
                return NotFound(new { message = $"Chamado com ID {id} não encontrado." });
            }
            return Ok(chamado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro interno.", error = ex.Message });
        }
    }

    [HttpPost("chamados/{id}/mensagens")] // Rota: POST api/tecnico/chamados/123/mensagens
    public async Task<IActionResult> AddMensagemTecnico(long id, [FromBody] MensagemCreateDto mensagemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (userEmail == null || userRole == null)
        {
            return Unauthorized();
        }

        try
        {
            var novaMensagem = await _mensagemService.AddMensagemAsync(id, mensagemDto, userEmail, userRole);
            return Ok(novaMensagem);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPatch("chamados/{id}/status")] // Rota: PATCH api/tecnico/chamados/123/status
    public async Task<IActionResult> UpdateChamadoStatus(long id, [FromBody] ChamadoUpdateStatusDto statusDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _chamadoService.UpdateChamadoStatusAsync(id, statusDto.NovoStatus);
            return Ok(new { message = $"Status do chamado {id} atualizado para {statusDto.NovoStatus}." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}