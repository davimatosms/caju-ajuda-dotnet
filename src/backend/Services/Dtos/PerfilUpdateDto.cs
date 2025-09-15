using System.ComponentModel.DataAnnotations;

namespace CajuAjuda.Backend.Services.Dtos;

public class PerfilUpdateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}