using System.ComponentModel.DataAnnotations;

namespace CajuAjuda.Backend.Services.Dtos;

public class ChamadoMergeDto
{
    [Required(ErrorMessage = "O ID do chamado principal é obrigatório.")]
    public long ChamadoPrincipalId { get; set; }
}