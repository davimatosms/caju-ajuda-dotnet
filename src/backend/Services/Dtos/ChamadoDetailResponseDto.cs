using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services.Dtos;

public class ChamadoDetailResponseDto
{
    public long Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public string? NomeTecnicoResponsavel { get; set; }
    public StatusChamado Status { get; set; }
    public PrioridadeChamado Prioridade { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataFechamento { get; set; }
    public int? NotaAvaliacao { get; set; }
    public string? ComentarioAvaliacao { get; set; }
    public string? SugestaoIA { get; set; }
    public List<MensagemResponseDto> Mensagens { get; set; } = new();
}