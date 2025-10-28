# Planejamento do Projeto: Caju Ajuda

* **Última Atualização:** 01 de Setembro de 2025
* **Visão Geral:** Este documento contém o Product Backlog completo e o Roadmap de Sprints estimado para o desenvolvimento da versão 1.0 do sistema Caju Ajuda.

---

## 1. Product Backlog

Este backlog documenta as funcionalidades, melhorias e débitos técnicos para o ecossistema Caju Ajuda.

### **Prioridade Alta**

#### 🚀 **Épico 1: Gestão de Contas de Administrador**
* **Descrição:** Expandir as funcionalidades do painel de administração para fornecer um ciclo de vida completo para a gestão de contas de técnicos.

| ID | User Story |
| :--- | :--- |
| **A-1** | **Como Admin**, quero poder **editar os dados** de um técnico. |
| **A-2** | **Como Admin**, quero poder **desativar/reativar** uma conta de técnico. |
| **A-3** | **Como Admin**, quero poder **redefinir a senha** de um técnico. |

#### 👤 **Épico 2: Paridade de Funcionalidades do Cliente (Mobile & Web)**
* **Descrição:** Garantir que a experiência do cliente seja consistente e completa tanto na plataforma web quanto na aplicação mobile.

| ID | User Story |
| :--- | :--- |
| **C-1** | **Como Cliente (Mobile)**, quero poder **enviar e visualizar anexos** no chat. |
| **C-2** | **Como Cliente (Web e Mobile)**, quero ver uma **notificação visual (badge)** nos meus chamados. |
| **C-3** | **Como Cliente (Web e Mobile)**, quero poder **fechar o meu próprio chamado**. |

---

### **Prioridade Média**

#### ⚙️ **Épico 3: Melhorias de Produtividade do Técnico (Desktop)**
* **Descrição:** Otimizar a interface desktop para melhorar a eficiência do fluxo de trabalho da equipa de suporte.

| ID | User Story |
| :--- | :--- |
| **T-1** | **Como Técnico**, quero **receber uma notificação em tempo real** quando um novo chamado for criado. |
| **T-2** | **Como Técnico**, quero poder **filtrar e ordenar a lista de chamados**. |
| **T-3** | **Como Técnico**, quero poder **atribuir um chamado a mim mesmo**. |
| **T-4** | **Como Técnico**, quero poder adicionar **notas internas** a um chamado que não são visíveis para o cliente. |
| **T-5** | **Como Técnico**, quero usar **respostas prontas (templates)** para resolver problemas comuns rapidamente. |

#### 📊 **Épico 4: Dashboards e Métricas**
* **Descrição:** Fornecer dados visuais para administradores e recolher feedback dos clientes.

| ID | User Story |
| :--- | :--- |
| **A-4** | **Como Admin**, quero ver um **dashboard com métricas chave** (total de chamados, tempo médio de resposta, etc.). |
| **C-4** | **Como Cliente**, quero poder **avaliar o atendimento** após a resolução de um chamado. |

---

### **Prioridade Baixa (Icebox)**
* **Descrição:** Ideias e melhorias que agregam valor, mas não são essenciais para o lançamento da V1.0.

| ID | User Story / Feature |
| :--- | :--- |
| **F-1** | **Knowledge Base (FAQ):** Permitir que clientes pesquisem artigos de ajuda antes de abrir um chamado. |
| **T-6** | **Mesclar Chamados:** Permitir que um técnico mescle dois chamados duplicados abertos pelo mesmo cliente. |
| **A-5** | **Gestão de Clientes:** Permitir que um Admin possa visualizar e desativar contas de clientes. |

---

### **Débito Técnico & Otimizações (Contínuo)**
* **Descrição:** Tarefas internas focadas em melhorar a qualidade, performance e manutenibilidade do sistema.

| ID | Tarefa Técnica |
| :--- | :--- |
| **D-1** | Implementar Testes Unitários e de Integração. |
| **D-2** | Refatorar DTOs Duplicados. |
| **D-3** | Implementar Paginação (Pagination) nas APIs. |
| **D-4** | Centralizar e Padronizar o Logging (SLF4J/Logback). |
| **D-5** | Configurar um Error Handler Global (`@ControllerAdvice`). |
| **D-6** | Criar uma interface `FileStorageService` para abstrair o armazenamento de ficheiros. |

---