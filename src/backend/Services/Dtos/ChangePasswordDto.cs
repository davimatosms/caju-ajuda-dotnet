using System.ComponentModel.DataAnnotations;

namespace CajuAjuda.Backend.Services.Dtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "A senha antiga é obrigatória.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A nova senha deve ter no mínimo 6 caracteres.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação da nova senha é obrigatória.")]
        [Compare(nameof(NewPassword), ErrorMessage = "A nova senha e a confirmação não correspondem.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}