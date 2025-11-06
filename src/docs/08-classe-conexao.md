# 8. CLASSE DE CONEXÃO COM O BANCO DE DADOS

## 8.1 INTRODUÇÃO

A classe `CajuAjudaDbContext` é a ponte entre a aplicação .NET e o banco de dados SQL Server. Ela herda de `DbContext` do Entity Framework Core e é responsável por:

- 🔗 Gerenciar a conexão com o banco de dados
- 📊 Mapear as entidades C# para tabelas SQL
- 🔄 Rastrear mudanças nas entidades (Change Tracking)
- 💾 Executar operações de CRUD através de LINQ
- ⚙️ Configurar relacionamentos e constraints via Fluent API

---

## 8.2 CÓDIGO COMPLETO DA CLASSE

### **Arquivo: `backend/Data/CajuAjudaDbContext.cs`**

```csharp
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Data;

/// <summary>
/// Contexto do Entity Framework Core para o banco de dados CajuAjuda.
/// Gerencia as entidades e o mapeamento objeto-relacional (ORM).
/// </summary>
public class CajuAjudaDbContext : DbContext
{
    // ========================================================================
    // CONSTRUTORES
    // ========================================================================

    /// <summary>
    /// Construtor padrão sem parâmetros.
    /// Necessário para ferramentas de teste (Moq, InMemoryDatabase).
    /// </summary>
    public CajuAjudaDbContext() { }

    /// <summary>
    /// Construtor que recebe as opções de configuração do DbContext.
    /// Usado pela injeção de dependência no ASP.NET Core.
    /// </summary>
    /// <param name="options">Configurações do contexto (connection string, provider, etc.)</param>
    public CajuAjudaDbContext(DbContextOptions<CajuAjudaDbContext> options) : base(options)
    {
    }

    // ========================================================================
    // DBSETS (COLEÇÕES DE ENTIDADES)
    // ========================================================================

    /// <summary>
    /// DbSet de Usuários (Clientes, Técnicos, Admins).
    /// Mapeia para a tabela 'Usuarios' no banco de dados.
    /// </summary>
    public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

    /// <summary>
    /// DbSet de Chamados (Tickets de suporte).
    /// Mapeia para a tabela 'Chamados' no banco de dados.
    /// </summary>
    public virtual DbSet<Chamado> Chamados { get; set; } = null!;

    /// <summary>
    /// DbSet de Mensagens (Chat dos chamados).
    /// Mapeia para a tabela 'Mensagens' no banco de dados.
    /// </summary>
    public virtual DbSet<Mensagem> Mensagens { get; set; } = null!;

    /// <summary>
    /// DbSet de Anexos (Metadados de arquivos).
    /// Mapeia para a tabela 'Anexos' no banco de dados.
    /// </summary>
    public virtual DbSet<Anexo> Anexos { get; set; } = null!;

    /// <summary>
    /// DbSet de Respostas Prontas (Templates de mensagens).
    /// Mapeia para a tabela 'RespostasProntas' no banco de dados.
    /// </summary>
    public virtual DbSet<RespostaPronta> RespostasProntas { get; set; } = null!;

    // ========================================================================
    // CONFIGURAÇÃO DO MODELO (FLUENT API)
    // ========================================================================

    /// <summary>
    /// Configura os relacionamentos e constraints das entidades.
    /// Chamado automaticamente pelo EF Core ao criar o modelo.
    /// </summary>
    /// <param name="modelBuilder">Construtor do modelo de dados</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ====================================================================
        // CONFIGURAÇÃO: Mensagem → Chamado (N:1)
        // ====================================================================
        // Evita ciclo de exclusão em cascata quando Chamado é excluído.
        // Usa Restrict para que mensagens sejam tratadas manualmente.
        modelBuilder.Entity<Mensagem>()
            .HasOne(m => m.Chamado)              // Uma mensagem tem um chamado
            .WithMany(c => c.Mensagens)          // Um chamado tem muitas mensagens
            .HasForeignKey(m => m.ChamadoId)     // FK: ChamadoId
            .OnDelete(DeleteBehavior.Restrict);   // Não excluir mensagens automaticamente

        // ====================================================================
        // CONFIGURAÇÃO: Mensagem → Autor (N:1)
        // ====================================================================
        // Quando um usuário é excluído, suas mensagens também são excluídas.
        modelBuilder.Entity<Mensagem>()
            .HasOne(m => m.Autor)                // Uma mensagem tem um autor
            .WithMany()                          // Um usuário pode ter muitas mensagens
            .HasForeignKey(m => m.AutorId)       // FK: AutorId
            .OnDelete(DeleteBehavior.Cascade);   // Excluir mensagens se usuário for excluído

        // ====================================================================
        // CONFIGURAÇÃO: Chamado → Cliente (N:1)
        // ====================================================================
        // Quando um cliente é excluído, seus chamados também são excluídos.
        modelBuilder.Entity<Chamado>()
            .HasOne(c => c.Cliente)              // Um chamado tem um cliente
            .WithMany(u => u.Chamados)           // Um usuário pode ter muitos chamados
            .HasForeignKey(c => c.ClienteId)     // FK: ClienteId
            .OnDelete(DeleteBehavior.Cascade);   // Excluir chamados se cliente for excluído

        // ====================================================================
        // CONFIGURAÇÃO: Chamado → TecnicoResponsavel (N:1 opcional)
        // ====================================================================
        // Um chamado pode não ter técnico atribuído (TecnicoResponsavelId = null).
        modelBuilder.Entity<Chamado>()
            .HasOne(c => c.TecnicoResponsavel)   // Um chamado pode ter um técnico
            .WithMany()                          // Um técnico pode atender muitos chamados
            .HasForeignKey(c => c.TecnicoResponsavelId)
            .OnDelete(DeleteBehavior.SetNull);   // Se técnico for excluído, setar null

        // ====================================================================
        // CONFIGURAÇÃO: Anexo → Chamado (N:1)
        // ====================================================================
        // Quando um chamado é excluído, seus anexos também são excluídos.
        modelBuilder.Entity<Anexo>()
            .HasOne(a => a.Chamado)              // Um anexo pertence a um chamado
            .WithMany(c => c.Anexos)             // Um chamado pode ter muitos anexos
            .HasForeignKey(a => a.ChamadoId)     // FK: ChamadoId
            .OnDelete(DeleteBehavior.Cascade);   // Excluir anexos se chamado for excluído

        // ====================================================================
        // CONFIGURAÇÃO: Índices Únicos
        // ====================================================================
        // Garante que não existam dois usuários com o mesmo email
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // ====================================================================
        // CONFIGURAÇÃO: Valores Padrão
        // ====================================================================
        // Define valores padrão para campos ao criar novos registros
        modelBuilder.Entity<Usuario>()
            .Property(u => u.DataCriacao)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Ativo)
            .HasDefaultValue(true);

        modelBuilder.Entity<Chamado>()
            .Property(c => c.DataCriacao)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Chamado>()
            .Property(c => c.DataAtualizacao)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Mensagem>()
            .Property(m => m.DataEnvio)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<Mensagem>()
            .Property(m => m.LidoPeloCliente)
            .HasDefaultValue(false);

        modelBuilder.Entity<Mensagem>()
            .Property(m => m.IsNotaInterna)
            .HasDefaultValue(false);

        // ====================================================================
        // CONFIGURAÇÃO: Conversões de Enum
        // ====================================================================
        // Armazena enums como strings no banco de dados
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Chamado>()
            .Property(c => c.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Chamado>()
            .Property(c => c.Prioridade)
            .HasConversion<string>();
    }

    // ========================================================================
    // MÉTODOS SOBRESCRITOS
    // ========================================================================

    /// <summary>
    /// Intercepta o método SaveChanges para adicionar lógica personalizada.
    /// Atualiza automaticamente DataAtualizacao nos chamados.
    /// </summary>
    public override int SaveChanges()
    {
        AtualizarTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Versão assíncrona do SaveChanges com lógica personalizada.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Atualiza automaticamente o campo DataAtualizacao dos chamados modificados.
    /// </summary>
    private void AtualizarTimestamps()
    {
        var chamadosModificados = ChangeTracker.Entries<Chamado>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in chamadosModificados)
        {
            entry.Entity.DataAtualizacao = DateTime.Now;
        }
    }
}
```

