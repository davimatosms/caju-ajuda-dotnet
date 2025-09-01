using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Protege TODOS os endpoints neste controller por padrão
public class ChamadosController : ControllerBase
{
    private readonly IChamadoService _chamadoService;
    private readonly IMensagemService _mensagemService;

    public ChamadosController(IChamadoService chamadoService, IMensagemService mensagemService)
    {
        _chamadoService = chamadoService;
        _mensagemService = mensagemService;
    }

    [HttpPost] // Rota: POST api/chamados
    [Authorize(Roles = "CLIENTE")] // Apenas usuários com a role CLIENTE podem acessar
    public async Task<IActionResult> CreateChamado([FromBody] ChamadoCreateDto chamadoDto)
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

        try
        {
            var novoChamado = await _chamadoService.CreateAsync(chamadoDto, userEmail);
            return CreatedAtAction(null, new { id = novoChamado.Id }, novoChamado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("meus")] // Rota: GET api/chamados/meus
    [Authorize(Roles = "CLIENTE")]
    public async Task<IActionResult> GetMeusChamados()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null)
        {
            return Unauthorized();
        }

        try
        {
            var chamados = await _chamadoService.GetChamadosByClienteEmailAsync(userEmail);
            return Ok(chamados);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("{id}")] // Rota: GET api/chamados/123
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
        catch (UnauthorizedAccessException)
        {
            return Forbid(); // Retorna 403 Forbidden se o serviço negar o acesso
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro interno.", error = ex.Message });
        }
    }

    [HttpPost("{id}/mensagens")] // Rota: POST api/chamados/123/mensagens
    [Authorize(Roles = "CLIENTE")]
    public async Task<IActionResult> AddMensagemCliente(long id, [FromBody] MensagemCreateDto mensagemDto)
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
            // Idealmente, retornaríamos um MensagemResponseDto
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
}