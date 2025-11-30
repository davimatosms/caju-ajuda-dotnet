IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250828211720_InitialCreate'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [Id] bigint NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [Senha] nvarchar(max) NOT NULL,
        [Role] int NOT NULL,
        [Enabled] bit NOT NULL,
        [VerificationToken] nvarchar(max) NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250828211720_InitialCreate'
)
BEGIN
    CREATE TABLE [Chamados] (
        [Id] bigint NOT NULL IDENTITY,
        [Titulo] nvarchar(150) NOT NULL,
        [Descricao] nvarchar(max) NOT NULL,
        [Status] int NOT NULL,
        [Prioridade] int NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataFechamento] datetime2 NULL,
        [ClienteId] bigint NOT NULL,
        CONSTRAINT [PK_Chamados] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Chamados_Usuarios_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250828211720_InitialCreate'
)
BEGIN
    CREATE TABLE [Anexos] (
        [Id] bigint NOT NULL IDENTITY,
        [NomeArquivo] nvarchar(max) NOT NULL,
        [NomeUnico] nvarchar(max) NOT NULL,
        [TipoArquivo] nvarchar(max) NOT NULL,
        [ChamadoId] bigint NOT NULL,
        CONSTRAINT [PK_Anexos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Anexos_Chamados_ChamadoId] FOREIGN KEY ([ChamadoId]) REFERENCES [Chamados] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250828211720_InitialCreate'
)
BEGIN
    CREATE TABLE [Mensagens] (
        [Id] bigint NOT NULL IDENTITY,
        [Texto] nvarchar(max) NOT NULL,
        [DataEnvio] datetime2 NOT NULL,
        [AutorId] bigint NOT NULL,
        [ChamadoId] bigint NOT NULL,
        CONSTRAINT [PK_Mensagens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Mensagens_Chamados_ChamadoId] FOREIGN KEY ([ChamadoId]) REFERENCES [Chamados] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Mensagens_Usuarios_AutorId] FOREIGN KEY ([AutorId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250828211720_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Anexos_ChamadoId] ON [Anexos] ([ChamadoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250828211720_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Chamados_ClienteId] ON [Chamados] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250828211720_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Mensagens_AutorId] ON [Mensagens] ([AutorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250828211720_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Mensagens_ChamadoId] ON [Mensagens] ([ChamadoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250828211720_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250828211720_InitialCreate', N'9.0.8');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250904172349_AddLidoPeloClienteToMensagem'
)
BEGIN
    ALTER TABLE [Mensagens] ADD [LidoPeloCliente] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250904172349_AddLidoPeloClienteToMensagem'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250904172349_AddLidoPeloClienteToMensagem', N'9.0.8');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905155235_AddTecnicoResponsavelToChamado'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250905155235_AddTecnicoResponsavelToChamado', N'9.0.8');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905160150_AddIsNotaInternaToMensagem'
)
BEGIN
    ALTER TABLE [Mensagens] ADD [IsNotaInterna] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905160150_AddIsNotaInternaToMensagem'
)
BEGIN
    ALTER TABLE [Chamados] ADD [TecnicoResponsavelId] bigint NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905160150_AddIsNotaInternaToMensagem'
)
BEGIN
    CREATE INDEX [IX_Chamados_TecnicoResponsavelId] ON [Chamados] ([TecnicoResponsavelId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905160150_AddIsNotaInternaToMensagem'
)
BEGIN
    ALTER TABLE [Chamados] ADD CONSTRAINT [FK_Chamados_Usuarios_TecnicoResponsavelId] FOREIGN KEY ([TecnicoResponsavelId]) REFERENCES [Usuarios] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905160150_AddIsNotaInternaToMensagem'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250905160150_AddIsNotaInternaToMensagem', N'9.0.8');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905160817_AddRespostasProntasTable'
)
BEGIN
    CREATE TABLE [RespostasProntas] (
        [Id] bigint NOT NULL IDENTITY,
        [Titulo] nvarchar(150) NOT NULL,
        [Corpo] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_RespostasProntas] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905160817_AddRespostasProntasTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250905160817_AddRespostasProntasTable', N'9.0.8');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905165803_AddAvaliacaoToChamado'
)
BEGIN
    ALTER TABLE [Chamados] ADD [ComentarioAvaliacao] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905165803_AddAvaliacaoToChamado'
)
BEGIN
    ALTER TABLE [Chamados] ADD [NotaAvaliacao] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905165803_AddAvaliacaoToChamado'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250905165803_AddAvaliacaoToChamado', N'9.0.8');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905170801_AddKnowledgeBaseTables'
)
BEGIN
    CREATE TABLE [KbCategorias] (
        [Id] bigint NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_KbCategorias] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905170801_AddKnowledgeBaseTables'
)
BEGIN
    CREATE TABLE [KbArtigos] (
        [Id] bigint NOT NULL IDENTITY,
        [Titulo] nvarchar(200) NOT NULL,
        [Conteudo] nvarchar(max) NOT NULL,
        [CategoriaId] bigint NOT NULL,
        CONSTRAINT [PK_KbArtigos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_KbArtigos_KbCategorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [KbCategorias] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905170801_AddKnowledgeBaseTables'
)
BEGIN
    CREATE INDEX [IX_KbArtigos_CategoriaId] ON [KbArtigos] ([CategoriaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250905170801_AddKnowledgeBaseTables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250905170801_AddKnowledgeBaseTables', N'9.0.8');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250906023518_AddMergeFieldsToChamado'
)
BEGIN
    ALTER TABLE [Chamados] ADD [ChamadoPrincipalId] bigint NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250906023518_AddMergeFieldsToChamado'
)
BEGIN
    CREATE INDEX [IX_Chamados_ChamadoPrincipalId] ON [Chamados] ([ChamadoPrincipalId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250906023518_AddMergeFieldsToChamado'
)
BEGIN
    ALTER TABLE [Chamados] ADD CONSTRAINT [FK_Chamados_Chamados_ChamadoPrincipalId] FOREIGN KEY ([ChamadoPrincipalId]) REFERENCES [Chamados] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250906023518_AddMergeFieldsToChamado'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250906023518_AddMergeFieldsToChamado', N'9.0.8');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250916225558_AdicionarTabelasDoPerfil'
)
BEGIN
    DROP TABLE [KbArtigos];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250916225558_AdicionarTabelasDoPerfil'
)
BEGIN
    DROP TABLE [KbCategorias];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250916225558_AdicionarTabelasDoPerfil'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250916225558_AdicionarTabelasDoPerfil', N'9.0.8');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251028203216_AddSugestaoIAToChamado'
)
BEGIN
    ALTER TABLE [Chamados] ADD [SugestaoIA] nvarchar(4000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251028203216_AddSugestaoIAToChamado'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251028203216_AddSugestaoIAToChamado', N'9.0.8');
END;

COMMIT;
GO

