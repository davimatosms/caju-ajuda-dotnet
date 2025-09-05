using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services.Dtos;

public class ChamadoListResponseDto
{
    public long Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public StatusChamado Status { get; set; }
    public PrioridadeChamado Prioridade { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool HasUnreadMessages { get; set; } 
}