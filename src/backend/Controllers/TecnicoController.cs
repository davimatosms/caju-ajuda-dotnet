using CajuAjuda.Backend.Helpers;
using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "TECNICO,ADMIN")]
public class TecnicoController : ControllerBase
{
    private readonly IChamadoService _chamadoService;
    private readonly IMensagemService _mensagemService;

    public TecnicoController(IChamadoService chamadoService, IMensagemService mensagemService)
    {
        _chamadoService = chamadoService;
        _mensagemService = mensagemService;
    }

    [HttpGet("chamados")]
    public async Task<IActionResult> GetAllChamados([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] StatusChamado? status = null, [FromQuery] PrioridadeChamado? prioridade = null)
    {
        try
        {
            var pagedResult = await _chamadoService.GetAllChamadosAsync(pageNumber, pageSize, status, prioridade);

            var paginationMetadata = new
            {
                pagedResult.TotalCount,
                pagedResult.PageSize,
                pagedResult.CurrentPage,
                pagedResult.TotalPages,
                pagedResult.HasNext,
                pagedResult.HasPrevious
            };
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(pagedResult.Items);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro interno ao buscar os chamados.", error = ex.Message });
        }
    }
    
    [HttpGet("chamados/{id}")]
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

    [HttpPost("chamados/{id}/mensagens")]
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
    
    [HttpPatch("chamados/{id}/status")]
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