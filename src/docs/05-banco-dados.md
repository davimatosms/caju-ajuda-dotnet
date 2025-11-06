# 5. BANCO DE DADOS

## 5.1 SGBD UTILIZADO

O sistema CajuAjuda utiliza **Microsoft SQL Server 2019** como Sistema Gerenciador de Banco de Dados (SGBD).

### **Justificativa da Escolha**

✅ **Performance**: Alta performance para aplicações enterprise  
✅ **Integração**: Integração nativa com Entity Framework Core  
✅ **Transações**: Suporte robusto a transações ACID  
✅ **Ferramentas**: SQL Server Management Studio (SSMS) para gerenciamento  
✅ **Compatibilidade**: Totalmente compatível com .NET 8.0  
✅ **Recursos Avançados**: Índices, procedures, triggers, views  

---

## 5.2 TIPO DE ACESSO AO BANCO DE DADOS

### **🖥️ ACESSO LOCAL (Desenvolvimento)**

O banco de dados está configurado para execução **local** durante o desenvolvimento:

**String de Conexão:**
```
Server=localhost;Database=CajuAjudaDB;Trusted_Connection=True;TrustServerCertificate=True;
```

**Características**:
- **Servidor**: localhost (127.0.0.1)
- **Porta**: 1433 (padrão SQL Server)
- **Autenticação**: Windows Authentication (Trusted_Connection)
- **Banco**: CajuAjudaDB
- **Certificado**: TrustServerCertificate=True (desenvolvimento)
- **Performance**: Sem latência de rede

**Localização do arquivo de configuração**:
```
backend/appsettings.json
backend/appsettings.Development.json
```

---

### **☁️ POSSIBILIDADE DE ACESSO EM NUVEM**

O sistema está preparado para migração para **Azure SQL Database** ou **AWS RDS for SQL Server**.

**String de Conexão para Azure SQL Database:**
```
Server=tcp:cajuajuda.database.windows.net,1433;
Initial Catalog=CajuAjudaDB;
Persist Security Info=False;
User ID=admin@cajuajuda;
Password={senha};
MultipleActiveResultSets=False;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
```

**Vantagens da Nuvem**:
- ☁️ Alta disponibilidade (SLA de 99.99%)
- 📈 Escalabilidade automática (scale up/down)
- 🔄 Backup automático diário
- 🌍 Acesso global de qualquer localização
- 🔒 Segurança gerenciada pela Microsoft
- 📊 Monitoramento integrado
- 💰 Modelo pay-as-you-go

---

## 5.3 TECNOLOGIA DE ACESSO A DADOS

O sistema utiliza **Entity Framework Core 8.0** como ORM (Object-Relational Mapper).

### **Principais Recursos Utilizados**

#### **1. Code-First Migrations**
Versionamento do schema do banco de dados através de código C#:

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Aplicar migrations ao banco
dotnet ef database update

# Reverter migration
dotnet ef database update NomeMigrationAnterior
```

#### **2. LINQ (Language Integrated Query)**
Consultas type-safe diretamente em C#:

```csharp
// Exemplo de consulta LINQ
var chamados = await _context.Chamados
    .Where(c => c.Status == StatusChamado.ABERTO)
    .Include(c => c.Cliente)
    .OrderByDescending(c => c.DataCriacao)
    .ToListAsync();
```

#### **3. Fluent API**
Configuração avançada de entidades e relacionamentos:

```csharp
modelBuilder.Entity<Chamado>()
    .HasOne(c => c.Cliente)
    .WithMany(u => u.ChamadosCliente)
    .HasForeignKey(c => c.ClienteId)
    .OnDelete(DeleteBehavior.NoAction);
```

#### **4. Change Tracking**
Rastreamento automático de mudanças em entidades:

```csharp
var chamado = await _context.Chamados.FindAsync(id);
chamado.Status = StatusChamado.FECHADO;
await _context.SaveChangesAsync(); // EF detecta a mudança automaticamente
```

#### **5. Lazy Loading e Eager Loading**
Otimização de carregamento de dados relacionados:

```csharp
// Eager Loading (carrega tudo de uma vez)
var chamado = await _context.Chamados
    .Include(c => c.Cliente)
    .Include(c => c.Mensagens)
    .Include(c => c.Anexos)
    .FirstOrDefaultAsync(c => c.Id == id);

