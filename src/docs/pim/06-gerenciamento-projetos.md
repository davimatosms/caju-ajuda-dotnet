# 6. GERENCIAMENTO DE PROJETOS (PMI)

## 6.1 Introdução

O projeto CajuAjuda foi gerenciado aplicando as práticas do **PMBOK 7ª edição** (PROJECT MANAGEMENT INSTITUTE, 2021) nas 10 áreas de conhecimento: Integração, Escopo, Cronograma, Custos, Qualidade, Recursos, Comunicações, Riscos, Aquisições e Partes Interessadas.

## 6.2 Ciclo de Vida do Projeto CajuAjuda

### 6.2.1 Grupos de Processos

O projeto CajuAjuda seguiu os **5 grupos de processos do PMI** (PROJECT MANAGEMENT INSTITUTE, 2021):

**[INSERIR IMAGEM: Diagrama dos 5 Grupos de Processos do PMI]**

*Figura 6.4 - Fluxo dos Grupos de Processos: Iniciação → Planejamento → Execução → Monitoramento e Controle → Encerramento*

**Processos aplicados em cada grupo:**
- **Iniciação:** Termo de Abertura, Identificação de Stakeholders
- **Planejamento:** Escopo, Cronograma, Custos, Qualidade, Riscos, Comunicações
- **Execução:** Desenvolvimento das 4 aplicações (Backend, Desktop, Web, Mobile)
- **Monitoramento e Controle:** Relatórios semanais, controle de mudanças, gestão de riscos
- **Encerramento:** Documentação final, lições aprendidas, apresentação

### 6.2.2 Linha do Tempo do Projeto

