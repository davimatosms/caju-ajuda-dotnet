using System.ComponentModel.DataAnnotations;
using CajuAjuda.Backend.Models;

namespace CajuAjuda.Backend.Services.Dtos;

public class ChamadoCreateDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150, ErrorMessage = "O título deve ter no máximo 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    public string Descricao { get; set; } = string.Empty;

    // A prioridade será definida pela IA no backend, então não é obrigatória aqui.
    public PrioridadeChamado Prioridade { get; set; } = PrioridadeChamado.BAIXA;
}