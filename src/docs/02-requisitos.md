# 2. REQUISITOS DO SISTEMA CAJUAJUDA

## 2.1 LISTA DE REQUISITOS FUNCIONAIS (RF)

### RF01 - Autenticação e Controle de Acesso
**Descrição**: O sistema deve permitir autenticação segura de usuários com diferentes perfis de acesso.

**Regras de Negócio**:
- RF01.1: Sistema deve validar email e senha no login
- RF01.2: Sistema deve gerar token JWT válido por 24 horas
- RF01.3: Sistema deve diferenciar perfis (Cliente, Técnico, Administrador)
- RF01.4: Sistema deve permitir registro de novos clientes
- RF01.5: Sistema deve armazenar senhas com hash bcrypt

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RF02 - Gerenciamento de Chamados
**Descrição**: O sistema deve permitir criação e gerenciamento completo de chamados de suporte.

**Regras de Negócio**:
- RF02.1: Cliente pode criar chamados com título e descrição
- RF02.2: Sistema deve permitir definir prioridade (Baixa, Normal, Alta, Urgente)
- RF02.3: Sistema deve registrar data/hora de abertura automaticamente
- RF02.4: Sistema deve permitir anexar arquivos na criação (opcional)
- RF02.5: Chamado deve ter status inicial "Aberto"

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RF03 - Listagem e Filtragem de Chamados
**Descrição**: O sistema deve listar chamados com filtros por status e responsável.

**Regras de Negócio**:
- RF03.1: Técnico pode ver chamados disponíveis (sem técnico atribuído)
- RF03.2: Técnico pode ver seus chamados em andamento
- RF03.3: Técnico pode ver chamados fechados
- RF03.4: Cliente pode ver apenas seus próprios chamados
- RF03.5: Sistema deve ordenar por data de abertura (mais recente primeiro)

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RF04 - Atribuição de Técnico
**Descrição**: O sistema deve permitir atribuir técnicos aos chamados.

**Regras de Negócio**:
- RF04.1: Apenas técnicos podem se atribuir a chamados
- RF04.2: Ao atribuir, status deve mudar automaticamente para "Em Andamento"
- RF04.3: Um chamado pode ter apenas um técnico responsável
- RF04.4: Sistema deve registrar data/hora da atribuição

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RF05 - Sistema de Mensagens (Chat)
**Descrição**: O sistema deve permitir comunicação via chat entre cliente e técnico.

**Regras de Negócio**:
- RF05.1: Cliente e técnico podem enviar mensagens de texto
- RF05.2: Sistema deve exibir nome do autor e data/hora
- RF05.3: Mensagens devem aparecer em tempo real (SignalR no Web)
- RF05.4: Desktop deve atualizar mensagens automaticamente
- RF05.5: Sistema deve manter histórico completo de mensagens

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RF06 - Notas Internas
**Descrição**: O sistema deve permitir técnicos criarem notas internas não visíveis ao cliente.

**Regras de Negócio**:
- RF06.1: Apenas técnicos podem criar notas internas
- RF06.2: Notas internas não são visíveis para clientes
- RF06.3: Notas internas aparecem destacadas visualmente
- RF06.4: Sistema deve marcar claramente "Nota Interna"

**Prioridade**: Média  
**Status**: ✅ Implementado

---

### RF07 - Anexo de Arquivos
**Descrição**: O sistema deve permitir upload e download de arquivos.

**Regras de Negócio**:
- RF07.1: Sistema deve aceitar PDF, DOC, DOCX, JPG, PNG, GIF
- RF07.2: Tamanho máximo por arquivo: 10MB
- RF07.3: Arquivos devem ser vinculados a mensagens ou chamados
- RF07.4: Sistema deve exibir ícone conforme tipo de arquivo
- RF07.5: Sistema deve permitir download seguro

**Prioridade**: Média  
**Status**: ✅ Implementado

---

### RF08 - Atualização de Status
**Descrição**: O sistema deve permitir atualizar status dos chamados.

**Regras de Negócio**:
- RF08.1: Técnico pode alterar status para "Aguardando Cliente"
- RF08.2: Técnico pode alterar status para "Resolvido"
- RF08.3: Técnico pode fechar chamado (status "Fechado")
- RF08.4: Sistema deve registrar data/hora de cada mudança
- RF08.5: Cliente não pode alterar status

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RF09 - Perfil do Usuário
**Descrição**: O sistema deve permitir visualizar e editar dados do perfil.

**Regras de Negócio**:
- RF09.1: Usuário pode visualizar seus dados (nome, email, tipo de conta)
- RF09.2: Usuário pode editar nome e email
- RF09.3: Usuário pode alterar senha (validando senha atual)
- RF09.4: Sistema deve validar força da nova senha (mínimo 6 caracteres)