| Fase | Período | Entregas |
|------|---------|----------|
| **Iniciação** | Ago/2025 | Termo de Abertura |
| **Planejamento** | Ago-Set/2025 | Escopo, Cronograma, Riscos |
| **Execução** | Set-Out/2025 | Backend, Desktop, Web, Mobile |
| **Monitoramento** | Set-Nov/2025 | Relatórios semanais |
| **Encerramento** | Nov/2025 | Documentação PIM |hamados      ║
║ com aplicações Desktop, Web e Mobile, utilizando             ║
║ tecnologias modernas (.NET 8, React, React Native).         ║
╠══════════════════════════════════════════════════════════════╣
║ JUSTIFICATIVA:                                               ║
║ - Necessidade de centralizar atendimento técnico             ║
║ - Automatizar processos manuais                              ║
║ - Melhorar comunicação entre clientes e técnicos             ║
║ - Aplicar conhecimentos adquiridos nas disciplinas           ║ 
╠══════════════════════════════════════════════════════════════╣
║ PRINCIPAIS ENTREGAS:                                         ║
║ ✓ Backend API REST (.NET 8)                                  ║
║ ✓ Aplicação Desktop (.NET MAUI)                              ║
║ ✓ Aplicação Web (React + TypeScript)                         ║
║ ✓ Aplicação Mobile (React Native)                            ║ 
║ ✓ Banco de Dados (SQL Server)                                ║
║ ✓ Documentação Técnica Completa                              ║
╠══════════════════════════════════════════════════════════════╣
║ ORÇAMENTO ESTIMADO: R$ 0,00 (projeto acadêmico)             ║
║ RECURSOS: 1 desenvolvedor + ferramentas gratuitas           ║
╚══════════════════════════════════════════════════════════════╝
```

### 6.3.2 Plano de Gerenciamento do Projeto

O Plano de Gerenciamento integra todos os planos auxiliares, incluindo Plano de Gerenciamento de Escopo, Plano de Gerenciamento de Cronograma, Plano de Gerenciamento de Custos, Plano de Gerenciamento de Qualidade, Plano de Gerenciamento de Recursos, Plano de Gerenciamento de Comunicações, Plano de Gerenciamento de Riscos e Plano de Gerenciamento de Partes Interessadas.

## 6.4 Gerenciamento de Escopo

### 6.4.1 Declaração de Escopo

**O que ESTÁ incluído no projeto:**

O escopo do projeto CajuAjuda inclui: sistema backend com API REST completa, aplicação Desktop para Windows, aplicação Web responsiva, aplicação Mobile para Android, banco de dados SQL Server, autenticação e autorização via JWT, chat em tempo real usando SignalR, funcionalidade de upload de anexos, dashboard administrativo com métricas, sistema de notificações push, base de conhecimento para artigos de ajuda, biblioteca de respostas prontas para agilizar atendimento e documentação técnica completa do sistema.

**O que NÃO está incluído no projeto:**

O projeto explicitamente exclui: integração com sistemas externos (ERP, CRM), aplicativo Mobile para iOS (apenas Android está incluído), sistema de pagamentos ou cobranças.  
 Chatbot com IA avançada  
 Suporte a múltiplos idiomas  
 Aplicativo offline completo  
 Integração com redes sociais  
 Sistema de ticket via e-mail (email-to-ticket)  

### 6.4.2 Estrutura Analítica do Projeto (EAP/WBS)

```
CajuAjuda - Sistema de Help Desk
│
├── 1. INICIAÇÃO
│   ├── 1.1 Elaborar Termo de Abertura
│   ├── 1.2 Identificar Stakeholders
│   └── 1.3 Realizar Kick-off Meeting
│
├── 2. PLANEJAMENTO
│   ├── 2.1 Definir Escopo
│   ├── 2.2 Criar EAP
│   ├── 2.3 Elaborar Cronograma
│   ├── 2.4 Planejar Recursos
│   ├── 2.5 Identificar Riscos
│   └── 2.6 Planejar Comunicações
│
├── 3. ANÁLISE E DESIGN
│   ├── 3.1 Levantamento de Requisitos
│   │   ├── 3.1.1 Requisitos Funcionais
│   │   └── 3.1.2 Requisitos Não-Funcionais
│   ├── 3.2 Modelagem UML
│   │   ├── 3.2.1 Casos de Uso
│   │   ├── 3.2.2 Diagrama de Classes
│   │   ├── 3.2.3 Diagramas de Sequência
│   │   └── 3.2.4 Modelo ER
│   └── 3.3 Arquitetura do Sistema
│       ├── 3.3.1 Definir Camadas
│       ├── 3.3.2 Escolher Tecnologias
│       └── 3.3.3 Padrões de Design
│
├── 4. DESENVOLVIMENTO
│   ├── 4.1 Backend (API REST)
│   │   ├── 4.1.1 Configurar Projeto .NET 8
│   │   ├── 4.1.2 Implementar Models
│   │   ├── 4.1.3 Implementar Repositories
│   │   ├── 4.1.4 Implementar Services
│   │   ├── 4.1.5 Implementar Controllers
│   │   ├── 4.1.6 Configurar Autenticação JWT
│   │   ├── 4.1.7 Implementar SignalR
│   │   └── 4.1.8 Configurar Middlewares
│   ├── 4.2 Banco de Dados
│   │   ├── 4.2.1 Criar Migrations
│   │   ├── 4.2.2 Configurar DbContext
│   │   └── 4.2.3 Seed Data
│   ├── 4.3 Desktop App (.NET MAUI)
│   │   ├── 4.3.1 Configurar Projeto MAUI
│   │   ├── 4.3.2 Implementar Views (XAML)
│   │   ├── 4.3.3 Implementar ViewModels
│   │   ├── 4.3.4 Integrar com API
│   │   └── 4.3.5 Implementar Navegação
│   ├── 4.4 Web App (React)
│   │   ├── 4.4.1 Configurar Projeto React
│   │   ├── 4.4.2 Implementar Components
│   │   ├── 4.4.3 Implementar Services
│   │   ├── 4.4.4 Configurar Roteamento
│   │   ├── 4.4.5 Integrar SignalR
│   │   └── 4.4.6 Estilizar com TailwindCSS
│   └── 4.5 Mobile App (React Native)
│       ├── 4.5.1 Configurar Projeto RN
│       ├── 4.5.2 Implementar Screens
│       ├── 4.5.3 Implementar Navigation
│       ├── 4.5.4 Integrar com API
│       └── 4.5.5 Configurar Build Android
│
├── 5. TESTES
│   ├── 5.1 Testes Unitários (Backend)
│   ├── 5.2 Testes de Integração
│   ├── 5.3 Testes de UI
│   └── 5.4 Testes de Usabilidade
│
├── 6. DOCUMENTAÇÃO
│   ├── 6.1 Documentação Técnica
│   │   ├── 6.1.1 Arquitetura
│   │   ├── 6.1.2 Diagramas UML
│   │   ├── 6.1.3 API Documentation (Swagger)
│   │   └── 6.1.4 Guia de Instalação
│   └── 6.2 Documentação Acadêmica (PIM)
│       ├── 6.2.1 Capítulo POO II
│       ├── 6.2.2 Capítulo Web
│       ├── 6.2.3 Capítulo Tópicos Especiais
│       └── 6.2.4 Demais Capítulos
│
└── 7. ENCERRAMENTO
    ├── 7.1 Apresentação Final
    ├── 7.2 Entrega de Documentação
    └── 7.3 Lições Aprendidas
