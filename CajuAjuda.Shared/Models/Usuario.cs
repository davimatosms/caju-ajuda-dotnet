using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajuAjuda.Shared.Models;

// A DEFINIÇÃO DO ENUM ROLE FOI REMOVIDA DAQUI

[Table("Usuarios")]
public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; } // Agora usa o Enum do arquivo Role.cs

    public bool Enabled { get; set; } = true;

    public string? VerificationToken { get; set; }

    public virtual ICollection<Chamado> Chamados { get; set; } = new List<Chamado>();
}