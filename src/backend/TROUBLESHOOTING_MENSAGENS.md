# 🐛 Troubleshooting - Mensagens Não Persistem

## Status Atual
✅ Backend compila sem erros  
✅ Proteção de rotas funcionando  
✅ `ChangeTracker.Clear()` implementado  
✅ `.AsNoTracking()` implementado  
❌ Mensagens não persistem após recarregar página  

---

## 🔍 Diagnóstico Passo a Passo

### Passo 1: Reiniciar o Backend ⚠️ CRÍTICO
**Você DEVE reiniciar o backend para aplicar as mudanças!**

#### Visual Studio:
1. Pressione **Stop** (Shift+F5)
2. Pressione **Start** (F5)
3. Aguarde até ver: `Application started. Press Ctrl+C to shut down.`

#### CLI (.NET):
```powershell
# Pare o processo atual (Ctrl+C)
cd C:\Users\vdesg\CajuAjuda-DotNet\src\backend
dotnet run
```

---

### Passo 2: Verificar Logs do Backend

Após reiniciar, envie uma mensagem e procure por estes logs no Output do Visual Studio:

```
📝 [MensagemRepository] Adicionando mensagem ID 50023 ao contexto...
💾 [MensagemRepository] Salvando mensagem no banco de dados...
✅ [MensagemRepository] Mensagem ID 50023 salva com sucesso!
🧹 [MensagemRepository] Cache do ChangeTracker limpo.
```

**Se NÃO aparecer:**
- ❌ Backend não foi reiniciado
- ❌ Código antigo ainda está rodando

**Se aparecer:**
- ✅ Mensagem foi salva no banco
- Prossiga para Passo 3

---

### Passo 3: Verificar no SQL Server

Execute o script `SQL_DEBUG_MENSAGENS.sql`:

1. Abra **SQL Server Management Studio** ou **Azure Data Studio**
2. Conecte ao servidor: `localhost` (ou seu servidor)
3. Abra o arquivo `SQL_DEBUG_MENSAGENS.sql`
4. **ALTERE** a linha 4: `DECLARE @ChamadoId INT = 30013;` (coloque o ID do seu chamado)
5. Execute o script (F5)

**Resultado Esperado:**
```
MensagemId | Texto          | DataEnvio           | AutorNome
---------------------------------------------------------------
50021      | teste 1        | 2025-10-28 14:30:00 | Vdesgo
50022      | teste 2        | 2025-10-28 14:31:00 | Vdesgo
50023      | teste 3        | 2025-10-28 14:32:00 | Vdesgo
```

**Se aparecer:**
- ✅ Mensagens estão no banco
- Problema está no carregamento (Passo 4)

**Se NÃO aparecer:**
- ❌ Mensagens não estão sendo salvas
- Verifique string de conexão em `appsettings.json`

---

### Passo 4: Verificar Carregamento no Backend

Quando você **abre** um chamado, procure estes logs:

```
🔍 [ChamadoRepository] Buscando chamado ID 30013 com AsNoTracking...
📦 [ChamadoRepository] Chamado ID 30013 carregado com 3 mensagens
```

**Se mostrar "carregado com 0 mensagens":**
- ❌ Include não está funcionando
- Veja Passo 5

**Se mostrar "carregado com 3 mensagens" (mas frontend mostra 0):**
- ❌ Problema na serialização JSON
- Veja Passo 6

---

### Passo 5: Verificar Include do Entity Framework

Execute esta query SQL para confirmar as mensagens:

```sql
SELECT c.Id, c.Titulo, COUNT(m.Id) AS TotalMensagens
FROM Chamados c
LEFT JOIN Mensagens m ON c.Id = m.ChamadoId
WHERE c.Id = 30013
GROUP BY c.Id, c.Titulo;
```

**Se retornar TotalMensagens > 0:**
- ✅ Mensagens existem
- ❌ `.Include()` não está carregando
- Solução: Verificar `ChamadoRepository.GetByIdAsync()`

---

### Passo 6: Verificar Serialização JSON

No **Network** do navegador (F12):

1. Abra um chamado
2. Aba **Network** → Filtrar: `XHR`
3. Procure: `GET /api/chamados/30013`
4. Clique → **Response**

**Resposta Esperada:**
```json
{
  "id": 30013,
  "titulo": "...",
  "mensagens": {
    "$values": [
      {
        "id": 50021,
        "texto": "teste 1",
        "dataEnvio": "2025-10-28T14:30:00",
        "autorNome": "Vdesgo"
      }
    ]
  }
}
```

**Se mensagens: { $values: [] } (array vazio):**
- ❌ Backend não está enviando mensagens
- Volte ao Passo 4

**Se mensagens não aparecer:**
- ❌ Controller não está retornando DTO correto
- Verifique `ChamadoService.GetChamadoByIdAsync()`

---

### Passo 7: Verificar Frontend (React)

Console do navegador (F12 → Console):

Procure por erros:
```
❌ TypeError: Cannot read property '$values' of undefined
❌ mensagens is undefined
```

**Se houver erros:**
- ❌ Frontend não está parseando resposta
- Verifique `ChamadoDetailPage.tsx` linha 24-36

---

## 🔧 Soluções Comuns

### Solução 1: Limpar Cache Completo
```powershell
# Frontend
cd C:\Users\vdesg\CajuAjuda-DotNet\src\web
npm cache clean --force
rm -rf node_modules
npm install

# Backend
cd C:\Users\vdesg\CajuAjuda-DotNet\src\backend
dotnet clean
dotnet build
```

### Solução 2: Resetar Banco de Dados
```powershell
cd C:\Users\vdesg\CajuAjuda-DotNet\src\backend
dotnet ef database drop --force
dotnet ef database update
```
⚠️ **ATENÇÃO:** Isso apaga TODOS os dados!

### Solução 3: Verificar String de Conexão
`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CajuAjudaDB;..."
  }
}
```

---

## 📊 Checklist de Verificação

- [ ] Backend reiniciado após alterações no código
- [ ] Logs aparecem no Output do Visual Studio
- [ ] Mensagens aparecem no SQL Server (query direta)
- [ ] Log mostra "Chamado carregado com X mensagens" (X > 0)
- [ ] Network tab mostra mensagens no JSON Response
- [ ] Console do navegador não mostra erros
- [ ] Frontend atualiza estado corretamente

---

## 🆘 Se Nada Funcionar

Execute este comando para coletar informações:

```powershell
# Backend
cd C:\Users\vdesg\CajuAjuda-DotNet\src\backend
dotnet --version
dotnet ef --version

# Frontend
cd C:\Users\vdesg\CajuAjuda-DotNet\src\web
node --version
npm --version
```

E envie:
1. Versões acima
2. Screenshot dos logs do Visual Studio
3. Screenshot do SQL Server Management Studio (query de mensagens)
4. Screenshot do Network tab (Response JSON)
5. Screenshot do Console (erros)
