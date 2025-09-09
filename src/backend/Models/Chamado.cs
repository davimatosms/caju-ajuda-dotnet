using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajuAjuda.Backend.Models;

public enum StatusChamado
{
    ABERTO,
    EM_ANDAMENTO,
    AGUARDANDO_CLIENTE,
    FECHADO,
    CANCELADO,
    MESCLADO 
}

public enum PrioridadeChamado
{
    BAIXA,
    MEDIA,
    ALTA,
    URGENTE 
}

[Table("Chamados")]
public class Chamado
{
    [Key]
    public long Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public StatusChamado Status { get; set; }

    [Required]
    public PrioridadeChamado Prioridade { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataFechamento { get; set; }
    
    public int? NotaAvaliacao { get; set; }
    
    public string? ComentarioAvaliacao { get; set; }
    
    // --- CAMPOS PARA MESCLAGEM ---
    public long? ChamadoPrincipalId { get; set; } 
    [ForeignKey("ChamadoPrincipalId")]
    public virtual Chamado? ChamadoPrincipal { get; set; }
    
    // Relações...
    public long ClienteId { get; set; }
    [ForeignKey("ClienteId")]
    public virtual Usuario Cliente { get; set; } = null!;
    
    public long? TecnicoResponsavelId { get; set; }
    [ForeignKey("TecnicoResponsavelId")]
    public virtual Usuario? TecnicoResponsavel { get; set; }
    
    // Propriedades de navegação
    public virtual ICollection<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
    public virtual ICollection<Anexo> Anexos { get; set; } = new List<Anexo>();
}