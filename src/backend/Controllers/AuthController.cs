using CajuAjuda.Backend.Services;
using CajuAjuda.Backend.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CajuAjuda.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ITokenService _tokenService;

    public AuthController(IUsuarioService usuarioService, ITokenService tokenService)
    {
        _usuarioService = usuarioService;
        _tokenService = tokenService;
    }

    [HttpPost("register/cliente")]
    public async Task<IActionResult> RegisterCliente([FromBody] UsuarioCreateDto usuarioDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var novoUsuario = await _usuarioService.RegisterClienteAsync(usuarioDto);

        return CreatedAtAction(null, new { id = novoUsuario.Id }, new { message = "Registo bem-sucedido. Por favor, verifique o seu e-mail para ativar a conta." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _usuarioService.AuthenticateAsync(loginDto);

        if (user == null)
        {
            return Unauthorized("E-mail ou senha inválidos, ou a conta não foi verificada.");
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(new { token });
    }

    [HttpGet("verify")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        var success = await _usuarioService.VerifyEmailAsync(token);
        if (success)
        {
            // Idealmente, redirecionaria para uma página de sucesso no frontend.
            // Por agora, retornamos uma mensagem.
            return Ok("<h1>E-mail verificado com sucesso!</h1><p>Você já pode fechar esta aba e fazer o login na aplicação.</p>");
        }
        return BadRequest("<h1>Erro na Verificação</h1><p>Token inválido ou expirado. Por favor, tente se registar novamente.</p>");
    }
}