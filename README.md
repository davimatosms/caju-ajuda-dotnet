# <img width="35" height="35" alt="logo caju" src="https://github.com/user-attachments/assets/0ded85a6-c88c-416c-80d1-aaf5b5effb11" /> Caju Ajuda - Sistema de Helpdesk (.NET)

[![Status](https://img.shields.io/badge/status-em_desenvolvimento-yellowgreen.svg)](https://shields.io/)
[![Backend](https://img.shields.io/badge/Backend-ASP.NET%20Core%208-blueviolet.svg)](https://shields.io/)
[![Frontend](https://img.shields.io/badge/Frontend-Desktop%20%7C%20Web%20%7C%20Mobile-orange.svg)](https://shields.io/)

O **Caju Ajuda** é uma solução de suporte e gerenciamento de tickets (helpdesk) completa, projetada com uma arquitetura moderna API-First para servir múltiplos clientes. O backend, construído em C# com ASP.NET Core, serve como o cérebro da operação para clientes Web, Mobile (React Native) e Desktop (.NET MAUI).

---

### **Documentação e Planejamento**

* **[📈 Product Backlog](./documentacao/Backlog.md)**: A lista completa de funcionalidades, épicos e user stories do projeto.
* **[📅 Roadmap de Sprints](./documentacao/sprint.md)**: O cronograma detalhado de cada Sprint, com objetivos e escopo de entrega.
* **[📚 Pasta de Documentação](./docs/)**: Acesso a todos os artefatos do projeto, incluindo diagramas, manuais e definições.

---

### **Cronograma Visual de Evolução**
O roadmap abaixo apresenta uma visão geral do planejamento de Sprints para a entrega da Versão 1.0.

```mermaid
gantt
    title Roadmap de Sprints - Caju Ajuda V1.0
    dateFormat  YYYY-MM-DD
    axisFormat %d/%m

    section Sprint 1
    Gestão de Admin e Cliente :done,    s1, 2025-09-02, 14d

    section Sprint 2
    Produtividade do Técnico  :active,  s2, 2025-09-17, 14d

    section Sprint 3
    Métricas e Autonomia      :         s3, 2025-10-02, 14d
    
    section Sprint 4
    Funcionalidades Avançadas :         s4, 2025-10-17, 14d

    section Sprint 5
    Testes e Preparação       :         s5, 2025-10-31, 14d

    section Sprint 6
    Lançamento V1.0           :         s6, 2025-11-14, 14d

```

---

### **Tecnologias Utilizadas**
* **Backend:** C# 12, ASP.NET Core 8, Entity Framework Core 8, JWT Bearer Authentication
* **Frontend:** .NET MAUI, React com TypeScript, React Native
* **Banco de Dados:** MS SQL Server
* **Inteligência Artificial:** Google Gemini API
* **Ferramentas:** Git, Visual Studio Code, SQL Server Management Studio, Bruno

---

### **Como Executar o Projeto**

#### Pré-requisitos
* .NET 8 SDK
* Node.js e npm
* SQL Server (Developer ou Express Edition)

#### Execução
1.  **Backend:** Navegue até `src/backend` e execute `dotnet run`. A API iniciará em `https://localhost:7113`.
2.  **Web:** Navegue até `src/web` e execute `npm start`. A aplicação web iniciará em `http://localhost:3000`.

*(Para instruções detalhadas sobre os clientes Desktop e Mobile, consulte os READMEs específicos em suas respectivas pastas).*

---

### **Equipe**

| Foto | Nome Completo | Papel | Contato |
| :--- | :--- | :--- | :--- |
| <img src="https://avatars.githubusercontent.com/u/101799753?v=4" width=115> | Davi Matos Marques Silva | Desenvolvedor Full-Stack & Arquiteto do Projeto | [LinkedIn](URL_DO_SEU_LINKEDIN_AQUI) |
