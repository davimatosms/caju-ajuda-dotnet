namespace CajuAjuda.Backend.Services.Dtos;

public class AnexoResponseDto
{
    public long Id { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string NomeUnico { get; set; } = string.Empty;
    public string TipoArquivo { get; set; } = string.Empty;
    public long ChamadoId { get; set; }
}

