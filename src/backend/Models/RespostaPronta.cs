using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajuAjuda.Backend.Models;

[Table("RespostasProntas")]
public class RespostaPronta
{
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Corpo { get; set; } = string.Empty; // O texto do template
}