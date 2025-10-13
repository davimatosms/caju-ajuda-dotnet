using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using CajuAjuda.Backend.Exceptions;

namespace CajuAjuda.Backend.Controllers
{
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

        [HttpGet]
        [Authorize(Roles = "TECNICO, ADMIN")]
        public async Task<IActionResult> GetAllChamados([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] StatusChamado? status = null, [FromQuery] PrioridadeChamado? prioridade = null)
        {
            var chamadosPaginados = await _chamadoService.GetAllChamadosAsync(pageNumber, pageSize, status, prioridade);
            return Ok(chamadosPaginados);
        }

      
        [HttpGet("atribuidos")]
        [Authorize(Roles = "TECNICO, ADMIN")]
        public async Task<IActionResult> GetMeusChamadosAtribuidos()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return Unauthorized();

            var chamados = await _chamadoService.GetChamadosAtribuidosAsync(userEmail);
            return Ok(chamados);
        }

        [HttpGet("disponiveis")]
        [Authorize(Roles = "TECNICO, ADMIN")]
        public async Task<IActionResult> GetChamadosDisponiveis()
        {
            var chamados = await _chamadoService.GetChamadosDisponiveisAsync();
            return Ok(chamados);
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

        public class UpdateStatusDto
        {
            public StatusChamado Status { get; set; }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "TECNICO, ADMIN")]
        public async Task<IActionResult> UpdateChamadoStatus(long id, [FromBody] UpdateStatusDto updateStatusDto)
        {
            await _chamadoService.UpdateChamadoStatusAsync(id, updateStatusDto.Status);
            return NoContent();
        }

        [HttpPatch("{id}/assign")]
        [Authorize(Roles = "TECNICO, ADMIN")]
        public async Task<IActionResult> AssignChamado(long id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return Unauthorized();

            try
            {
                await _chamadoService.AssignChamadoAsync(id, userEmail);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/mensagens")]
        [Authorize(Roles = "TECNICO, ADMIN, CLIENTE")]
        public async Task<IActionResult> AddMensagem(long id, [FromBody] MensagemCreateDto mensagemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userEmail == null || userRole == null) return Unauthorized();
            var novaMensagem = await _mensagemService.AddMensagemAsync(id, mensagemDto, userEmail, userRole);
            return Ok(novaMensagem);
        }

        [HttpPost("{id}/avaliar")]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> AvaliarChamado(long id, [FromBody] AvaliacaoDto avaliacaoDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return Unauthorized();
            await _chamadoService.AvaliarChamadoAsync(id, avaliacaoDto, userEmail);
            return Ok(new { message = "Avaliação registrada com sucesso." });
        }
    }
}