# 3. DESENVOLVIMENTO DE SOFTWARE PARA INTERNET

## 3.1 Fundamentação Teórica

### 3.1.1 Desenvolvimento Web Moderno

O desenvolvimento web moderno é caracterizado por uma evolução significativa em relação às aplicações tradicionais. As Single Page Applications (SPA) representam aplicações que carregam uma única página HTML inicial e atualizam o conteúdo dinamicamente via JavaScript, proporcionando experiência mais fluida e responsiva ao usuário. A arquitetura REST estabelece comunicação stateless entre cliente e servidor através do protocolo HTTP, utilizando verbos padrão (GET, POST, PUT, DELETE) para operações sobre recursos. A responsividade tornou-se requisito fundamental, exigindo interfaces que se adaptam automaticamente a diferentes tamanhos de tela, desde smartphones até monitores widescreen. Por fim, as Progressive Web Apps (PWAs) combinam o melhor dos mundos web e mobile, oferecendo capacidades de aplicações nativas como funcionamento offline e notificações push, porém acessíveis diretamente pelo navegador.

### 3.1.2 React - Biblioteca JavaScript

React é uma biblioteca JavaScript para construção de interfaces de usuário, desenvolvida pelo Facebook. A biblioteca adota uma abordagem baseada em componentes (Component-Based), onde a interface é dividida em blocos reutilizáveis e compostos, facilitando manutenção e reuso de código. O Virtual DOM é uma abstração em memória do DOM real que permite ao React calcular diferenças e atualizar apenas os elementos necessários, proporcionando performance superior. O fluxo unidirecional de dados (Unidirectional Data Flow) garante previsibilidade, com dados fluindo sempre do componente pai para os filhos através de props. JSX é uma extensão de sintaxe que permite escrever código semelhante a HTML dentro de JavaScript, tornando a criação de interfaces mais intuitiva e declarativa.

### 3.1.3 ASP.NET Core Web API

ASP.NET Core é um framework multiplataforma para construção de aplicações web modernas. Como Web API, oferece suporte completo a serviços RESTful, com endpoints HTTP que seguem os princípios e convenções REST para manipulação de recursos. As Minimal APIs introduzidas no .NET 6+ permitem criação rápida de APIs com código mínimo, ideal para microsserviços e protótipos. O Middleware Pipeline processa requisições HTTP em camadas sequenciais, permitindo adicionar funcionalidades como autenticação, logging e tratamento de erros de forma modular. A Dependency Injection é nativa do framework, promovendo inversão de controle e facilitando testabilidade sem necessidade de bibliotecas externas.

## 3.2 Tecnologias Adotadas no Projeto Web

### 3.2.1 Stack Frontend

- **Framework:** React 18.3
- **Linguagem:** TypeScript 5.x
- **Estilização:** TailwindCSS 3.x
- **State Management:** React Hooks (useState, useEffect, useContext)
- **HTTP Client:** Axios
- **Roteamento:** React Router v6
- **Comunicação Real-Time:** SignalR Client

### 3.2.2 Stack Backend

- **Framework:** ASP.NET Core 8.0
- **Linguagem:** C# 12
- **Arquitetura:** REST API + SignalR Hubs
- **Autenticação:** JWT (JSON Web Tokens)
- **Documentação:** Swagger/OpenAPI
- **ORM:** Entity Framework Core 8.0

### 3.2.3 Arquitetura da Aplicação Web

```
┌──────────────────────────────────────────────────┐
│           Frontend (React + TypeScript)          │
│  ┌────────────────────────────────────────────┐  │
│  │  Components (UI)                           │  │
│  │  ├─ Pages                                  │  │
│  │  ├─ Layouts                                │  │
│  │  └─ Shared Components                      │  │
│  └────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────┐  │
│  │  Services (HTTP + SignalR)                 │  │
│  └────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────┘
                       ↕ HTTP/WebSocket
┌──────────────────────────────────────────────────┐
│         Backend (ASP.NET Core Web API)           │
│  ┌────────────────────────────────────────────┐  │
│  │  Controllers (Endpoints REST)              │  │
│  └────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────┐  │
│  │  Services (Business Logic)                 │  │
│  └────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────┐  │
│  │  Repositories (Data Access)                │  │
│  └────────────────────────────────────────────┘  │
│  ┌────────────────────────────────────────────┐  │
│  │  SignalR Hubs (Real-Time)                  │  │
│  └────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────┘
                       ↕
┌──────────────────────────────────────────────────┐
│            SQL Server Database                   │
└──────────────────────────────────────────────────┘
```

**Foto: Arquitetura da aplicação web**

