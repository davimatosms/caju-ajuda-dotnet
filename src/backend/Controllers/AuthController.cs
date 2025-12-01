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
        Console.WriteLine($"[VERIFY] Token recebido: '{token}'");
        Console.WriteLine($"[VERIFY] Token length: {token?.Length}");
        
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"[VERIFY] ❌ Token vazio ou nulo");
            return BadRequest(new { message = "Token não fornecido." });
        }
        
        var success = await _usuarioService.VerifyEmailAsync(token);
        if (success)
        {
            Console.WriteLine($"[VERIFY] ✅ Retornando OK para o frontend");
            return Ok(new { message = "E-mail verificado com sucesso! Você já pode fazer login na aplicação." });
        }
        
        Console.WriteLine($"[VERIFY] ❌ Retornando BadRequest para o frontend");
        return BadRequest(new { message = "Token inválido ou expirado. Por favor, tente se registrar novamente." });
    }

    // ENDPOINT TEMPORÁRIO PARA ATIVAR CONTA MANUALMENTE (DESENVOLVIMENTO)
    [HttpPost("activate-by-email")]
    public async Task<IActionResult> ActivateAccountByEmail([FromBody] ActivateAccountDto dto)
    {
        var success = await _usuarioService.ActivateAccountByEmailAsync(dto.Email);
        if (success)
        {
            return Ok(new { message = "Conta ativada com sucesso! Você já pode fazer login." });
        }
        return BadRequest(new { message = "Usuário não encontrado ou já está ativo." });
    }
}

public record ActivateAccountDto(string Email);