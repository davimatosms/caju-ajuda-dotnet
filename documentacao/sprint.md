## 2. Roadmap de Sprints (Estimativa)

* **Estimativa Total:** 6 Sprints (12 semanas)
* **Data de Início:** 02 de Setembro de 2025
* **Estimativa de Lançamento (V1.0):** Final de Novembro de 2025

---

### **Sprint 1: Fundações da Gestão e Experiência do Cliente**
* **Período:** 02 de Setembro – 16 de Setembro de 2025
* **Sprint Goal:** Concluir o ciclo de vida da gestão de técnicos no painel de administração e alcançar a paridade de funcionalidades críticas para o cliente nas plataformas web e mobile, garantindo a qualidade com testes e documentação inicial.
* **Itens do Backlog:**
    * **Épico 1 (Completo):** Gestão de Contas de Administrador (User Stories A-1, A-2, A-3).
    * **Épico 2 (Completo):** Paridade de Funcionalidades do Cliente (User Stories C-1, C-2, C-3).
    * **Débito Técnico:** Iniciar implementação de testes unitários (D-1) e configuração de logging (D-4).

---

### **Sprint 2: Produtividade da Equipe Técnica**
* **Período:** 17 de Setembro – 30 de Setembro de 2025
* **Sprint Goal:** Otimizar o fluxo de trabalho da equipe de suporte, entregando funcionalidades essenciais no cliente desktop para aumentar a eficiência na resolução de chamados.
* **Itens do Backlog:**
    * **Épico 3 (Completo):** Melhorias de Produtividade do Técnico (User Stories T-1, T-2, T-3, T-4, T-5).
    * **Épico 4 (Parcial):** Dashboards e Métricas (User Story A-4 - Dashboard do Admin).
    * **Débito Técnico:** Implementar Paginação nas APIs (D-3).

---

### **Sprint 3: Métricas e Autonomia do Cliente**
* **Período:** 01 de Outubro – 14 de Outubro de 2025
* **Sprint Goal:** Aumentar a satisfação e autonomia do cliente com a implementação de avaliação de atendimento e uma base de conhecimento, enquanto melhoramos a saúde técnica do backend.
* **Itens do Backlog:**
    * **Épico 4 (Finalização):** Dashboards e Métricas (User Story C-4 - Avaliação de Atendimento).
    * **Icebox (Priorizado):** Lançar a primeira versão da Knowledge Base (FAQ) (F-1).
    * **Débito Técnico:** Configurar um Error Handler Global (D-5).

---

### **Sprint 4: Funcionalidades Avançadas e Qualidade de Código**
* **Período:** 15 de Outubro – 28 de Outubro de 2025
* **Sprint Goal:** Finalizar as funcionalidades de gestão planejadas e realizar refatorações críticas no backend para garantir a manutenibilidade e escalabilidade do sistema.
* **Itens do Backlog:**
    * **Icebox (Priorizado):** Mesclar Chamados Duplicados (T-6).
    * **Icebox (Priorizado):** Gestão de Contas de Clientes pelo Admin (A-5).
    * **Débito Técnico:** Refatorar DTOs duplicados (D-2) e criar interface `FileStorageService` (D-6).

---

### **Sprint 5: Testes Integrados e Preparação para Produção**
* **Período:** 29 de Outubro – 11 de Novembro de 2025
* **Sprint Goal:** Garantir a estabilidade da V1.0 através de testes de ponta-a-ponta (E2E) em todas as plataformas e preparar a infraestrutura para o lançamento.
* **Itens do Backlog:**
    * **Testes:** Executar testes manuais completos nos fluxos do cliente e do técnico.
    * **Documentação:** Escrever a primeira versão do Manual do Usuário.
    * **Infra/DevOps:** Configurar o ambiente de produção na Azure (App Service, SQL Database).
    * **Bugs:** Corrigir todos os bugs críticos e de alta prioridade encontrados.

---

### **Sprint 6: Lançamento e Estabilização (Go-Live)**
* **Período:** 12 de Novembro – 25 de Novembro de 2025
* **Sprint Goal:** Lançar a versão 1.0 do Caju Ajuda, monitorar a saúde da aplicação em produção e realizar os ajustes finais.
* **Itens do Backlog:**
    * **Infra/DevOps:** Realizar o deploy da aplicação no ambiente da Azure.
    * **Monitoramento:** Acompanhar os logs e a performance da aplicação.
    * **Hotfixes:** Corrigir eventuais bugs críticos que surjam após o lançamento.
    * **Planejamento:** Realizar a retrospectiva do projeto e planejar o backlog para a V1.1.