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
        private readonly ILogger<ChamadosController> _logger;

        public ChamadosController(IChamadoService chamadoService, IMensagemService mensagemService, ILogger<ChamadosController> logger)
        {
            _chamadoService = chamadoService;
            _mensagemService = mensagemService;
            _logger = logger;
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
            
            if (chamado != null)
            {
                _logger.LogInformation("🔍 [ChamadosController] Retornando chamado ID {ChamadoId} com {MensagensCount} mensagens", 
                    chamado.Id, chamado.Mensagens?.Count ?? 0);
            }
            
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

        // NOVO: Rota POST para atribuir chamado (compatível com Desktop)
        [HttpPost("{id}/atribuir")]
        [Authorize(Roles = "TECNICO, ADMIN")]
        public async Task<IActionResult> AtribuirChamado(long id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return Unauthorized();

            try
            {
                await _chamadoService.AssignChamadoAsync(id, userEmail);
                return Ok(new { message = "Chamado atribuído com sucesso!" });
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

        // MANTÉM a rota PATCH também (para compatibilidade)
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

        [HttpPost("{id}/anexos")]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> AddAnexo(long id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Nenhum arquivo foi enviado." });

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return Unauthorized();

            try
            {
                var anexo = await _chamadoService.AddAnexoAsync(id, file, userEmail);
                return Ok(new 
                { 
                    id = anexo.Id,
                    nomeArquivo = anexo.NomeArquivo,
                    tipoArquivo = anexo.TipoArquivo,
                    message = "Anexo enviado com sucesso."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/anexos")]
        public async Task<IActionResult> GetAnexosByChamado(long id)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userEmail == null || userRole == null) return Unauthorized();

            try
            {
                var anexos = await _chamadoService.GetAnexosByChamadoIdAsync(id, userEmail, userRole);
                var anexosDto = anexos.Select(a => new
                {
                    id = a.Id,
                    nomeArquivo = a.NomeArquivo,
                    tipoArquivo = a.TipoArquivo,
                    chamadoId = a.ChamadoId
                });
                return Ok(anexosDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("anexos/{anexoId}/download")]
        public async Task<IActionResult> DownloadAnexo(long anexoId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userEmail == null || userRole == null) return Unauthorized();

            try
            {
                var anexo = await _chamadoService.GetAnexoByIdAsync(anexoId, userEmail, userRole);
                if (anexo == null) return NotFound(new { message = "Anexo não encontrado." });

                var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                var storagePath = configuration.GetValue<string>("FileStorage:LocalStoragePath") ?? "caju_uploads";
                var filePath = Path.Combine(storagePath, anexo.NomeUnico);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "Arquivo não encontrado no servidor." });
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, anexo.TipoArquivo, anexo.NomeArquivo);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
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
