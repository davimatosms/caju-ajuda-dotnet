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

    public bool LidoPeloCliente { get; set; } = false;
    
    public bool IsNotaInterna { get; set; } = false; 

    // Chaves Estrangeiras...
    public long AutorId { get; set; }
    [ForeignKey("AutorId")]
    public virtual Usuario Autor { get; set; } = null!;
    
    public long ChamadoId { get; set; }
    [ForeignKey("ChamadoId")]
    public virtual Chamado Chamado { get; set; } = null!;
}