using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajuAjuda.Backend.Models;

[Table("Anexos")]
public class Anexo
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string NomeArquivo { get; set; } = string.Empty;

    [Required]
    public string NomeUnico { get; set; } = string.Empty; // Nome do arquivo no storage

    [Required]
    public string TipoArquivo { get; set; } = string.Empty;

    // Chave Estrangeira para o Chamado
    public long ChamadoId { get; set; }

    [ForeignKey("ChamadoId")]
    public virtual Chamado Chamado { get; set; } = null!;
}