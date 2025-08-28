using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajuAjuda.Backend.Models;

[Table("Mensagens")]
public class Mensagem
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Texto { get; set; } = string.Empty;

    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    // Chave Estrangeira para o Autor da mensagem
    public long AutorId { get; set; }

    [ForeignKey("AutorId")]
    public virtual Usuario Autor { get; set; } = null!;
    
    // Chave Estrangeira para o Chamado
    public long ChamadoId { get; set; }

    [ForeignKey("ChamadoId")]
    public virtual Chamado Chamado { get; set; } = null!;
}