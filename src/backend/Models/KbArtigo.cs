using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajuAjuda.Backend.Models;

[Table("KbArtigos")]
public class KbArtigo
{
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Conteudo { get; set; } = string.Empty; // Conteúdo em Markdown ou HTML

    // Relação com a Categoria
    public long CategoriaId { get; set; }
    [ForeignKey("CategoriaId")]
    public virtual KbCategoria Categoria { get; set; } = null!;
}