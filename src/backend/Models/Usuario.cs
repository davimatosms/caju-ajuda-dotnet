using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CajuAjuda.Backend.Models;

// Enum para os papéis do usuário
public enum Role
{
    CLIENTE,
    TECNICO,
    ADMIN
}

[Table("Usuarios")] // Define o nome da tabela no banco de dados
public class Usuario
{
    [Key] // Chave primária
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-incremento
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
    public Role Role { get; set; }

    public bool Enabled { get; set; } = true;

    public string? VerificationToken { get; set; }

    // Propriedade de navegação: um usuário (cliente) pode ter muitos chamados
    public virtual ICollection<Chamado> Chamados { get; set; } = new List<Chamado>();
}