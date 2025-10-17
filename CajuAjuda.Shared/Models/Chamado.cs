using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajuAjuda.Shared.Models;

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
    public long? ChamadoPrincipalId { get; set; }
    [ForeignKey("ChamadoPrincipalId")]
    public virtual Chamado? ChamadoPrincipal { get; set; }

    public long ClienteId { get; set; }
    [ForeignKey("ClienteId")]
    public virtual Usuario Cliente { get; set; } = null!;

    // PROPRIEDADE ADICIONADA PARA O BINDING DO XAML
    [NotMapped]
    public string NomeCliente => Cliente?.Nome ?? "Cliente não encontrado";

    public long? TecnicoResponsavelId { get; set; }
    [ForeignKey("TecnicoResponsavelId")]
    public virtual Usuario? TecnicoResponsavel { get; set; }

    public virtual ICollection<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
    public virtual ICollection<Anexo> Anexos { get; set; } = new List<Anexo>();
}