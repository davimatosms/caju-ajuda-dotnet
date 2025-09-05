using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajuAjuda.Backend.Models;

[Table("KbCategorias")]
public class KbCategoria
{
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    public virtual ICollection<KbArtigo> Artigos { get; set; } = new List<KbArtigo>();
}