---

## 8.3 REGISTRO DO CONTEXTO NO `Program.cs`

### **Arquivo: `backend/Program.cs`**

```csharp
using CajuAjuda.Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURAÇÃO DO ENTITY FRAMEWORK CORE
// ============================================================================

// Registrar o DbContext como serviço no container de DI
builder.Services.AddDbContext<CajuAjudaDbContext>(options =>
{
    // Obter a connection string do appsettings.json
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    // Configurar o SQL Server como provider
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        // Retry automático em caso de falha de conexão (resiliência)
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 3,                    // Máximo de 3 tentativas
            maxRetryDelay: TimeSpan.FromSeconds(5), // Esperar 5 segundos entre tentativas
            errorNumbersToAdd: null              // Erros que acionam retry (null = todos)
        );
        
        // Timeout de comando SQL (30 segundos)
        sqlServerOptions.CommandTimeout(30);
        
        // Usar paginação do lado do servidor (SQL Server 2012+)
        sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
    
    // Configurações adicionais do EF Core
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment()); // Logs detalhados em dev
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());       // Erros detalhados em dev
});

// Outros serviços...
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================================================================
// APLICAR MIGRATIONS AUTOMATICAMENTE NO STARTUP (OPCIONAL)
// ============================================================================

// Em produção, é recomendado aplicar migrations manualmente via CLI
if (builder.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<CajuAjudaDbContext>();
        
        // Aplicar migrations pendentes automaticamente
        dbContext.Database.Migrate();
        
        // Ou apenas verificar se o banco existe
        // dbContext.Database.EnsureCreated();
    }
}

app.Run();
```

