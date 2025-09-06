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
    
    [HttpGet("chamados/{id}")]
    public async Task<IActionResult> GetChamadoById(long id)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (userEmail == null || userRole == null)
        {
            return Unauthorized();
        }
        
        var chamado = await _chamadoService.GetChamadoByIdAsync(id, userEmail, userRole);
        return Ok(chamado);
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

        var novaMensagem = await _mensagemService.AddMensagemAsync(id, mensagemDto, userEmail, userRole);
        return Ok(novaMensagem);
    }
    
    [HttpPatch("chamados/{id}/status")]
    public async Task<IActionResult> UpdateChamadoStatus(long id, [FromBody] ChamadoUpdateStatusDto statusDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _chamadoService.UpdateChamadoStatusAsync(id, statusDto.NovoStatus);
        return Ok(new { message = $"Status do chamado {id} atualizado para {statusDto.NovoStatus}." });
    }

    [HttpPost("chamados/{id}/atribuir")]
    public async Task<IActionResult> AssignChamado(long id)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null)
        {
            return Unauthorized();
        }
        
        await _chamadoService.AssignChamadoAsync(id, userEmail);
        return Ok(new { message = $"Chamado {id} atribuído ao técnico {userEmail} com sucesso." });
    }
    
    [HttpPost("chamados/{id}/notas")]
    public async Task<IActionResult> AddNotaInterna(long id, [FromBody] MensagemCreateDto notaDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null)
        {
            return Unauthorized();
        }

        var novaNota = await _mensagemService.AddNotaInternaAsync(id, notaDto, userEmail);
        return Ok(novaNota);
    }
    
    [HttpPost("chamados/{id}/mesclar")]
    public async Task<IActionResult> MergeChamado(long id, [FromBody] ChamadoMergeDto mergeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null)
        {
            return Unauthorized();
        }
        
        await _chamadoService.MergeChamadosAsync(id, mergeDto.ChamadoPrincipalId, userEmail);
        return Ok(new { message = $"Chamado #{id} mesclado com sucesso no chamado #{mergeDto.ChamadoPrincipalId}." });
    }
}