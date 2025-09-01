using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Data;

public class CajuAjudaDbContext : DbContext
{
    public CajuAjudaDbContext(DbContextOptions<CajuAjudaDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Chamado> Chamados { get; set; }
    public DbSet<Mensagem> Mensagens { get; set; }
    public DbSet<Anexo> Anexos { get; set; }

   
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
            .WithMany() // Relação sem coleção de volta em Usuario
            .HasForeignKey(m => m.AutorId)
            .OnDelete(DeleteBehavior.Cascade); 
            
        modelBuilder.Entity<Chamado>()
            .HasOne(c => c.Cliente)
            .WithMany(u => u.Chamados)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}