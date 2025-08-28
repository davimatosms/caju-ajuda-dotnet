using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CajuAjuda.Backend.Controllers;

[ApiController] // Marca esta classe como um Controller de API
[Route("api/[controller]")] // Define a rota base como "api/auth"
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    // O IUsuarioService é injetado aqui
    public AuthController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("register/cliente")] // Rota: POST api/auth/register/cliente
    public async Task<IActionResult> RegisterCliente([FromBody] UsuarioCreateDto usuarioDto)
    {
        // Validação automática do DTO
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var novoUsuario = await _usuarioService.RegisterClienteAsync(usuarioDto);

            // Cria um DTO de resposta para não expor a senha
            var responseDto = new UsuarioResponseDto
            {
                Id = novoUsuario.Id,
                Nome = novoUsuario.Nome,
                Email = novoUsuario.Email
            };

            // Retorna o status 201 Created com os dados do usuário criado
            return CreatedAtAction(null, new { id = responseDto.Id }, responseDto);
        }
        catch (Exception ex)
        {
            // Se o serviço lançar uma exceção (ex: e-mail duplicado), retorna um erro 400
            return BadRequest(new { message = ex.Message });
        }
    }
}