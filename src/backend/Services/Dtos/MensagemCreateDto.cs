using System.ComponentModel.DataAnnotations;

namespace CajuAjuda.Backend.Services.Dtos;

public class MensagemCreateDto
{
    [Required(ErrorMessage = "O texto da mensagem é obrigatório.")]
    public string Texto { get; set; } = string.Empty;
}