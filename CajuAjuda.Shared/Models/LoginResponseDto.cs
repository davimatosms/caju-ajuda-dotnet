namespace CajuAjuda.Shared.Models;

/// <summary>
/// Representa a resposta enviada pela API após um login bem-sucedido.
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}