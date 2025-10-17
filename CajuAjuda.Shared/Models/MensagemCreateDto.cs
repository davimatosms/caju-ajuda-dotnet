using System.ComponentModel.DataAnnotations;

namespace CajuAjuda.Shared.Models;

public class MensagemCreateDto
{
    [Required(ErrorMessage = "O texto da mensagem é obrigatório.")]
    public string Texto { get; set; } = string.Empty;
}