---

## 8.4 STRING DE CONEXÃO

### **Arquivo: `backend/appsettings.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CajuAjudaDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning",
      "Microsoft.EntityFrameworkCore.Infrastructure": "Warning"
    }
  }
}
```

### **Arquivo: `backend/appsettings.Development.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CajuAjudaDB_Dev;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "Microsoft.EntityFrameworkCore.Infrastructure": "Information"
    }
  }
}
```

### **Arquivo: `backend/appsettings.Production.json` (Exemplo para Azure)**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:cajuajuda.database.windows.net,1433;Database=CajuAjudaDB;User ID=admin@cajuajuda;Password={senha};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  }
}
```

---

## 8.5 EXEMPLO DE USO EM UM SERVIÇO

### **Arquivo: `backend/Services/ChamadoService.cs`**

```csharp
using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CajuAjuda.Backend.Services;

public class ChamadoService
{
    private readonly CajuAjudaDbContext _context;

    // Injeção de dependência do DbContext
    public ChamadoService(CajuAjudaDbContext context)
    {
        _context = context;
    }

    // ========================================================================
    // EXEMPLO 1: Criar novo chamado
    // ========================================================================
    public async Task<Chamado> CriarChamadoAsync(Chamado chamado)
    {
        // Adicionar entidade ao contexto
        _context.Chamados.Add(chamado);
        
        // Salvar mudanças no banco
        await _context.SaveChangesAsync();
        
        return chamado;
    }

    // ========================================================================
    // EXEMPLO 2: Buscar chamado por ID com relacionamentos (Eager Loading)
    // ========================================================================
    public async Task<Chamado?> ObterChamadoPorIdAsync(int id)
    {
        return await _context.Chamados
            .Include(c => c.Cliente)              // Carregar dados do cliente
            .Include(c => c.TecnicoResponsavel)   // Carregar dados do técnico
            .Include(c => c.Mensagens)            // Carregar todas as mensagens
                .ThenInclude(m => m.Autor)        // Carregar autor de cada mensagem
            .Include(c => c.Anexos)               // Carregar todos os anexos
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    // ========================================================================
    // EXEMPLO 3: Listar chamados com filtros e paginação
    // ========================================================================
    public async Task<List<Chamado>> ListarChamadosAsync(
        StatusChamado? status = null,
        PrioridadeChamado? prioridade = null,
        int? clienteId = null,
        int pagina = 1,
        int tamanhoPagina = 10)
    {
        IQueryable<Chamado> query = _context.Chamados
            .Include(c => c.Cliente)
            .Include(c => c.TecnicoResponsavel)
            .AsNoTracking(); // Melhor performance para leitura

        // Aplicar filtros condicionais
        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (prioridade.HasValue)
            query = query.Where(c => c.Prioridade == prioridade.Value);

        if (clienteId.HasValue)
            query = query.Where(c => c.ClienteId == clienteId.Value);

        // Ordenar e paginar
        return await query
            .OrderByDescending(c => c.DataCriacao)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
    }

    // ========================================================================
    // EXEMPLO 4: Atualizar status do chamado
    // ========================================================================
    public async Task AtualizarStatusAsync(int id, StatusChamado novoStatus)
    {
        var chamado = await _context.Chamados.FindAsync(id);
        
        if (chamado == null)
            throw new NotFoundException("Chamado não encontrado");

        chamado.Status = novoStatus;
        
        if (novoStatus == StatusChamado.FECHADO)
            chamado.DataFechamento = DateTime.Now;

        // SaveChanges detecta automaticamente que 'chamado' foi modificado
        await _context.SaveChangesAsync();
    }

    // ========================================================================
    // EXEMPLO 5: Excluir chamado (e mensagens/anexos em cascata)
    // ========================================================================
    public async Task ExcluirChamadoAsync(int id)
    {
        var chamado = await _context.Chamados.FindAsync(id);
        
        if (chamado == null)
            throw new NotFoundException("Chamado não encontrado");

        _context.Chamados.Remove(chamado);
        await _context.SaveChangesAsync();
        
        // Mensagens e anexos são excluídos automaticamente por DeleteBehavior.Cascade
    }

    // ========================================================================
    // EXEMPLO 6: Contar chamados por status (para dashboard)
    // ========================================================================
    public async Task<Dictionary<StatusChamado, int>> ContarChamadosPorStatusAsync()
    {
        return await _context.Chamados
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }
}
```