## 3.3 Sistema Web Responsivo

### 3.3.1 Design Responsivo com TailwindCSS

O TailwindCSS é um framework CSS utility-first que permite criar layouts responsivos rapidamente:

```jsx
// Exemplo de componente responsivo
<div className="
  grid 
  grid-cols-1        // 1 coluna em mobile
  md:grid-cols-2     // 2 colunas em tablet
  lg:grid-cols-3     // 3 colunas em desktop
  gap-4 
  p-4
">
  {chamados.map(chamado => (
    <ChamadoCard key={chamado.id} chamado={chamado} />
  ))}
</div>
```

### 3.3.2 Breakpoints Utilizados

| Dispositivo | Largura | Breakpoint |
|-------------|---------|------------|
| Mobile | < 768px | (padrão) |
| Tablet | 768px - 1023px | `md:` |
| Desktop | 1024px+ | `lg:` |
| Large Desktop | 1280px+ | `xl:` |

No Caju Ajuda, diversos componentes responsivos foram implementados para garantir experiência consistente em todos os dispositivos. A Navbar (barra de navegação) utiliza menu hambúrguer em dispositivos móveis e apresentação horizontal em desktop, economizando espaço de tela. A Sidebar aparece como drawer lateral deslizante em mobile e permanece fixa na lateral em desktop, facilitando navegação. As tabelas se transformam em cards empilhados em mobile, onde espaço horizontal é limitado, retornando ao formato tradicional de tabela em desktop. Os formulários organizam campos verticalmente em mobile e utilizam grid de múltiplas colunas em desktop, otimizando preenchimento. Por fim, os modais ocupam tela inteira em dispositivos móveis para melhor visualização e aparecem centralizados sobre overlay em desktop.  

**Foto: Interface web em diferentes resoluções (mobile, tablet, desktop)**

## 3.4 Mecanismos de Segurança

### 3.4.1 Autenticação JWT

JSON Web Token (JWT) é um padrão aberto RFC 7519 para transmissão segura de informações entre partes como um objeto JSON assinado digitalmente (JONES; BRADLEY; SAKIMURA, 2015). A implementação de JWT garante autenticação stateless e escalável.

#### Fluxo de Autenticação:

```
1. Cliente envia credenciais (email + senha)
        ↓
2. Servidor valida credenciais
        ↓
3. Servidor gera JWT com dados do usuário
        ↓
4. Cliente armazena JWT (localStorage)
        ↓
5. Cliente envia JWT no header em cada requisição
        ↓
6. Servidor valida JWT e autoriza acesso
```

#### Implementação Backend:

```csharp
// Geração do Token
public string GenerateToken(Usuario user)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
    var token = new JwtSecurityToken(
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        expires: DateTime.Now.AddHours(8),
        signingCredentials: creds
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

#### Implementação Frontend:

```typescript
// AuthService.ts
export class AuthService {
  static async login(email: string, senha: string): Promise<LoginResponse> {
    const response = await api.post('/api/auth/login', { email, senha });
    
    // Armazenar token
    localStorage.setItem('token', response.data.token);
    localStorage.setItem('user', JSON.stringify(response.data.user));
    
    return response.data;
  }
  
  static getToken(): string | null {
    return localStorage.getItem('token');
  }
  
  static logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }
}
```

### 3.4.2 Autorização Baseada em Roles

O sistema implementa **3 perfis de usuário**:

| Role | Permissões |
|------|-----------|
| **CLIENTE** | Criar chamados, enviar mensagens, visualizar próprios chamados |
| **TECNICO** | Visualizar todos chamados, atribuir chamados, atualizar status, enviar notas internas |
| **ADMIN** | Todas as permissões + gerenciar usuários, acessar dashboard, excluir chamados |

#### Proteção de Rotas (Frontend):

```typescript
// ProtectedRoute.tsx
const ProtectedRoute = ({ requiredRoles, children }) => {
  const user = AuthService.getCurrentUser();
  
  if (!user) {
    return <Navigate to="/login" />;
  }
  
  if (requiredRoles && !requiredRoles.includes(user.role)) {
    return <Navigate to="/unauthorized" />;
  }
  
  return <>{children}</>;
};

