using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChamadosController : ControllerBase
{
    private readonly IChamadoService _chamadoService;
    private readonly IMensagemService _mensagemService;

    public ChamadosController(IChamadoService chamadoService, IMensagemService mensagemService)
    {
        _chamadoService = chamadoService;
        _mensagemService = mensagemService;
    }

    [HttpPost]
    [Authorize(Roles = "CLIENTE")]
    public async Task<IActionResult> CreateChamado([FromBody] ChamadoCreateDto chamadoDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null) return Unauthorized();
        var novoChamado = await _chamadoService.CreateAsync(chamadoDto, userEmail);
        return CreatedAtAction(null, new { id = novoChamado.Id }, novoChamado);
    }

    [HttpGet("meus")]
    [Authorize(Roles = "CLIENTE")]
    public async Task<IActionResult> GetMeusChamados()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null) return Unauthorized();
        var chamados = await _chamadoService.GetChamadosByClienteEmailAsync(userEmail);
        return Ok(chamados);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChamadoById(long id)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        if (userEmail == null || userRole == null) return Unauthorized();
        var chamado = await _chamadoService.GetChamadoByIdAsync(id, userEmail, userRole);
        return Ok(chamado);
    }

    [HttpPost("{id}/mensagens")]
    [Authorize(Roles = "CLIENTE")]
    public async Task<IActionResult> AddMensagemCliente(long id, [FromBody] MensagemCreateDto mensagemDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        if (userEmail == null || userRole == null) return Unauthorized();
        var novaMensagem = await _mensagemService.AddMensagemAsync(id, mensagemDto, userEmail, userRole);
        return Ok(novaMensagem);
    }

    [HttpPost("{id}/anexos")]
    [Authorize(Roles = "CLIENTE")]
    public async Task<IActionResult> UploadAnexo(long id, IFormFile file)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null) return Unauthorized();
        if (file == null || file.Length == 0) return BadRequest(new { message = "Nenhum arquivo foi enviado." });
        var anexo = await _chamadoService.AddAnexoAsync(id, file, userEmail);
        return Ok(anexo);
    }
    
    [HttpPost("{id}/avaliar")]
    [Authorize(Roles = "CLIENTE")]
    public async Task<IActionResult> AvaliarChamado(long id, [FromBody] AvaliacaoDto avaliacaoDto)
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
        await _chamadoService.AvaliarChamadoAsync(id, avaliacaoDto, userEmail);
        return Ok(new { message = "Avaliação registrada com sucesso." });
    }
}