namespace CajuAjuda.Backend.Services.Dtos;

public class MensagemResponseDto
{
    public long Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; }
    public string AutorNome { get; set; } = string.Empty;
    public long AutorId { get; set; }
}