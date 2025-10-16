using CajuAjuda.Backend.Exceptions;
using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CajuAjuda.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todos os endpoints neste controller
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Endpoint para o técnico alterar a própria senha
        [HttpPost("update-senha")]
        public async Task<IActionResult> UpdateSenha([FromBody] SenhaUpdateDto senhaDto)
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
                await _usuarioService.UpdateSenhaAsync(userEmail, senhaDto);
                return Ok(new { message = "Senha alterada com sucesso." });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}