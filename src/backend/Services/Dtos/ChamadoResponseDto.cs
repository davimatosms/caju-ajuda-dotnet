using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services.Dtos;

public class ChamadoResponseDto
{
    public long Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty; // Incluímos o nome do cliente
    public StatusChamado Status { get; set; }
    public PrioridadeChamado Prioridade { get; set; }
    public DateTime DataCriacao { get; set; }
}