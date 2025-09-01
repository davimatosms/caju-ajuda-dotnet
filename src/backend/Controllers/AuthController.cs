using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")] // Define a rota base como "api/auth"
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ITokenService _tokenService;

    public AuthController(IUsuarioService usuarioService, ITokenService tokenService)
    {
        _usuarioService = usuarioService;
        _tokenService = tokenService;
    }

    [HttpPost("register/cliente")] // Rota: POST api/auth/register/cliente
    public async Task<IActionResult> RegisterCliente([FromBody] UsuarioCreateDto usuarioDto)
    {
        // A validação do DTO (usando os atributos [Required], etc.) é feita automaticamente pelo ASP.NET
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var novoUsuario = await _usuarioService.RegisterClienteAsync(usuarioDto);

            // Cria um DTO de resposta para não expor dados sensíveis como a senha
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
            // Se o serviço lançar uma exceção (ex: e-mail duplicado), retorna um erro 400 Bad Request
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")] // Rota: POST api/auth/login
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _usuarioService.AuthenticateAsync(loginDto);

        if (user == null)
        {
            return Unauthorized("E-mail ou senha inválidos.");
        }

        var token = _tokenService.GenerateToken(user);

        return Ok(new { token });
    }
}