---

## 8.6 VANTAGENS DO ENTITY FRAMEWORK CORE

### **1. Produtividade**
✅ Menos código SQL escrito manualmente  
✅ LINQ oferece type-safety e IntelliSense  
✅ Migrations versionam o schema do banco  

### **2. Segurança**
✅ Proteção automática contra SQL Injection  
✅ Parametrização de queries  
✅ Validação de dados no modelo  

### **3. Manutenibilidade**
✅ Mudanças no modelo refletem no banco via Migrations  
✅ Código C# mais legível que SQL puro  
✅ Fácil refatoração  

### **4. Performance**
✅ Compiled Queries para queries frequentes  
✅ AsNoTracking para leitura sem tracking  
✅ Split Queries para evitar cartesian explosion  
✅ Connection pooling automático  

---

## 8.7 BOAS PRÁTICAS

### **✅ Sempre usar injeção de dependência**
```csharp
// ❌ ERRADO: Instanciar DbContext manualmente
var context = new CajuAjudaDbContext();

// ✅ CORRETO: Injetar via construtor
public class MeuServico
{
    private readonly CajuAjudaDbContext _context;
    
    public MeuServico(CajuAjudaDbContext context)
    {
        _context = context;
    }
}
```

### **✅ Usar AsNoTracking para leitura**
```csharp
// Melhor performance quando não vai modificar os dados
var chamados = await _context.Chamados
    .AsNoTracking()
    .ToListAsync();
```

### **✅ Incluir relacionamentos explicitamente**
```csharp
// ❌ ERRADO: Lazy Loading pode causar N+1 queries
var chamado = await _context.Chamados.FindAsync(id);
var clienteNome = chamado.Cliente.Nome; // SELECT adicional aqui!

// ✅ CORRETO: Eager Loading
var chamado = await _context.Chamados
    .Include(c => c.Cliente)
    .FirstOrDefaultAsync(c => c.Id == id);
```

### **✅ Usar transações para operações múltiplas**
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    // Operação 1
    _context.Chamados.Add(chamado);
    await _context.SaveChangesAsync();
    
    // Operação 2
    _context.Mensagens.Add(mensagem);
    await _context.SaveChangesAsync();
    
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

---

**Resumo**: A classe `CajuAjudaDbContext` encapsula toda a lógica de acesso a dados usando Entity Framework Core, oferecendo uma interface type-safe e orientada a objetos para interagir com o banco SQL Server, com suporte a migrations, tracking de mudanças, e configuração declarativa de relacionamentos via Fluent API.
