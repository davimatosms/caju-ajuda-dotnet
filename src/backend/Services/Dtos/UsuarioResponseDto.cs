namespace CajuAjuda.Backend.Services.Dtos;

// DTO para retornar dados seguros de um usuário criado
public class UsuarioResponseDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}