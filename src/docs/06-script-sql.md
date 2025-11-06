# 6. SCRIPT SQL DE CRIAÇÃO DO BANCO

## 6.1 INTRODUÇÃO

Este documento contém o script SQL completo para criar todas as tabelas, constraints, índices e relacionamentos do banco de dados **CajuAjudaDB**.

> **Nota**: Este script foi gerado a partir das Migrations do Entity Framework Core e representa o estado atual do banco de dados.

---

## 6.2 SCRIPT COMPLETO

```sql
-- ============================================================================
-- SCRIPT DE CRIAÇÃO DO BANCO DE DADOS - CajuAjuda Help Desk System
-- ============================================================================
-- Versão: 1.0
-- Data: Janeiro 2025
-- SGBD: Microsoft SQL Server 2019+
-- ============================================================================

USE master;
GO

-- Criar banco de dados se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CajuAjudaDB')
BEGIN
    CREATE DATABASE CajuAjudaDB;
    PRINT 'Banco de dados CajuAjudaDB criado com sucesso!';
END
ELSE
BEGIN
    PRINT 'Banco de dados CajuAjudaDB já existe.';
END
GO

USE CajuAjudaDB;
GO

-- ============================================================================
-- TABELA: Usuarios
-- Descrição: Armazena todos os usuários do sistema (Clientes, Técnicos, Admins)
-- ============================================================================

CREATE TABLE [dbo].[Usuarios] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nome] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Senha] NVARCHAR(MAX) NOT NULL,
    [Role] NVARCHAR(50) NOT NULL,
    [DataCriacao] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [UltimoAcesso] DATETIME2(7) NULL,
    [Ativo] BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_Usuarios_Role] CHECK ([Role] IN ('CLIENTE', 'TECNICO', 'ADMIN')),
    CONSTRAINT [UQ_Usuarios_Email] UNIQUE ([Email])
);
GO

-- Índices para performance
CREATE NONCLUSTERED INDEX [IX_Usuarios_Email] ON [dbo].[Usuarios] ([Email] ASC);
CREATE NONCLUSTERED INDEX [IX_Usuarios_Role] ON [dbo].[Usuarios] ([Role] ASC);
CREATE NONCLUSTERED INDEX [IX_Usuarios_Ativo] ON [dbo].[Usuarios] ([Ativo] ASC);
GO

PRINT 'Tabela Usuarios criada com sucesso!';
GO

-- ============================================================================
-- TABELA: Chamados
-- Descrição: Armazena os tickets/chamados de suporte
-- ============================================================================

CREATE TABLE [dbo].[Chamados] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Titulo] NVARCHAR(200) NOT NULL,
    [Descricao] NVARCHAR(MAX) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [Prioridade] NVARCHAR(50) NOT NULL,
    [ClienteId] INT NOT NULL,
    [TecnicoResponsavelId] INT NULL,
    [DataCriacao] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [DataAtualizacao] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [DataFechamento] DATETIME2(7) NULL,
    [NotaAvaliacao] INT NULL,
    [ComentarioAvaliacao] NVARCHAR(500) NULL,
    
    CONSTRAINT [PK_Chamados] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_Chamados_Status] CHECK ([Status] IN ('ABERTO', 'EM_ANDAMENTO', 'AGUARDANDO_CLIENTE', 'RESOLVIDO', 'FECHADO', 'CANCELADO')),
    CONSTRAINT [CK_Chamados_Prioridade] CHECK ([Prioridade] IN ('BAIXA', 'MEDIA', 'ALTA', 'URGENTE')),
    CONSTRAINT [CK_Chamados_NotaAvaliacao] CHECK ([NotaAvaliacao] >= 1 AND [NotaAvaliacao] <= 5),
    
    -- Foreign Keys
    CONSTRAINT [FK_Chamados_Cliente] FOREIGN KEY ([ClienteId]) 
        REFERENCES [dbo].[Usuarios]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Chamados_Tecnico] FOREIGN KEY ([TecnicoResponsavelId]) 
        REFERENCES [dbo].[Usuarios]([Id]) ON DELETE NO ACTION
);
GO

-- Índices para performance
CREATE NONCLUSTERED INDEX [IX_Chamados_ClienteId] ON [dbo].[Chamados] ([ClienteId] ASC);
CREATE NONCLUSTERED INDEX [IX_Chamados_TecnicoResponsavelId] ON [dbo].[Chamados] ([TecnicoResponsavelId] ASC);
CREATE NONCLUSTERED INDEX [IX_Chamados_Status] ON [dbo].[Chamados] ([Status] ASC);
CREATE NONCLUSTERED INDEX [IX_Chamados_Prioridade] ON [dbo].[Chamados] ([Prioridade] ASC);
CREATE NONCLUSTERED INDEX [IX_Chamados_DataCriacao] ON [dbo].[Chamados] ([DataCriacao] DESC);
CREATE NONCLUSTERED INDEX [IX_Chamados_DataAtualizacao] ON [dbo].[Chamados] ([DataAtualizacao] DESC);

-- Índice composto para query comum: listar chamados por status e prioridade
CREATE NONCLUSTERED INDEX [IX_Chamados_Status_Prioridade] 
    ON [dbo].[Chamados] ([Status] ASC, [Prioridade] ASC);
GO

PRINT 'Tabela Chamados criada com sucesso!';
GO

-- ============================================================================
-- TABELA: Mensagens
-- Descrição: Armazena as mensagens do chat de cada chamado
-- ============================================================================

CREATE TABLE [dbo].[Mensagens] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ChamadoId] INT NOT NULL,
    [RemetenteId] INT NOT NULL,
    [Conteudo] NVARCHAR(MAX) NOT NULL,
    [DataEnvio] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [LidoPeloCliente] BIT NOT NULL DEFAULT 0,
    [IsNotaInterna] BIT NOT NULL DEFAULT 0,
    
    CONSTRAINT [PK_Mensagens] PRIMARY KEY CLUSTERED ([Id] ASC),
    
    -- Foreign Keys
    CONSTRAINT [FK_Mensagens_Chamado] FOREIGN KEY ([ChamadoId]) 
        REFERENCES [dbo].[Chamados]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Mensagens_Remetente] FOREIGN KEY ([RemetenteId]) 
        REFERENCES [dbo].[Usuarios]([Id]) ON DELETE NO ACTION
);
GO

-- Índices para performance
CREATE NONCLUSTERED INDEX [IX_Mensagens_ChamadoId] ON [dbo].[Mensagens] ([ChamadoId] ASC);
CREATE NONCLUSTERED INDEX [IX_Mensagens_RemetenteId] ON [dbo].[Mensagens] ([RemetenteId] ASC);
CREATE NONCLUSTERED INDEX [IX_Mensagens_DataEnvio] ON [dbo].[Mensagens] ([DataEnvio] ASC);
CREATE NONCLUSTERED INDEX [IX_Mensagens_LidoPeloCliente] ON [dbo].[Mensagens] ([LidoPeloCliente] ASC);

-- Índice composto para query comum: buscar mensagens não lidas de um chamado
CREATE NONCLUSTERED INDEX [IX_Mensagens_ChamadoId_LidoPeloCliente] 
    ON [dbo].[Mensagens] ([ChamadoId] ASC, [LidoPeloCliente] ASC);
GO

PRINT 'Tabela Mensagens criada com sucesso!';
GO

-- ============================================================================
-- TABELA: Anexos
-- Descrição: Armazena metadados dos arquivos anexados aos chamados
-- ============================================================================

CREATE TABLE [dbo].[Anexos] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ChamadoId] INT NOT NULL,
    [NomeArquivo] NVARCHAR(255) NOT NULL,
    [NomeUnico] NVARCHAR(255) NOT NULL,
    [TipoArquivo] NVARCHAR(100) NOT NULL,
    [CaminhoArquivo] NVARCHAR(500) NOT NULL,
    [Tamanho] BIGINT NOT NULL,
    [DataUpload] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [PK_Anexos] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_Anexos_Tamanho] CHECK ([Tamanho] > 0 AND [Tamanho] <= 10485760), -- Max 10MB
    
    -- Foreign Key
    CONSTRAINT [FK_Anexos_Chamado] FOREIGN KEY ([ChamadoId]) 
        REFERENCES [dbo].[Chamados]([Id]) ON DELETE CASCADE
);
GO

-- Índices para performance
CREATE NONCLUSTERED INDEX [IX_Anexos_ChamadoId] ON [dbo].[Anexos] ([ChamadoId] ASC);
CREATE NONCLUSTERED INDEX [IX_Anexos_DataUpload] ON [dbo].[Anexos] ([DataUpload] DESC);
CREATE NONCLUSTERED INDEX [IX_Anexos_NomeUnico] ON [dbo].[Anexos] ([NomeUnico] ASC);
GO

PRINT 'Tabela Anexos criada com sucesso!';
GO

-- ============================================================================
-- TABELA: RespostasProntas
-- Descrição: Armazena templates de respostas rápidas para técnicos
-- ============================================================================

CREATE TABLE [dbo].[RespostasProntas] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Titulo] NVARCHAR(200) NOT NULL,
    [Conteudo] NVARCHAR(MAX) NOT NULL,
    [Categoria] NVARCHAR(100) NULL,
    [DataCriacao] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [Ativo] BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_RespostasProntas] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_RespostasProntas_Categoria] ON [dbo].[RespostasProntas] ([Categoria] ASC);
CREATE NONCLUSTERED INDEX [IX_RespostasProntas_Ativo] ON [dbo].[RespostasProntas] ([Ativo] ASC);
GO

PRINT 'Tabela RespostasProntas criada com sucesso!';
GO

-- ============================================================================
-- TABELA: ArtigosKnowledgeBase
-- Descrição: Artigos da base de conhecimento
-- ============================================================================

CREATE TABLE [dbo].[ArtigosKnowledgeBase] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Titulo] NVARCHAR(300) NOT NULL,
    [Conteudo] NVARCHAR(MAX) NOT NULL,
    [Categoria] NVARCHAR(100) NULL,
    [Tags] NVARCHAR(500) NULL,
    [Visualizacoes] INT NOT NULL DEFAULT 0,
    [Publicado] BIT NOT NULL DEFAULT 0,
    [DataCriacao] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [DataAtualizacao] DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    [AutorId] INT NOT NULL,
    
    CONSTRAINT [PK_ArtigosKnowledgeBase] PRIMARY KEY CLUSTERED ([Id] ASC),
    
    -- Foreign Key
    CONSTRAINT [FK_ArtigosKnowledgeBase_Autor] FOREIGN KEY ([AutorId]) 
        REFERENCES [dbo].[Usuarios]([Id]) ON DELETE NO ACTION
);
GO

CREATE NONCLUSTERED INDEX [IX_ArtigosKnowledgeBase_Categoria] ON [dbo].[ArtigosKnowledgeBase] ([Categoria] ASC);
CREATE NONCLUSTERED INDEX [IX_ArtigosKnowledgeBase_Publicado] ON [dbo].[ArtigosKnowledgeBase] ([Publicado] ASC);
CREATE NONCLUSTERED INDEX [IX_ArtigosKnowledgeBase_AutorId] ON [dbo].[ArtigosKnowledgeBase] ([AutorId] ASC);
GO

PRINT 'Tabela ArtigosKnowledgeBase criada com sucesso!';
GO

-- ============================================================================
-- INSERÇÃO DE DADOS INICIAIS (SEED DATA)
-- ============================================================================

PRINT 'Inserindo dados iniciais...';
GO

-- Inserir usuário Administrador padrão
-- Senha: Admin@123 (hash bcrypt)
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'admin@cajuajuda.com')
BEGIN
    INSERT INTO Usuarios (Nome, Email, Senha, Role, DataCriacao, Ativo)
    VALUES (
        'Administrador',
        'admin@cajuajuda.com',
        '$2a$11$ZxLJcjYxKHvNb8K.yqYpQu1wPxFWqGv5B8y0NdVJPLKQG8sN5XjzO', -- Admin@123
        'ADMIN',
        GETDATE(),
        1
    );
    PRINT 'Usuário administrador criado com sucesso!';
END
GO

-- Inserir usuário Técnico padrão
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'tecnico@cajuajuda.com')
BEGIN
    INSERT INTO Usuarios (Nome, Email, Senha, Role, DataCriacao, Ativo)
    VALUES (
        'Técnico de Suporte',
        'tecnico@cajuajuda.com',
        '$2a$11$ZxLJcjYxKHvNb8K.yqYpQu1wPxFWqGv5B8y0NdVJPLKQG8sN5XjzO', -- Admin@123
        'TECNICO',
        GETDATE(),
        1
    );
    PRINT 'Usuário técnico criado com sucesso!';
END
GO

-- Inserir usuário Cliente de teste
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'cliente@teste.com')
BEGIN
    INSERT INTO Usuarios (Nome, Email, Senha, Role, DataCriacao, Ativo)
    VALUES (
        'Cliente Teste',
        'cliente@teste.com',
        '$2a$11$ZxLJcjYxKHvNb8K.yqYpQu1wPxFWqGv5B8y0NdVJPLKQG8sN5XjzO', -- Admin@123
        'CLIENTE',
        GETDATE(),
        1
    );
    PRINT 'Usuário cliente de teste criado com sucesso!';
END
GO

-- Inserir respostas prontas padrão
IF NOT EXISTS (SELECT 1 FROM RespostasProntas)
BEGIN
    INSERT INTO RespostasProntas (Titulo, Conteudo, Categoria, DataCriacao, Ativo)
    VALUES 
    ('Saudação Inicial', 'Olá! Obrigado por entrar em contato com o suporte. Como posso ajudá-lo hoje?', 'Atendimento', GETDATE(), 1),
    ('Solicitação de Informações', 'Para que eu possa ajudá-lo melhor, poderia fornecer mais detalhes sobre o problema?', 'Atendimento', GETDATE(), 1),
    ('Problema Resolvido', 'Fico feliz em saber que conseguimos resolver seu problema! Se precisar de mais ajuda, não hesite em nos contatar.', 'Finalização', GETDATE(), 1),
    ('Reiniciar Sistema', 'Por favor, tente reiniciar o sistema e verificar se o problema persiste. Aguardo seu retorno.', 'Técnico', GETDATE(), 1),
    ('Aguardando Retorno', 'Estou aguardando seu retorno com as informações solicitadas para darmos continuidade ao atendimento.', 'Atendimento', GETDATE(), 1);
    
    PRINT 'Respostas prontas inseridas com sucesso!';
END
GO

-- Inserir artigo de boas-vindas na base de conhecimento
DECLARE @AdminId INT;
SELECT @AdminId = Id FROM Usuarios WHERE Email = 'admin@cajuajuda.com';

IF NOT EXISTS (SELECT 1 FROM ArtigosKnowledgeBase)
BEGIN
    INSERT INTO ArtigosKnowledgeBase (Titulo, Conteudo, Categoria, Tags, Visualizacoes, Publicado, DataCriacao, DataAtualizacao, AutorId)
    VALUES (
        'Bem-vindo ao CajuAjuda!',
        'Este é o sistema de Help Desk CajuAjuda. Aqui você pode abrir chamados, acompanhar o atendimento e consultar nossa base de conhecimento.',
        'Geral',
        'bem-vindo,introdução,ajuda',
        0,
        1,
        GETDATE(),
        GETDATE(),
        @AdminId
    );
    PRINT 'Artigo de boas-vindas criado com sucesso!';
END
GO

-- ============================================================================
-- VIEWS ÚTEIS PARA RELATÓRIOS
-- ============================================================================

-- View: Resumo de chamados por status
CREATE OR ALTER VIEW vw_ChamadosPorStatus AS
SELECT 
    Status,
    COUNT(*) AS Quantidade,
    COUNT(CASE WHEN Prioridade = 'URGENTE' THEN 1 END) AS Urgentes,
    COUNT(CASE WHEN Prioridade = 'ALTA' THEN 1 END) AS AltaPrioridade
FROM Chamados
GROUP BY Status;
GO

PRINT 'View vw_ChamadosPorStatus criada com sucesso!';
GO

-- View: Desempenho dos técnicos
CREATE OR ALTER VIEW vw_DesempenhoTecnicos AS
SELECT 
    u.Id AS TecnicoId,
    u.Nome AS TecnicoNome,
    COUNT(c.Id) AS TotalChamados,
    COUNT(CASE WHEN c.Status = 'FECHADO' THEN 1 END) AS ChamadosFechados,
    AVG(CASE WHEN c.NotaAvaliacao IS NOT NULL THEN CAST(c.NotaAvaliacao AS FLOAT) END) AS AvaliacaoMedia
FROM Usuarios u
LEFT JOIN Chamados c ON u.Id = c.TecnicoResponsavelId
WHERE u.Role = 'TECNICO'
GROUP BY u.Id, u.Nome;
GO

PRINT 'View vw_DesempenhoTecnicos criada com sucesso!';
GO

-- ============================================================================
-- STORED PROCEDURES
-- ============================================================================

-- Procedure: Atribuir chamado a um técnico
CREATE OR ALTER PROCEDURE sp_AtribuirChamado
    @ChamadoId INT,
    @TecnicoId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar se o chamado existe
        IF NOT EXISTS (SELECT 1 FROM Chamados WHERE Id = @ChamadoId)
        BEGIN
            RAISERROR('Chamado não encontrado.', 16, 1);
            RETURN;
        END
        
        -- Validar se o usuário é técnico
        IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Id = @TecnicoId AND Role IN ('TECNICO', 'ADMIN'))
        BEGIN
            RAISERROR('Usuário não é um técnico.', 16, 1);
            RETURN;
        END
        
        -- Atualizar chamado
        UPDATE Chamados
        SET 
            TecnicoResponsavelId = @TecnicoId,
            Status = 'EM_ANDAMENTO',
            DataAtualizacao = GETDATE()
        WHERE Id = @ChamadoId;
        
        COMMIT TRANSACTION;
        PRINT 'Chamado atribuído com sucesso!';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

PRINT 'Procedure sp_AtribuirChamado criada com sucesso!';
GO

-- Procedure: Fechar chamado
CREATE OR ALTER PROCEDURE sp_FecharChamado
    @ChamadoId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        UPDATE Chamados
        SET 
            Status = 'FECHADO',
            DataFechamento = GETDATE(),
            DataAtualizacao = GETDATE()
        WHERE Id = @ChamadoId;
        
        COMMIT TRANSACTION;
        PRINT 'Chamado fechado com sucesso!';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

PRINT 'Procedure sp_FecharChamado criada com sucesso!';
GO

-- ============================================================================
-- TRIGGERS
-- ============================================================================

-- Trigger: Atualizar DataAtualizacao automaticamente em Chamados
CREATE OR ALTER TRIGGER tr_Chamados_AtualizarDataAtualizacao
ON Chamados
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Chamados
    SET DataAtualizacao = GETDATE()
    FROM Chamados c
    INNER JOIN inserted i ON c.Id = i.Id;
END
GO

PRINT 'Trigger tr_Chamados_AtualizarDataAtualizacao criado com sucesso!';
GO

-- ============================================================================
-- VERIFICAÇÃO FINAL
-- ============================================================================

PRINT '';
PRINT '============================================================================';
PRINT 'RESUMO DA CRIAÇÃO DO BANCO DE DADOS';
PRINT '============================================================================';

SELECT 'Tabelas' AS Tipo, COUNT(*) AS Quantidade
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
UNION ALL
SELECT 'Views', COUNT(*)
FROM INFORMATION_SCHEMA.VIEWS
UNION ALL
SELECT 'Procedures', COUNT(*)
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
UNION ALL
SELECT 'Triggers', COUNT(*)
FROM sys.triggers
WHERE is_ms_shipped = 0;

PRINT '';
PRINT 'Banco de dados CajuAjudaDB criado e configurado com sucesso!';
PRINT '============================================================================';
GO
```

