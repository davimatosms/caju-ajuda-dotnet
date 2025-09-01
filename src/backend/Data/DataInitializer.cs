using CajuAjuda.Backend.Models;
using CajuAjuda.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Data;

public class DataInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public DataInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedDataAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CajuAjudaDbContext>();
        
        // Aplica quaisquer migrações pendentes para garantir que o BD esteja atualizado
        await context.Database.MigrateAsync();

        // Verifica se já existem usuários
        if (await context.Usuarios.AnyAsync())
        {
            return; // O banco de dados já foi populado
        }

        // Criação de usuários padrão
        var admin = new Usuario
        {
            Nome = "Admin Caju",
            Email = "admin@cajuajuda.com",
            Senha = BCrypt.Net.BCrypt.HashPassword("senha123"),
            Role = Role.ADMIN,
            Enabled = true
        };

        var tecnico = new Usuario
        {
            Nome = "Tecnico Caju",
            Email = "tecnico@cajuajuda.com",
            Senha = BCrypt.Net.BCrypt.HashPassword("senha123"),
            Role = Role.TECNICO,
            Enabled = true
        };
        
        var cliente = new Usuario
        {
            Nome = "Cliente Caju",
            Email = "cliente@cajuajuda.com",
            Senha = BCrypt.Net.BCrypt.HashPassword("senha123"),
            Role = Role.CLIENTE,
            Enabled = true
        };

        await context.Usuarios.AddRangeAsync(admin, tecnico, cliente);
        await context.SaveChangesAsync();
    }
}