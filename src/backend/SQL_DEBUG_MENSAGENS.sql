-- Script de Diagnóstico - Verificar Mensagens Salvas
-- Execute este script no SQL Server Management Studio ou Azure Data Studio

-- 1. Verificar todas as mensagens de um chamado específico
DECLARE @ChamadoId INT = 30013; -- ALTERE ESTE NÚMERO PARA O ID DO SEU CHAMADO

SELECT 
    m.Id AS MensagemId,
    m.Texto,
    m.DataEnvio,
    m.AutorId,
    u.Nome AS AutorNome,
    u.Role AS AutorRole,
    m.IsNotaInterna,
    m.LidoPeloCliente,
    m.ChamadoId
FROM 
    Mensagens m
    INNER JOIN Usuarios u ON m.AutorId = u.Id
WHERE 
    m.ChamadoId = @ChamadoId
ORDER BY 
    m.DataEnvio ASC;

-- 2. Contar mensagens por chamado
SELECT 
    c.Id AS ChamadoId,
    c.Titulo,
    c.Status,
    COUNT(m.Id) AS TotalMensagens
FROM 
    Chamados c
    LEFT JOIN Mensagens m ON c.Id = m.ChamadoId
WHERE 
    c.Id = @ChamadoId
GROUP BY 
    c.Id, c.Titulo, c.Status;

-- 3. Últimas 10 mensagens inseridas (qualquer chamado)
SELECT TOP 10
    m.Id AS MensagemId,
    m.Texto,
    m.DataEnvio,
    u.Nome AS AutorNome,
    u.Role AS AutorRole,
    m.ChamadoId,
    c.Titulo AS ChamadoTitulo
FROM 
    Mensagens m
    INNER JOIN Usuarios u ON m.AutorId = u.Id
    INNER JOIN Chamados c ON m.ChamadoId = c.Id
ORDER BY 
    m.DataEnvio DESC;

-- 4. Verificar se há mensagens "órfãs" (sem chamado ou autor)
SELECT 
    'Mensagens sem Autor' AS Problema,
    COUNT(*) AS Quantidade
FROM 
    Mensagens m
    LEFT JOIN Usuarios u ON m.AutorId = u.Id
WHERE 
    u.Id IS NULL

UNION ALL

SELECT 
    'Mensagens sem Chamado' AS Problema,
    COUNT(*) AS Quantidade
FROM 
    Mensagens m
    LEFT JOIN Chamados c ON m.ChamadoId = c.Id
WHERE 
    c.Id IS NULL;

-- 5. Verificar estrutura da tabela Mensagens
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'Mensagens'
ORDER BY 
    ORDINAL_POSITION;
