using System.ComponentModel.DataAnnotations;

namespace CajuAjuda.Backend.Services.Dtos;

public class SenhaUpdateDto
{
    [Required(ErrorMessage = "A senha atual é obrigatória.")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A nova senha deve ter no mínimo 6 caracteres.")]
    public string NovaSenha { get; set; } = string.Empty;
}