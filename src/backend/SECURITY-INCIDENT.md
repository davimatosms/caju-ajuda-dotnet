# ⚠️ CONFIGURAÇÃO DE SEGREDOS - IMPORTANTE

## 🚨 SEGREDOS VAZADOS DETECTADOS

O GitGuardian detectou segredos expostos no repositório. **Ações tomadas:**

### 1. Remover segredos do appsettings.json
✅ JWT Key substituída por placeholder
✅ Senha de email substituída por placeholder

### 2. Criar configuração local (NÃO commitada)

Crie o arquivo `appsettings.Development.local.json` com suas credenciais REAIS:

```json
{
  "Jwt": {
    "Key": "SUA_NOVA_CHAVE_JWT_AQUI_MINIMO_32_CARACTERES"
  },
  "SmtpSettings": {
    "Password": "SUA_SENHA_DE_APP_DO_GMAIL_AQUI"
  }
}
```

### 3. Ações URGENTES necessárias:

#### 🔴 GMAIL - Revogar senha exposta:
1. Acesse: https://myaccount.google.com/apppasswords
2. **DELETE** a senha `npll uuyn uqvt aozs` (COMPROMETIDA)
3. Crie uma **NOVA** senha de aplicativo
4. Cole a nova senha no arquivo `appsettings.Development.local.json`

#### 🔴 JWT - Gerar nova chave:
```bash
# PowerShell - Gerar nova chave aleatória
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | % {[char]$_})
```

Cole a chave gerada no arquivo `appsettings.Development.local.json`

### 4. Adicionar ao .gitignore

Certifique-se de que estes arquivos estão no `.gitignore`:

```
# Secrets
appsettings.Development.local.json
appsettings.Production.json
*.local.json

# User-specific files
appsettings.*.json
!appsettings.json
!appsettings.Development.json
```

### 5. Limpar histórico Git (OPCIONAL - Avançado)

⚠️ **Apenas se necessário:** Para remover completamente os segredos do histórico do Git, você precisaria reescrever o histórico (BFG Repo-Cleaner ou git filter-branch). Isso é complexo e pode quebrar o repositório compartilhado.

**Alternativa mais simples:** Apenas revogue as credenciais antigas e use novas.

---

## ✅ Checklist de Segurança

- [ ] Revoguei a senha de email antiga do Gmail
- [ ] Gerei nova senha de aplicativo do Gmail
- [ ] Gerei nova chave JWT
- [ ] Criei `appsettings.Development.local.json` com as novas credenciais
- [ ] Verifiquei que `.local.json` está no `.gitignore`
- [ ] Commitei as mudanças (removendo segredos)
- [ ] Sistema funcionando com novas credenciais

---

## 📚 Boas Práticas de Segurança

1. **NUNCA** commitar senhas, tokens, ou chaves de API
2. Usar variáveis de ambiente ou arquivos `.local.json`
3. Sempre adicionar arquivos de segredos no `.gitignore`
4. Usar Azure Key Vault ou AWS Secrets Manager em produção
5. Rotacionar credenciais periodicamente

---

**Data do incidente:** 06/11/2025  
**Segredos comprometidos:** JWT Key, Gmail App Password  
**Status:** 🔴 REQUER AÇÃO IMEDIATA
