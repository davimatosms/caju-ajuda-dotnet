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
    [Authorize]
    public class PerfilController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public PerfilController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPerfil()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return Unauthorized();

            var perfil = await _usuarioService.GetPerfilAsync(userEmail);
            return Ok(perfil);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePerfil([FromBody] PerfilUpdateDto perfilDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return Unauthorized();

            // CORREÇÃO APLICADA AQUI: O nome do método foi corrigido.
            await _usuarioService.UpdatePerfilAsync(userEmail, perfilDto);
            return NoContent();
        }

        [HttpPatch("update-senha")]
        public async Task<IActionResult> UpdateMinhaSenha([FromBody] SenhaUpdateDto senhaDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) return Unauthorized();

            try
            {
                await _usuarioService.UpdateSenhaAsync(userEmail, senhaDto);
                return NoContent();
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}