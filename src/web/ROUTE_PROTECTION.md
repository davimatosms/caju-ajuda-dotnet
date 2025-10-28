# Sistema de Proteção de Rotas 🔒

## Visão Geral

O sistema implementa proteção de rotas em dois níveis:

### 1. ProtectedRoute (Rotas Privadas)
Protege rotas que requerem autenticação e/ou roles específicos.

**Funcionalidades:**
- ✅ Verifica se o usuário está autenticado (token JWT válido)
- ✅ Valida se o usuário tem a role necessária
- ✅ Redireciona para `/login` se não estiver autenticado
- ✅ Redireciona para a página apropriada se não tiver permissão

### 2. PublicRoute (Rotas Públicas)
Protege rotas de login/registro para evitar acesso duplicado.

**Funcionalidades:**
- ✅ Redireciona usuários já autenticados para o dashboard apropriado
- ✅ Permite acesso apenas a usuários não autenticados

## Estrutura de Roles

### CLIENTE
- Acesso às rotas:
  - `/dashboard` - Lista de chamados
  - `/chamados/novo` - Criar novo chamado
  - `/chamado/:id` - Detalhes do chamado
  - `/perfil` - Perfil do usuário

### TECNICO
- Acesso às rotas de CLIENTE +
- Acesso às rotas administrativas:
  - `/admin` - Home do admin
  - `/admin/dashboard` - Dashboard administrativo
  - `/admin/tecnicos` - Gerenciar técnicos
  - `/admin/clientes` - Gerenciar clientes

### ADMIN
- Acesso total a todas as rotas

## Implementação

### App.tsx
```tsx
// Rotas Privadas - Requer Autenticação
<Route element={<ProtectedRoute allowedRoles={['CLIENTE', 'TECNICO', 'ADMIN']} />}>
  <Route element={<Layout />}>
    <Route path="/dashboard" element={<DashboardPage />} />
  </Route>
</Route>

// Rotas Administrativas - Requer ADMIN ou TECNICO
<Route element={<ProtectedRoute allowedRoles={['ADMIN', 'TECNICO']} />}>
  <Route element={<Layout />}>
    <Route path="/admin" element={<AdminHomePage />} />
  </Route>
</Route>

// Rotas Públicas - Redireciona se já autenticado
<Route path="/login" element={<PublicRoute><LoginPage /></PublicRoute>} />
```

## Fluxo de Autenticação

### Login Bem-Sucedido
1. Usuário faz login
2. Backend retorna JWT token
3. Token é salvo no `localStorage` como `user_token`
4. Token é decodificado para obter informações do usuário (email, role)
5. Usuário é redirecionado para o dashboard apropriado

### Tentativa de Acesso Sem Autenticação
1. Usuário tenta acessar `/dashboard` sem estar logado
2. `ProtectedRoute` detecta ausência de token
3. Console mostra: `🚫 Acesso negado: Usuário não autenticado`
4. Usuário é redirecionado para `/login`

### Tentativa de Acesso Sem Permissão
1. CLIENTE tenta acessar `/admin`
2. `ProtectedRoute` detecta role inválida
3. Console mostra: `🚫 Acesso negado: Role "CLIENTE" não autorizada`
4. Usuário é redirecionado para `/dashboard`

### Logout
1. Usuário clica em "Sair"
2. `AuthService.logout()` é chamado
3. `localStorage` é completamente limpo
4. Usuário é redirecionado para `/login`

## Segurança

### Frontend (React)
- ✅ Proteção de rotas com React Router
- ✅ Validação de JWT client-side
- ✅ Redirecionamento automático baseado em role
- ⚠️ **Nota:** Proteção frontend é apenas UX, não segurança real

### Backend (API)
- ✅ Autenticação JWT obrigatória
- ✅ Atributo `[Authorize]` em controllers
- ✅ Validação de roles: `[Authorize(Roles = "ADMIN, TECNICO")]`
- ✅ Middleware de autenticação valida todos os requests

## Testando a Proteção

### Teste 1: Acesso sem login
```
1. Limpe o localStorage (F12 → Application → Clear Storage)
2. Tente acessar http://localhost:3000/dashboard
3. ✅ Deve redirecionar para /login
```

### Teste 2: Acesso com role inadequada
```
1. Faça login como CLIENTE
2. Tente acessar http://localhost:3000/admin
3. ✅ Deve redirecionar para /dashboard
```

### Teste 3: Login duplicado
```
1. Faça login normalmente
2. Tente acessar http://localhost:3000/login
3. ✅ Deve redirecionar para /dashboard
```

### Teste 4: Logout
```
1. Estando logado, clique em "Sair"
2. Verifique localStorage (deve estar vazio)
3. Tente acessar /dashboard
4. ✅ Deve redirecionar para /login
```

## Arquivos Modificados

- `src/App.tsx` - Configuração de rotas com proteção
- `src/components/ProtectedRoute/ProtectedRoute.tsx` - Componente de proteção
- `src/components/PublicRoute/PublicRoute.tsx` - Proteção de rotas públicas
- `src/pages/NotFoundPage/NotFoundPage.tsx` - Página 404
- `src/services/AuthService.ts` - Serviço de autenticação melhorado

## Console Logs (Debug)

Os componentes de proteção emitem logs úteis:

- `🚫 Acesso negado: Usuário não autenticado`
- `🚫 Acesso negado: Role "X" não autorizada. Requer: Y, Z`

Esses logs ajudam no debug durante desenvolvimento.
