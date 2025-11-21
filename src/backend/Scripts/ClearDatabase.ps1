# ====================================================================
# Script PowerShell para Limpar o Banco de Dados CajuAjudaDB
# ====================================================================
# Execute este script no PowerShell como Administrador
# ====================================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Limpando Banco de Dados CajuAjudaDB" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Confirmar antes de executar
$confirmacao = Read-Host "ATENÇÃO! Este script irá DELETAR TODOS OS DADOS do banco. Deseja continuar? (S/N)"

if ($confirmacao -ne "S" -and $confirmacao -ne "s") {
    Write-Host "Operação cancelada pelo usuário." -ForegroundColor Yellow
    exit
}

Write-Host ""
Write-Host "Executando limpeza..." -ForegroundColor Green

# Conectar ao SQL Server e executar comandos
$sqlCommands = @"
USE CajuAjudaDB;

-- Desabilitar constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

-- Deletar dados
DELETE FROM [Anexos];
DELETE FROM [Mensagens];
DELETE FROM [Chamados];
DELETE FROM [RespostasProntas];
DELETE FROM [Usuarios] WHERE Email != 'admin@cajuajuda.com';

-- Resetar IDs
DBCC CHECKIDENT ('Anexos', RESEED, 0);
DBCC CHECKIDENT ('Mensagens', RESEED, 0);
DBCC CHECKIDENT ('Chamados', RESEED, 0);
DBCC CHECKIDENT ('RespostasProntas', RESEED, 0);

-- Reabilitar constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';

-- Mostrar contagem
SELECT 
    'Usuarios' AS Tabela, COUNT(*) AS Total FROM Usuarios
UNION ALL SELECT 'Chamados', COUNT(*) FROM Chamados
UNION ALL SELECT 'Mensagens', COUNT(*) FROM Mensagens
UNION ALL SELECT 'Anexos', COUNT(*) FROM Anexos
UNION ALL SELECT 'RespostasProntas', COUNT(*) FROM RespostasProntas;
"@

try {
    # Executar via sqlcmd
    $sqlCommands | sqlcmd -S localhost -E -d CajuAjudaDB
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Limpeza concluída com sucesso!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "O usuário admin@cajuajuda.com foi mantido." -ForegroundColor Yellow
    Write-Host "Todos os outros dados foram removidos." -ForegroundColor Yellow
}
catch {
    Write-Host ""
    Write-Host "ERRO ao executar o script!" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host ""
Read-Host "Pressione ENTER para sair"
