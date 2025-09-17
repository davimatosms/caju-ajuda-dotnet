using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Data;

public class CajuAjudaDbContext : DbContext
{
    // Adicione um construtor sem parâmetros para o Moq
    public CajuAjudaDbContext() {}
    
    public CajuAjudaDbContext(DbContextOptions<CajuAjudaDbContext> options) : base(options)
    {
    }

    // Adicione a palavra-chave 'virtual' a todos os DbSets
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Chamado> Chamados { get; set; }
    public virtual DbSet<Mensagem> Mensagens { get; set; }
    public virtual DbSet<Anexo> Anexos { get; set; }
    public virtual DbSet<RespostaPronta> RespostasProntas { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração para evitar o ciclo de exclusão em cascata
        modelBuilder.Entity<Mensagem>()
            .HasOne(m => m.Chamado)
            .WithMany(c => c.Mensagens)
            .HasForeignKey(m => m.ChamadoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Mensagem>()
            .HasOne(m => m.Autor)
            .WithMany()
            .HasForeignKey(m => m.AutorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Chamado>()
            .HasOne(c => c.Cliente)
            .WithMany(u => u.Chamados)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}