**Prioridade**: Média  
**Status**: ✅ Implementado

---

### RF10 - Dashboard
**Descrição**: O sistema deve exibir resumo de chamados na tela principal.

**Regras de Negócio**:
- RF10.1: Dashboard deve mostrar total de chamados por status
- RF10.2: Dashboard deve exibir chamados recentes
- RF10.3: Cliente vê apenas seus chamados
- RF10.4: Técnico vê todos os chamados

**Prioridade**: Baixa  
**Status**: ✅ Implementado

---

## 2.2 LISTA DE REQUISITOS NÃO FUNCIONAIS (RNF)

### RNF01 - Desempenho
**Descrição**: O sistema deve ter tempos de resposta aceitáveis.

**Critérios**:
- Lista de chamados deve carregar em < 2 segundos
- Envio de mensagem deve ter latência < 1 segundo
- Auto-refresh do desktop deve ocorrer a cada 3 segundos
- Upload de arquivo deve mostrar progress bar

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RNF02 - Segurança
**Descrição**: O sistema deve proteger dados sensíveis dos usuários.

**Critérios**:
- Senhas devem ser armazenadas com hash bcrypt (cost factor 11)
- Comunicação deve usar HTTPS em produção
- Tokens JWT devem expirar após 24 horas
- Uploads devem validar tipo e tamanho de arquivo
- Proteção contra SQL Injection (uso de ORM)

**Prioridade**: Crítica  
**Status**: ✅ Implementado

---

### RNF03 - Usabilidade
**Descrição**: O sistema deve ser fácil de usar e intuitivo.

**Critérios**:
- Interface deve ser responsiva (mobile, tablet, desktop)
- Feedback visual para todas as ações (loading, success, error)
- Mensagens de erro claras em português
- Máximo 3 cliques para ações principais
- Design consistente entre Web e Desktop

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RNF04 - Confiabilidade
**Descrição**: O sistema deve funcionar de forma estável e confiável.

**Critérios**:
- Disponibilidade de 99% (aceitável para ambiente acadêmico)
- Tratamento de erros com mensagens apropriadas
- Logs de erros para debugging
- Transações de banco de dados devem ser atômicas

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RNF05 - Manutenibilidade
**Descrição**: O código deve ser fácil de manter e evoluir.

**Critérios**:
- Aplicação de princípios SOLID
- Código em camadas (Presentation, Business, Data)
- Comentários em métodos complexos
- Nomenclatura clara de classes e métodos
- Padrão Repository para acesso a dados

**Prioridade**: Média  
**Status**: ✅ Implementado

---

### RNF06 - Portabilidade
**Descrição**: O sistema deve funcionar em diferentes ambientes.

**Critérios**:
- Web deve funcionar em Chrome, Firefox, Edge, Safari
- Desktop deve funcionar em Windows 10/11
- Backend deve funcionar em Windows, Linux, macOS
- Banco de dados: SQL Server 2019+

**Prioridade**: Média  
**Status**: ✅ Implementado

---

### RNF07 - Escalabilidade
**Descrição**: O sistema deve suportar crescimento de usuários e dados.

**Critérios**:
- Suporte a pelo menos 100 usuários simultâneos
- Banco de dados deve usar índices em queries frequentes
- Paginação de resultados quando apropriado
- Otimização de queries com LINQ

**Prioridade**: Baixa  
**Status**: ✅ Implementado

---

### RNF08 - Tecnologias
**Descrição**: O sistema deve utilizar tecnologias modernas e robustas.

**Tecnologias Obrigatórias**:
- Backend: ASP.NET Core 8.0
- Frontend Web: React 18+ com TypeScript
- Frontend Desktop: .NET MAUI 8.0
- Banco de Dados: SQL Server
- ORM: Entity Framework Core 8.0
- Comunicação Real-Time: SignalR

**Prioridade**: Alta  
**Status**: ✅ Implementado

---

### RNF09 - Padrões de Projeto
**Descrição**: O sistema deve aplicar padrões de projeto reconhecidos.

**Padrões Aplicados**:
- Repository Pattern (acesso a dados)
- Service Layer Pattern (lógica de negócio)
- DTO Pattern (transferência de dados)
- Dependency Injection (injeção de dependência)
- MVC/MVVM (separação de responsabilidades)

**Prioridade**: Média  
**Status**: ✅ Implementado

---

### RNF10 - Documentação
**Descrição**: O sistema deve ter documentação adequada.

**Critérios**:
- API documentada com Swagger/OpenAPI
- Comentários XML em métodos públicos
- README com instruções de instalação
- Diagramas UML (Casos de Uso, Classes, MER)
- Relatório acadêmico completo

**Prioridade**: Alta  
**Status**: ✅ Implementado
