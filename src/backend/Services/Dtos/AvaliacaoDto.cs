using System.ComponentModel.DataAnnotations;

namespace CajuAjuda.Backend.Services.Dtos;

public class AvaliacaoDto
{
    [Required(ErrorMessage = "A nota é obrigatória.")]
    [Range(1, 5, ErrorMessage = "A nota deve ser um valor entre 1 e 5.")]
    public int Nota { get; set; }

    public string? Comentario { get; set; }
}