---

## 6.3 SCRIPT DE REMOÇÃO (DROP)

```sql
-- ============================================================================
-- SCRIPT DE REMOÇÃO DO BANCO DE DADOS
-- ============================================================================
-- ATENÇÃO: Este script APAGA TODOS OS DADOS!
-- Use apenas em ambiente de desenvolvimento.
-- ============================================================================

USE master;
GO

-- Forçar desconexão de todos os usuários
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'CajuAjudaDB')
BEGIN
    ALTER DATABASE CajuAjudaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CajuAjudaDB;
    PRINT 'Banco de dados CajuAjudaDB removido com sucesso!';
END
ELSE
BEGIN
    PRINT 'Banco de dados CajuAjudaDB não existe.';
END
GO
```

---

## 6.4 COMO EXECUTAR O SCRIPT

### **Opção 1: SQL Server Management Studio (SSMS)**

1. Abrir o SSMS
2. Conectar ao servidor SQL Server
3. Clicar em **File → Open → File**
4. Selecionar o arquivo SQL
5. Clicar em **Execute** (F5)

### **Opção 2: Azure Data Studio**

1. Abrir o Azure Data Studio
2. Conectar ao servidor
3. Clicar em **File → Open File**
4. Selecionar o arquivo SQL
5. Clicar em **Run** (F5)

