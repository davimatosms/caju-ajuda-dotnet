# 🚀 Deploy do Caju Ajuda no Azure

Este guia detalha como fazer o deploy completo da aplicação Caju Ajuda no Microsoft Azure.

## 📋 Pré-requisitos

- Conta Microsoft Azure ativa
- Repositório GitHub configurado
- Azure CLI instalada (opcional, mas recomendado)

## 🏗️ Arquitetura de Deploy

- **Backend (.NET)**: Azure App Service (Linux)
- **Frontend (React)**: Azure Static Web Apps
- **Banco de Dados**: Azure SQL Database
- **CI/CD**: GitHub Actions

---

## 1️⃣ Configurar Recursos no Azure Portal

### 1.1 Criar Resource Group

1. Acesse o [Azure Portal](https://portal.azure.com)
2. Navegue para **Resource Groups** > **Create**
3. Preencha:
   - **Resource group name**: `rg-cajuajuda`
   - **Region**: `Brazil South` (ou sua preferência)
4. Clique em **Review + create** > **Create**

### 1.2 Criar Azure SQL Database

1. No Azure Portal, navegue para **SQL databases** > **Create**
2. Preencha:
   - **Resource group**: `rg-cajuajuda`
   - **Database name**: `cajuajuda-db`
   - **Server**: Crie um novo servidor
     - **Server name**: `cajuajuda-sqlserver` (deve ser único globalmente)
     - **Location**: `Brazil South`
     - **Authentication method**: SQL authentication
     - **Server admin login**: `cajuadmin`
     - **Password**: (escolha uma senha forte)
   - **Compute + storage**: Basic (para começar) ou Standard S0
3. Em **Networking**:
   - **Connectivity method**: Public endpoint
   - ✅ Allow Azure services to access server
   - ✅ Add current client IP address
4. Clique em **Review + create** > **Create**
5. **Salve a connection string** (será necessária depois):
   ```
   Server=tcp:cajuajuda-sqlserver.database.windows.net,1433;Initial Catalog=cajuajuda-db;Persist Security Info=False;User ID=cajuadmin;Password={sua_senha};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

### 1.3 Criar Azure App Service (Backend)

1. No Azure Portal, navegue para **App Services** > **Create**
2. Preencha:
   - **Resource group**: `rg-cajuajuda`
   - **Name**: `cajuajuda-backend` (deve ser único globalmente)
   - **Publish**: Code
   - **Runtime stack**: .NET 8 (LTS)
   - **Operating System**: Linux
   - **Region**: `Brazil South`
   - **Pricing plan**: B1 (Basic) ou superior
3. Clique em **Review + create** > **Create**
4. Após criado, vá para o recurso e navegue para **Configuration** > **Application settings**
5. Adicione as seguintes variáveis de ambiente:

   | Name | Value |
   |------|-------|
   | `AZURE_SQL_CONNECTION_STRING` | (cole a connection string do SQL Database) |
   | `FRONTEND_URL` | `https://cajuajuda-frontend.azurestaticapps.net` (será criado depois) |
   | `JWT_SECRET_KEY` | `CajuAjuda_UmaChaveSuperSecretaELongaParaAssinarNossosTokens_2025` |
   | `SMTP_USERNAME` | `suportecajuajuda@gmail.com` |
   | `SMTP_PASSWORD` | `npll uuyn uqvt aozs` |
   | `SMTP_SENDER_EMAIL` | `suportecajuajuda@gmail.com` |

6. Clique em **Save**
7. Navegue para **Deployment Center** > **Settings**
8. Baixe o **Publish Profile** (botão "Download publish profile" no topo)
9. **Salve este arquivo XML** (será usado no GitHub Secrets)

### 1.4 Criar Azure Static Web App (Frontend)

1. No Azure Portal, navegue para **Static Web Apps** > **Create**
2. Preencha:
   - **Resource group**: `rg-cajuajuda`
   - **Name**: `cajuajuda-frontend`
   - **Plan type**: Free
   - **Region**: `East US 2` (Static Web Apps tem regiões limitadas)
   - **Deployment details**: GitHub
     - Conecte sua conta GitHub
     - Selecione **Organization**: seu usuário
     - **Repository**: `caju-ajuda-dotnet`
     - **Branch**: `main`
   - **Build details**:
     - **Build presets**: React
     - **App location**: `/src/web`
     - **Output location**: `build`
3. Clique em **Review + create** > **Create**
4. Após criado, navegue para **Configuration** > **Application settings**
5. Adicione:
   | Name | Value |
   |------|-------|
   | `REACT_APP_API_URL` | `https://cajuajuda-backend.azurewebsites.net` |
6. Clique em **Save**
7. Copie a **URL** do Static Web App (ex: `https://cajuajuda-frontend.azurestaticapps.net`)
8. **Volte ao App Service do backend** e atualize a variável `FRONTEND_URL` com esta URL

---

## 2️⃣ Configurar GitHub Secrets

1. No GitHub, vá para o repositório `caju-ajuda-dotnet`
2. Navegue para **Settings** > **Secrets and variables** > **Actions**
3. Clique em **New repository secret** e adicione os seguintes secrets:

### Secrets Necessários:

| Secret Name | Descrição | Onde Obter |
|-------------|-----------|------------|
| `AZURE_BACKEND_PUBLISH_PROFILE` | Perfil de publicação do App Service | Baixado do App Service > Deployment Center |
| `AZURE_STATIC_WEB_APPS_API_TOKEN` | Token do Static Web App | Static Web App > Overview > "Manage deployment token" |
| `REACT_APP_API_URL` | URL da API no Azure | `https://cajuajuda-backend.azurewebsites.net` |

**Como adicionar cada secret:**
1. Clique em **New repository secret**
2. **Name**: (nome do secret da tabela acima)
3. **Value**: (valor correspondente)
4. Clique em **Add secret**

---

## 3️⃣ Fazer o Deploy

### Opção A: Deploy Automático via Push

1. Commit e push das alterações:
   ```bash
   git add .
   git commit -m "chore: configurar deploy Azure"
   git push origin main
   ```

2. O GitHub Actions será automaticamente acionado
3. Acompanhe o progresso em **Actions** no GitHub

### Opção B: Deploy Manual

1. No GitHub, vá para **Actions**
2. Selecione o workflow desejado:
   - `Deploy Backend to Azure`
   - `Deploy Frontend to Azure`
3. Clique em **Run workflow** > **Run workflow**

---

## 4️⃣ Verificar o Deploy

### Backend
1. Acesse: `https://cajuajuda-backend.azurewebsites.net/swagger`
2. Teste o endpoint de login: `POST /api/auth/login`
   ```json
   {
     "email": "admin@cajuajuda.com",
     "senha": "Admin@2025"
   }
   ```

### Frontend
1. Acesse: `https://cajuajuda-frontend.azurestaticapps.net`
2. Faça login com:
   - **Email**: admin@cajuajuda.com
   - **Senha**: Admin@2025

---

## 5️⃣ Executar Migrations no Azure

Após o primeiro deploy, execute as migrations para criar as tabelas:

### Opção 1: Via Azure Portal (Console do App Service)

1. No App Service, vá para **Console** (no menu lateral)
2. Execute:
   ```bash
   cd /home/site/wwwroot
   dotnet ef database update
   ```

### Opção 2: Localmente (requer Azure CLI)

```bash
# Instalar ferramentas EF
dotnet tool install --global dotnet-ef

# Executar migrations
cd src/backend
dotnet ef database update --connection "Server=tcp:cajuajuda-sqlserver.database.windows.net,1433;Initial Catalog=cajuajuda-db;User ID=cajuadmin;Password={sua_senha};Encrypt=True;"
```

---

## 🔧 Configurações CORS

O backend já está configurado para aceitar requisições do frontend. Se precisar ajustar:

1. No App Service, vá para **CORS**
2. Adicione a URL do Static Web App: `https://cajuajuda-frontend.azurestaticapps.net`
3. Salve

---

## 📊 Monitoramento

### Application Insights (Recomendado)

1. No Azure Portal, crie um **Application Insights**
2. Conecte ao App Service em **Settings** > **Application Insights**
3. Monitore logs, performance e erros em tempo real

### Logs do App Service

1. No App Service, vá para **Log stream**
2. Veja logs em tempo real da aplicação

---

## 💰 Estimativa de Custos (Plano Básico)

| Recurso | Plano | Custo Mensal (USD) |
|---------|-------|-------------------|
| App Service (B1) | Basic | ~$13 |
| SQL Database (Basic) | Basic | ~$5 |
| Static Web App | Free | $0 |
| **TOTAL** | | **~$18/mês** |

*Custos podem variar. Para produção, considere planos superiores.*

---

## 🚨 Troubleshooting

### Backend não inicia
- ✅ Verifique se as variáveis de ambiente estão configuradas
- ✅ Verifique os logs no Log Stream
- ✅ Confirme que o .NET 8 está selecionado no App Service

### Frontend não carrega
- ✅ Verifique se `REACT_APP_API_URL` está correto
- ✅ Confirme que o build foi concluído com sucesso no GitHub Actions
- ✅ Verifique CORS no backend

### Erro de banco de dados
- ✅ Confirme que o firewall do SQL Server permite conexões do Azure
- ✅ Execute as migrations manualmente
- ✅ Verifique a connection string

### 401 Unauthorized
- ✅ Verifique se o JWT_SECRET_KEY é o mesmo no backend
- ✅ Confirme que as migrations criaram os usuários padrão
- ✅ Teste login diretamente no Swagger

---

## 🔐 Segurança em Produção

⚠️ **IMPORTANTE**: Antes de ir para produção:

1. ✅ Gere um novo `JWT_SECRET_KEY` forte e único
2. ✅ Configure SMTP com credenciais de produção
3. ✅ Ative HTTPS em todos os endpoints
4. ✅ Configure Azure Key Vault para secrets sensíveis
5. ✅ Habilite Azure Active Directory para autenticação
6. ✅ Configure backups automáticos do SQL Database
7. ✅ Implemente rate limiting no App Service
8. ✅ Configure alertas no Application Insights

---

## 📚 Referências

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure Static Web Apps Documentation](https://docs.microsoft.com/azure/static-web-apps/)
- [Azure SQL Database Documentation](https://docs.microsoft.com/azure/azure-sql/)
- [GitHub Actions for Azure](https://docs.microsoft.com/azure/developer/github/github-actions)

---

## ✅ Checklist Final

Antes de considerar o deploy completo:

- [ ] Backend acessível via Swagger
- [ ] Frontend carregando corretamente
- [ ] Login funcionando com usuários seed
- [ ] Criação de chamados funcionando
- [ ] Upload de anexos funcionando
- [ ] SignalR conectando (notificações em tempo real)
- [ ] CORS configurado corretamente
- [ ] Migrations aplicadas
- [ ] Variáveis de ambiente configuradas
- [ ] GitHub Actions executando sem erros
- [ ] Application Insights configurado (opcional)
- [ ] Backups configurados

---

**Deploy realizado com sucesso! 🎉**

Se tiver problemas, consulte a seção de Troubleshooting ou abra uma issue no GitHub.