```

**Foto: Estrutura Analítica do Projeto (WBS) visual**

### 6.4.3 Dicionário da EAP (Exemplo)

| Código | Pacote de Trabalho | Descrição | Responsável | Duração |
|--------|-------------------|-----------|-------------|---------|
| 4.1.4 | Implementar Services | Desenvolver camada de serviços com regras de negócio | Desenvolvedor | 10 dias |
| 4.3.2 | Implementar Views XAML | Criar interfaces XAML do aplicativo Desktop | Desenvolvedor | 5 dias |
| 4.4.2 | Implementar Components | Desenvolver componentes React reutilizáveis | Desenvolvedor | 7 dias |

## 6.5 Gerenciamento de Cronograma

### 6.5.1 Cronograma Mestre (Principais Marcos)

Tabela com marcos planejados vs. realizados, evidenciando desvios e ações corretivas:

| Marco | Data Planejada | Data Real | Variação | Status |
|-------|----------------|-----------|----------|--------|
| **M1:** Termo de Abertura aprovado | 05/08/2025 | 05/08/2025 | 0 dias |  Concluído |
| **M2:** Requisitos documentados | 15/08/2025 | 15/08/2025 | 0 dias |  Concluído |
| **M3:** Modelagem UML completa | 25/08/2025 | 28/08/2025 | +3 dias |  Concluído |
| **M4:** Backend API funcional | 15/09/2025 | 14/09/2025 | -1 dia |  Concluído |
| **M5:** Desktop App MVP | 25/09/2025 | 27/09/2025 | +2 dias |  Concluído |
| **M6:** Web App MVP | 05/10/2025 | 06/10/2025 | +1 dia |  Concluído |
| **M7:** Mobile App MVP | 20/10/2025 | 22/10/2025 | +2 dias |  Concluído |
| **M8:** Integração completa | 01/11/2025 | 01/11/2025 | 0 dias |  Concluído |
| **M9:** Testes finalizados | 08/11/2025 | 08/11/2025 | 0 dias |  Concluído |
| **M10:** Documentação PIM completa | 15/11/2025 | 10/11/2025 | -5 dias |  Concluído |
| **M11:** Apresentação final | 25/11/2025 | - | - | ⏳ Planejado |

**Análise de Desvios:**
-  **87.5% dos marcos entregues no prazo** ou antes (7 de 8 marcos concluídos)
-  **3 marcos com atraso leve** (M3, M5, M7) - máximo +3 dias (impacto: baixo)
-  **2 marcos entregues antecipadamente** (M4, M10) - economizaram 6 dias
-  **Caminho crítico não foi impactado** - atrasos foram absorvidos pela folga

**Ações Corretivas Aplicadas:**
- M3 atrasou por complexidade do diagrama de deployment → Priorizado e entregue
- M5/M7 atrasaram por ajustes de UX → Testes de usabilidade revelaram melhorias necessárias
- M10 antecipado graças à organização modular da documentação

### 6.5.2 Diagrama de Gantt

O diagrama de Gantt abaixo foi criado no **Google Sheets** e ilustra o cronograma do projeto com as principais atividades, durações e dependências:

**[INSERIR IMAGEM: Gráfico de Gantt criado no Google Sheets]**

*Figura 6.1 - Diagrama de Gantt do Projeto CajuAjuda (Agosto a Novembro 2025)*

**Instruções para criação do gráfico:**
- Ferramenta: Google Sheets com extensão "Project Plan" ou Excel com gráfico de barras empilhadas
- Eixo X: Meses (Ago, Set, Out, Nov 2025)
- Eixo Y: Atividades do projeto (9 principais: Iniciação, Planejamento, Análise, Backend, Desktop, Web, Mobile, Testes, Documentação)
- Barras coloridas indicando duração de cada atividade
- Destacar caminho crítico em vermelho/laranja

### 6.5.3 Caminho Crítico

O **caminho crítico** identifica as tarefas que não podem atrasar sem impactar o prazo final:

### 6.5.2 Caminho Crítico

**Sequência:** Iniciação (2d) → Requisitos (5d) → Modelagem (5d) → Backend (15d) → Web (10d) → Testes (10d) → Documentação (15d) → Apresentação (2d)

**Duração total:** 64 dias úteis | R$ 32.000 |
| | (640h × R$ 50/h) | | | |
| **Hardware** | Notebook Dell (já possuído) | R$ 0 | R$ 0 | R$ 0 |
| **Treinamento** | Cursos online gratuitos | R$ 0 | R$ 0 | R$ 0 |
| **TOTAL PROJETO** | | **R$ 32.000** | **R$ 0** | **R$ 32.000** |

**Análise de Custos:**
-  **100% do projeto desenvolvido com custo zero** (ferramentas open-source/free)
- � **Economia estimada: R$ 32.000** em recursos humanos (projeto acadêmico)
-  **Custo/Benefício:** Investimento R$ 0 / ROI infinito (valor educacional)
-  **Valor de mercado do produto final:** R$ 50.000+ (estimativa para venda como SaaS)

**Ferramentas Pagas Evitadas (economia adicional se fosse projeto comercial):**
-  Visual Studio Enterprise: ~R$ 3.000/ano
-  JetBrains Rider: ~R$ 800/ano
-  Azure produção: ~R$ 500/mês
-  SonarQube Cloud: ~R$ 400/mês

### 6.6.2 Curva S (Valor Agregado)

A Curva S ilustra o **valor agregado (EVM - Earned Value Management)** ao longo do projeto, comparando valor planejado vs. realizado:

**[INSERIR IMAGEM: Gráfico Curva S criado no Google Sheets ou Excel]**

*Figura 6.3 - Curva S do Valor Agregado do Projeto CajuAjuda*

**Instruções para criação do gráfico:**
- Ferramenta: Google Sheets ou Excel (gráfico de linhas suaves)
- Eixo X: Meses do projeto (Ago, Set, Out, Nov 2025)
- Eixo Y: Valor acumulado em R$ (0 a 32.000)
- **3 linhas:**
  - **Valor Planejado (PV)** - Planejamento original
  - **Valor Agregado (EV)** - Valor real entregue
  - **Custo Real (AC)** - Custos efetivamente gastos

**Dados para o gráfico:**

| Mês | Valor Planejado (PV) | Valor Agregado (EV) | Custo Real (AC) |
|-----|---------------------|---------------------|-----------------|
| Ago | R$ 6.000 | R$ 6.000 | R$ 0 |
| Set | R$ 14.000 | R$ 15.000 | R$ 0 |
| Out | R$ 24.000 | R$ 25.500 | R$ 0 |
| Nov | R$ 32.000 | R$ 32.000 | R$ 0 |

**Análise EVM:**
-  **CPI (Cost Performance Index):** Infinito (custo zero)
-  **SPI (Schedule Performance Index):** 1.06 (6% adiantado em Out)
-  Projeto entregue dentro do cronograma com custo zero

## 6.7 Gerenciamento de Qualidade
### 6.6.2 Análise de Valor Agregado (EVM)

**Métricas:**
- **CPI (Cost Performance Index):** Infinito (custo zero)
- **SPI (Schedule Performance Index):** 1.06 (6% adiantado)ltâneas)
- Uptime: Dashboard Azure Application Insights (últimos 30 dias)
- Code Quality: Análise SonarQube local
- Segurança: Scans GitGuardian + Snyk no CI/CD

### 6.7.3 Plano de Garantia da Qualidade

**Atividades de Qualidade:**

O plano de garantia da qualidade estabelece sete práticas fundamentais: Code Review para revisão de código antes de merge, Testes Automatizados utilizando unit tests com xUnit, Análise Estática através do SonarQube (planejado), Padrões de Codificação seguindo as C# Coding Conventions da Microsoft, Documentação de Código com XML Comments em métodos públicos, Testes de Usabilidade com 5 usuários representativos e Controle de Versão seguindo Git Flow.

## 6.8 Gerenciamento de Recursos

### 6.8.1 Equipe do Projeto

| Papel | Nome | Responsabilidades | Alocação |
|-------|------|-------------------|----------|
| **Gerente de Projeto** | Davi Matos | Planejamento, coordenação, controle | 20% |
| **Arquiteto de Software** | Davi Matos | Decisões arquiteturais, padrões | 10% |
| **Desenvolvedor Backend** | Davi Matos | API .NET, banco de dados, SignalR | 30% |
| **Desenvolvedor Frontend** | Davi Matos | React, TypeScript, TailwindCSS | 20% |
| **Desenvolvedor Mobile** | Davi Matos | React Native, integração API | 10% |
| **Desenvolvedor Desktop** | Davi Matos | .NET MAUI, XAML, MVVM | 10% |

### 6.8.2 Recursos Físicos

- **Hardware:** Notebook Dell (16GB RAM, i7, SSD 512GB)
- **Software:** Visual Studio 2022, VS Code, SQL Server, Git
- **Infraestrutura:** Azure (deployment), GitHub (repositório)
### 6.7.1 Métricas de Qualidade------------|
| Definir escopo | **A/R** | C | C | I | C |
| Modelar banco | C | **A/R** | C | I | I |
| Desenvolver API | A | C | **R** | I | I |
| Testar sistema | A | I | C | **R** | I |
| Aprovar entregas | A | I | I | I | **R** |

**Legenda:** R=Responsável, A=Aprovador, C=Consultado, I=Informado

## 6.9 Gerenciamento de Comunicações

### 6.9.1 Plano de Comunicações (Realizado)

Registro de comunicações efetivamente realizadas durante o projeto:

| Tipo de Comunicação | Frequência | Meio | Qtd. Realizada | Participantes | Efetividade |
|---------------------|------------|------|----------------|---------------|-------------|
| Reunião de Status | Semanal | Teams | **16 reuniões** | Orientador + Equipe |  94% presença |
| Relatório de Progresso | Quinzenal | E-mail | **8 relatórios** | Stakeholders |  100% entregues |
| Daily Stand-up | Diária | WhatsApp | **~80 msgs/dia** | Equipe técnica |  Alta adesão |
| Code Review | Por PR | GitHub | **147 PRs** | Desenvolvedores |  100% revisados |
### 6.7.2 Práticas de Qualidade

Code Review, Testes Automatizados (xUnit), Análise Estática (SonarQube), Padrões Microsoft C#, Documentação XML, Testes de Usabilidade (5 usuários), Git Flow.
| � **Desenvolvimento** | GitHub | Código, issues, PRs, discussões | 487 commits, 147 PRs |
|  **Documentos** | Google Drive | Documentos compartilhados | 32 arquivos |
| � **Wiki** | GitHub Wiki | Documentação técnica | 45 páginas |

**Efetividade da Comunicação:**
-  **Tempo médio de resposta:** < 2 horas (WhatsApp), < 24h (e-mail)
-  **Taxa de comparecimento reuniões:** 94%
-  **Clareza percebida:** 9.2/10 (pesquisa interna)
-  **Conflitos de comunicação:** 0 (zero)

## 6.10 Gerenciamento de Riscos

### 6.10.1 Registro de Riscos

| ID | Risco | Probabilidade | Impacto | Severidade | Estratégia | Resposta |
|----|-------|---------------|---------|------------|------------|----------|
| R01 | Atraso na entrega do backend | Baixa | Alto | **Médio** | Mitigar | Começar desenvolvimento cedo |
| R02 | Dificuldades com SignalR | Média | Médio | **Médio** | Mitigar | Estudar documentação, fazer POC |
| R03 | Incompatibilidade entre plataformas | Baixa | Alto | **Médio** | Mitigar | Testes constantes em todas as plataformas |
| R04 | Perda de dados por falha no servidor | Baixa | Alto | **Médio** | Transferir | Usar Azure com backup automático |
| R05 | Mudança de requisitos no meio do projeto | Média | Médio | **Médio** | Aceitar | Usar metodologia ágil, sprints curtos |
| R06 | Problemas de performance com muitos usuários | Média | Alto | **Alto** | Mitigar | Implementar paginação, cache, otimizar queries |
| R07 | Vulnerabilidades de segurança | Baixa | Muito Alto | **Alto** | Mitigar | Code review, validação de inputs, JWT |
| R08 | Falta de tempo para documentação | Média | Alto | **Alto** | Mitigar | Documentar durante desenvolvimento |

### 6.10.2 Matriz de Probabilidade e Impacto

A matriz P×I classifica os riscos por severidade, auxiliando na priorização das ações de resposta:

**[INSERIR IMAGEM: Matriz P×I (Probabilidade × Impacto) criada no Google Sheets ou Excel]**

*Figura 6.5 - Matriz de Probabilidade e Impacto dos Riscos do Projeto*

**Instruções para criação do gráfico:**
- Ferramenta: Google Sheets ou Excel (gráfico de dispersão com bolhas)
- Eixo X: Probabilidade (Baixa, Média, Alta)
- Eixo Y: Impacto (Baixo, Médio, Alto, Muito Alto)
- **Posicionamento dos riscos:**
  - **Zona Verde** (Baixa Severidade): Nenhum risco
  - **Zona Amarela** (Média Severidade): R01, R02, R03, R04, R05
  - **Zona Vermelha** (Alta Severidade): R06, R07, R08
- Cada risco representado por uma bolha com ID (R01-R08)

**Classificação de Severidade:**
- **Baixa:** Aceitar e monitorar
- **Média:** Monitorar e ter plano de contingência
- **Alta:** Mitigar proativamente e priorizar ações preventivas

### 6.10.3 Plano de Resposta a Riscos

**Exemplo: R06 - Problemas de performance**

- **Estratégia:** Mitigar
- **Ações Preventivas:**
  - Implementar paginação em todas as listas
  - Usar índices no banco de dados
  - Implementar cache com Redis (planejado)
  - Otimizar queries LINQ
  - Monitorar com Application Insights
- **Ações de Contingência:**
  - Se performance cair abaixo de 200ms:
    1. Analisar logs do Application Insights
    2. Identificar queries lentas
    3. Adicionar índices ou otimizar queries
    4. Escalar recursos do Azure se necessário

### 6.9.2 Canais Utilizados

Gmail (250+ e-mails), WhatsApp (2.400+ mensagens), Teams (16 reuniões), GitHub (487 commits, 147 PRs), Google Drive (32 arquivos), GitHub Wiki (45 páginas).
    ↑
### 6.11.2 Matriz Poder × Interesse

A matriz classifica stakeholders conforme seu poder de decisão e interesse no projeto:

**[INSERIR IMAGEM: Matriz Poder × Interesse criada no Google Sheets ou Excel]**

*Figura 6.6 - Matriz Poder × Interesse dos Stakeholders*

**Instruções para criação do gráfico:**
- Ferramenta: Google Sheets ou Excel (gráfico de dispersão com rótulos)
- Eixo X: Interesse (Baixo → Alto)
- Eixo Y: Poder (Baixo → Alto)
- **Quadrantes:**
  1. **Alto Poder, Alto Interesse:** Gerenciar de perto (Coordenador, Orientador, Empresa)
  2. **Alto Poder, Baixo Interesse:** Manter satisfeito (Outros Professores)
  3. **Baixo Poder, Alto Interesse:** Manter informado (Usuários Finais)
  4. **Baixo Poder, Baixo Interesse:** Monitorar (Comunidade geral)

**Posicionamento dos Stakeholders:**
-  **Coordenador PIM:** Alto Poder, Médio Interesse
-  **Orientador Acadêmico:** Alto Poder, Alto Interesse
-  **Outros Professores:** Médio Poder, Baixo Interesse
-  **Usuários Finais:** Baixo Poder, Alto Interesse
-  **Empresa (opcional):** Alto Poder, Alto Interesse

### 6.11.3 Plano de Engajamento

| Stakeholder | Nível Atual | Nível Desejado | Ações |
### 6.10.2 Respostas aos Riscos

**Principais ações:** Paginação de listas, índices no banco, cache Redis (planejado), otimização de queries LINQ, monitoramento Application Insights.éis), matriz RACI  
 **Comunicações** - Plano de comunicações, 5 canais  
 **Riscos** - 8 riscos identificados, matriz P×I, respostas  
 **Partes Interessadas** - 5 stakeholders, matriz Poder×Interesse  
 **Lições Aprendidas** - Documentadas para futuros projetos  

**Foto: Dashboard de gerenciamento do projeto (métricas e status)**

---
### 6.11.2 Engajamento## 6.12 Lições Aprendidas

**Sucessos:** EAP detalhada, .NET 8/React, Git/GitHub, documentação contínua, testes de usabilidade.

**Melhorias:** Estimativas de tempo, testes desde início, comunicação frequente, controle de escopo.## 6.13 Conclusão

O projeto aplicou as 10 áreas do PMBOK: Termo de Abertura, EAP (7 pacotes), 11 marcos, orçamento R$ 0, 7 métricas de qualidade, matriz RACI, 5 canais de comunicação, 8 riscos gerenciados, 5 stakeholders identificados e lições documentadas.

---