### **Opção 3: Linha de Comando (sqlcmd)**

```bash
sqlcmd -S localhost -i script_criacao.sql
```

### **Opção 4: Entity Framework Migrations (Recomendado)**

```bash
# Backend directory
cd backend

# Aplicar todas as migrations
dotnet ef database update

# Ver status das migrations
dotnet ef migrations list
```

---

## 6.5 VERIFICAÇÕES PÓS-CRIAÇÃO

### **Verificar se as tabelas foram criadas**

```sql
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

**Resultado esperado:**
- Anexos
- ArtigosKnowledgeBase
- Chamados
- Mensagens
- RespostasProntas
- Usuarios

### **Verificar constraints de chave estrangeira**

```sql
SELECT 
    fk.name AS ForeignKey,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumn
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
ORDER BY TableName;
```

### **Verificar índices criados**

```sql
SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
WHERE OBJECT_NAME(i.object_id) IN ('Usuarios', 'Chamados', 'Mensagens', 'Anexos')
ORDER BY TableName, IndexName;
```

### **Contar registros inseridos (seed data)**

```sql
SELECT 'Usuarios' AS Tabela, COUNT(*) AS Registros FROM Usuarios
UNION ALL
SELECT 'RespostasProntas', COUNT(*) FROM RespostasProntas
UNION ALL
SELECT 'ArtigosKnowledgeBase', COUNT(*) FROM ArtigosKnowledgeBase;
```

**Resultado esperado:**
- Usuarios: 3 (Admin, Técnico, Cliente)
- RespostasProntas: 5
- ArtigosKnowledgeBase: 1

---

## 6.6 CREDENCIAIS DE ACESSO INICIAL

| Email | Senha | Perfil |
|-------|-------|--------|
| admin@cajuajuda.com | Admin@123 | ADMIN |
| tecnico@cajuajuda.com | Admin@123 | TECNICO |
| cliente@teste.com | Admin@123 | CLIENTE |

> ⚠️ **IMPORTANTE**: Alterar estas senhas em produção!

---

## 6.7 TROUBLESHOOTING

### **Erro: "Database already exists"**

```sql
-- Remover banco existente
DROP DATABASE CajuAjudaDB;
-- Executar novamente o script de criação
```

### **Erro: "Cannot drop database because it is currently in use"**

```sql
USE master;
ALTER DATABASE CajuAjudaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE CajuAjudaDB;
```

### **Erro: "Login failed for user"**

- Verificar se o usuário do Windows tem permissão
- Ou criar um login SQL Server:

```sql
CREATE LOGIN cajuajuda_user WITH PASSWORD = 'SenhaForte@123';
CREATE USER cajuajuda_user FOR LOGIN cajuajuda_user;
ALTER ROLE db_owner ADD MEMBER cajuajuda_user;
```

---

**Resumo**: Este script SQL completo cria todas as estruturas necessárias (tabelas, índices, constraints, views, procedures, triggers) e insere dados iniciais para começar a usar o sistema imediatamente após a execução.
