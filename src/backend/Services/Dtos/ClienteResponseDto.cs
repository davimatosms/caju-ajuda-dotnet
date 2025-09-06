namespace CajuAjuda.Backend.Services.Dtos;

public class ClienteResponseDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}