// Lazy Loading (carrega sob demanda)
var cliente = chamado.Cliente; // Carrega automaticamente se configurado
```

---

## 5.4 ESTRUTURA DO BANCO DE DADOS

### **Tabelas Principais**

| Tabela | Registros (aprox.) | Descrição |
|--------|-------------------|-----------|
| **Usuarios** | 100-1.000 | Armazena clientes, técnicos e admins |
| **Chamados** | 1.000-10.000 | Tickets de suporte |
| **Mensagens** | 5.000-50.000 | Mensagens do chat |
| **Anexos** | 500-5.000 | Metadados de arquivos |

---

### **Tamanho Estimado do Banco**

**Desenvolvimento**: ~100 MB  
**Produção (1 ano)**: ~2-5 GB  
**Produção (5 anos)**: ~10-25 GB  

---

## 5.5 ESTRATÉGIA DE BACKUP

### **Desenvolvimento**
- ✅ Backup manual antes de migrations críticas
- ✅ Scripts SQL em controle de versão (Git)

### **Produção (Recomendado)**
- 📅 Backup completo diário (Full Backup)
- 🕐 Backup diferencial a cada 6 horas
- 📝 Backup de log de transações a cada hora
- 🗄️ Retenção de 30 dias
- ☁️ Armazenamento redundante (Azure Blob Storage)

---

## 5.6 PERFORMANCE E OTIMIZAÇÃO

### **Índices Criados**

| Tabela | Índice | Tipo | Justificativa |
|--------|--------|------|---------------|
| Usuarios | IX_Usuarios_Email | UNIQUE | Busca rápida no login |
| Usuarios | IX_Usuarios_Role | NONCLUSTERED | Filtro por perfil |
| Chamados | IX_Chamados_ClienteId | NONCLUSTERED | FK lookup |
| Chamados | IX_Chamados_TecnicoId | NONCLUSTERED | FK lookup |
| Chamados | IX_Chamados_Status | NONCLUSTERED | Filtro frequente |
| Chamados | IX_Chamados_DataCriacao | NONCLUSTERED | Ordenação temporal |
| Mensagens | IX_Mensagens_ChamadoId | NONCLUSTERED | Chat lookup |
| Mensagens | IX_Mensagens_DataEnvio | NONCLUSTERED | Ordenação temporal |

### **Queries Otimizadas**

✅ **Paginação**: Implementada em endpoints de listagem  
✅ **Select Específico**: Apenas colunas necessárias são carregadas  
✅ **AsNoTracking**: Usado em queries read-only para melhor performance  
✅ **Índices Compostos**: Criados para queries frequentes com múltiplos filtros  

---

## 5.7 SEGURANÇA DO BANCO

### **Medidas Implementadas**

🔒 **Autenticação**: Windows Authentication ou SQL Authentication com senha forte  
🔒 **Autorização**: Usuário do banco com permissões mínimas necessárias  
🔒 **Criptografia**: Senhas com hash bcrypt (nunca em texto plano)  
🔒 **SQL Injection**: Prevenido através do uso de Entity Framework (parametrização automática)  
🔒 **Auditoria**: Logs de acesso e alterações (planejado para v2.0)  

---

## 5.8 FERRAMENTAS DE GERENCIAMENTO

### **SQL Server Management Studio (SSMS)**
- ✅ Interface gráfica para gerenciamento
- ✅ Execução de queries SQL
- ✅ Visualização de estrutura e dados
- ✅ Backup e restore
- ✅ Análise de performance

### **Azure Data Studio** (Alternativa)
- ✅ Multiplataforma (Windows, Linux, macOS)
- ✅ Interface moderna
- ✅ Extensões e customização

### **Entity Framework Core CLI**
```bash
# Ver migrations aplicadas
dotnet ef migrations list

# Gerar script SQL de uma migration
dotnet ef migrations script

# Remover última migration
dotnet ef migrations remove
```

---

## 5.9 CONFIGURAÇÃO NO CÓDIGO

### **Arquivo: appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CajuAjudaDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  }
}
```

### **Arquivo: Program.cs (Registro do DbContext)**
```csharp
builder.Services.AddDbContext<CajuAjudaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
            );
        }
    )
);
```

---

## 5.10 MIGRAÇÃO PARA PRODUÇÃO

### **Checklist de Deploy**

- [ ] Alterar string de conexão para servidor de produção
- [ ] Remover `TrustServerCertificate=True`
- [ ] Habilitar SSL/TLS
- [ ] Configurar backup automático
- [ ] Ajustar performance settings do SQL Server
- [ ] Monitorar queries lentas (> 1 segundo)
- [ ] Configurar alertas de espaço em disco
- [ ] Documentar procedimentos de restore
- [ ] Testar failover e recuperação de desastres

---

**Resumo**: O banco de dados SQL Server foi escolhido pela robustez, integração com .NET e ferramentas de gerenciamento. A arquitetura permite fácil migração para nuvem quando necessário, mantendo compatibilidade total com o código existente.