// Uso
<Route path="/admin/dashboard" element={
  <ProtectedRoute requiredRoles={['ADMIN']}>
    <DashboardPage />
  </ProtectedRoute>
} />
```

#### Proteção de Endpoints (Backend):

```csharp
[Authorize(Roles = "TECNICO,ADMIN")]
[HttpPut("{id}/atribuir")]
public async Task<IActionResult> AtribuirTecnico(int id, [FromBody] AtribuirDto dto)
{
    await _chamadoService.AtribuirTecnicoAsync(id, dto.TecnicoId);
    return Ok();
}
```

O sistema implementa proteção robusta contra os ataques mais comuns à segurança de aplicações web. SQL Injection é prevenido através do uso de Entity Framework Core, que utiliza queries parametrizadas automaticamente, evitando concatenação direta de strings SQL. Ataques de XSS (Cross-Site Scripting) são mitigados pelo React, que escapa automaticamente todo conteúdo renderizado, impedindo execução de scripts maliciosos injetados. CSRF (Cross-Site Request Forgery) é neutralizado pela utilização de token JWT no header Authorization ao invés de cookies, eliminando o vetor de ataque. Senhas nunca são armazenadas em texto plano, sendo sempre processadas com hash bcrypt incluindo salt único por usuário. Por fim, a exposição de dados sensíveis é controlada através de DTOs (Data Transfer Objects) que definem explicitamente quais campos são retornados pela API, evitando vazamento de informações confidenciais.  

## 3.5 Testes de Usabilidade

### 3.5.1 Metodologia

Foram realizados testes de usabilidade com **5 usuários representativos** (2 clientes, 2 técnicos, 1 administrador), utilizando protótipos de alta fidelidade.

### 3.5.2 Tarefas Avaliadas

| # | Tarefa | Perfil | Sucesso |
|---|--------|--------|---------|
| 1 | Fazer login no sistema | Todos | 100% |
| 2 | Criar novo chamado | Cliente | 100% |
| 3 | Anexar arquivo ao chamado | Cliente | 80% |
| 4 | Enviar mensagem no chat | Cliente | 100% |
| 5 | Visualizar lista de chamados | Técnico | 100% |
| 6 | Atribuir chamado a técnico | Técnico | 80% |
| 7 | Atualizar status do chamado | Técnico | 100% |
| 8 | Acessar dashboard | Admin | 100% |

### 3.5.3 Métricas Coletadas

- **Taxa de Sucesso:** 95% (média)
- **Tempo Médio de Conclusão:** 32 segundos por tarefa
- **Número de Erros:** 0,4 erros por tarefa
- **Satisfação (escala 1-5):** 4,6

### 3.5.4 Principais Feedbacks

Os usuários identificaram diversos aspectos positivos durante os testes. A interface foi considerada limpa e intuitiva, facilitando o aprendizado e uso do sistema. O chat em tempo real impressionou pela fluidez e ausência de atrasos perceptíveis. O sistema de cores para identificar prioridades (vermelho para alta, amarelo para média, azul para baixa) foi elogiado por facilitar triagem visual rápida. A performance geral do sistema foi avaliada como rápida e responsiva, sem travamentos ou lentidão.

Entre os pontos de melhoria identificados, destaca-se que o upload de arquivos foi considerado funcional mas poderia ser mais intuitivo com suporte a drag-and-drop (arrastar e soltar). Em dispositivos móveis, o botão "Atribuir técnico" estava pouco visível, dificultando acesso rápido à funcionalidade. Adicionalmente, usuários sentiram falta de notificação visual imediata quando uma mensagem era enviada com sucesso.

### 3.5.5 Melhorias Implementadas

Após os testes, foram implementadas as seguintes melhorias baseadas no feedback dos usuários. A funcionalidade de Drag-and-Drop para upload foi adicionada, criando uma área destacada onde usuários podem arrastar arquivos diretamente do sistema operacional. Toast Notifications foram implementadas para fornecer feedback visual imediato após ações como envio de mensagem, atualização de status ou exclusão de anexos. Loading States foram aprimorados, adicionando indicadores de carregamento (spinners) durante operações assíncronas para informar que o sistema está processando. A hierarquia visual de botões foi melhorada em dispositivos móveis, tornando ações primárias mais destacadas e acessíveis. Por fim, modais de confirmação foram adicionados antes de ações críticas como exclusão de chamados ou usuários, prevenindo remoções acidentais.

**Foto: Antes e depois das melhorias de usabilidade**

## 3.6 Principais Páginas Implementadas

### 3.6.1 Página de Login
- Formulário de autenticação
- Validação client-side e server-side
- Link para registro de novos usuários
- Recuperação de senha (planejado)

### 3.6.2 Dashboard (Admin)
- Gráficos de chamados por status
- Gráficos de chamados por prioridade
- Métricas: Total de chamados, Taxa de resolução, Tempo médio
- Lista de técnicos com performance

**Foto: Dashboard administrativo**

### 3.6.3 Lista de Chamados
- Filtros por status, prioridade, técnico
- Paginação (10 itens por página)
- Busca por título/descrição
- Badges coloridos para status e prioridade

**Foto: Lista de chamados com filtros**

### 3.6.4 Detalhes do Chamado
- Informações completas do chamado
- Chat em tempo real
- Upload de anexos
- Histórico de mudanças de status
- Avaliação do atendimento (cliente)

**Foto: Tela de detalhes do chamado com chat**

### 3.6.5 Novo Chamado
- Formulário com validação
- Campo de título e descrição
- Seleção de prioridade
- Upload de anexos (opcional)
- Sugestão de prioridade via IA

## 3.7 Comunicação em Tempo Real (SignalR)

### 3.7.1 O que é SignalR?

SignalR é uma biblioteca ASP.NET Core que facilita a adição de funcionalidades web em tempo real. Utiliza WebSockets quando disponível, com fallback para Server-Sent Events ou Long Polling.

### 3.7.2 Hub de Notificações

```csharp
  // Backend - NotificacaoHub.cs
  public class NotificacaoHub : Hub
  {
      public async Task EntrarNoChamado(string chamadoId)
      {
          await Groups.AddToGroupAsync(Context.ConnectionId, $"Chamado_{chamadoId}");
      }
      
      public async Task EnviarMensagem(int chamadoId, string conteudo)
      {
          // Processar mensagem...
          await Clients.Group($"Chamado_{chamadoId}")
              .SendAsync("NovaMensagem", mensagem);
      }
  }
  ```

  ```typescript
  // Frontend - signalRService.ts
  import * as signalR from '@microsoft/signalr';

  class SignalRService {
    private connection: signalR.HubConnection;
    
    async connect(token: string) {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl('/notificacaoHub', {
          accessTokenFactory: () => token
        })
        .build();
      
      await this.connection.start();
    }
    
    onNovaMensagem(callback: (mensagem: Mensagem) => void) {
      this.connection.on('NovaMensagem', callback);
    }
  }
```

## 3.8 API REST Documentada

### 3.8.1 Endpoints Principais

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/login` | Autenticar usuário | Não |
| POST | `/api/auth/register/cliente` | Registrar cliente | Não |
| GET | `/api/chamados` | Listar chamados | Sim |
| GET | `/api/chamados/{id}` | Detalhes do chamado | Sim |
| POST | `/api/chamados` | Criar chamado | Sim |
| PUT | `/api/chamados/{id}/status` | Atualizar status | Sim |
| POST | `/api/chamados/{id}/mensagens` | Enviar mensagem | Sim |
| POST | `/api/chamados/{id}/anexos` | Upload de anexo | Sim |

### 3.8.2 Documentação Swagger

O sistema utiliza Swagger/OpenAPI para documentação interativa da API, oferecendo diversos recursos essenciais para desenvolvedores e integradores. A interface apresenta descrição detalhada de cada endpoint, incluindo propósito e comportamento esperado. Todos os parâmetros de URL, query string e body são documentados com tipos de dados e obrigatoriedade. Os schemas completos de DTOs são exibidos, mostrando estrutura de requisições e respostas. Uma interface interativa permite testar endpoints diretamente no navegador, facilitando desenvolvimento e debugging. Por fim, o sistema possui suporte integrado para autenticação JWT, permitindo adicionar o token Bearer diretamente na interface do Swagger.

**Foto: Interface do Swagger UI**

## 3.9 Conclusão do Capítulo

A aplicação web desenvolvida atende plenamente aos requisitos da disciplina de Desenvolvimento de Software para Internet. Foi implementado um sistema web responsivo com interface adaptativa que funciona perfeitamente em dispositivos móveis, tablets e desktops. A segurança é garantida através de autenticação JWT e controle de acesso baseado em roles (CLIENTE, TECNICO, ADMIN), protegendo recursos sensíveis. Testes de usabilidade foram realizados com cinco usuários reais representando os três perfis do sistema, alcançando taxa de sucesso de 95% e satisfação média de 4,6 em escala de 1 a 5. Tecnologias modernas e consolidadas foram adotadas, incluindo React 18 e TypeScript no frontend, ASP.NET Core 8 no backend, e SignalR para comunicação em tempo real. A API REST foi completamente documentada através do Swagger/OpenAPI, expondo mais de 40 endpoints com descrições detalhadas e interface de testes. Por fim, a comunicação em tempo real foi implementada com sucesso utilizando SignalR, permitindo que mensagens de chat sejam entregues instantaneamente entre usuários conectados.  

**Foto: Demonstração do sistema web funcionando em diferentes dispositivos**

---
