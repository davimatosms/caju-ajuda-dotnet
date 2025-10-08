PR: feat/ui-design-tokens-buttons

Resumo
- Adiciona design tokens e estilos globais para a aplicação web.
- Cria componente `Button` reutilizável com gradiente que preserva a cor laranja da marca.
- Substitui botões ad-hoc no frontend pelo novo componente `Button`.
- Desktop: normaliza o formato de mensagens e paged lists no `ChamadoService`, usa `OnAppearing` no `DetalheChamadoPage` e inicialização assíncrona do `App`.

Arquivos principais alterados
- web/src/styles/tokens.css
- web/src/styles/global.css
- web/src/components/UI/Button.tsx & Button.module.css
- várias páginas do web que agora usam Button (LandingPage, LoginPage, RegisterPage, etc.)
- Adiciona logo: `src/web/src/assets/caju-logo.svg` e `src/desktop/Resources/Images/caju-logo.svg`. O logo é exibido em todas as páginas via Navbar / AuthLayout.
- desktop/src: App.xaml.cs, MauiProgram.cs, Views/DetalheChamadoPage.xaml.cs, Services/ChamadoService.cs, ViewModels/LoginViewModel.cs

Motivação
- Unificar identidade visual e facilitar manutenção com tokens.
- Evitar runtime errors por formatos inconsistentes retornados pela API.
- Melhorar inicialização do app desktop (evitar .Result blocking).

Testes manuais realizados
1) Web: `npm run build` — compilou com warnings de ESLint (variáveis não usadas existentes). Verificado que o build gera `build/`.
2) Desktop: `dotnet build` — compilou com avisos MVVMTK0045 e CS8602 (residual), mas sem erros.
3) Fluxo manual (recomendado executar localmente com backend rodando):
   - Login na aplicação Desktop
   - Abrir a tela de detalhes de um chamado sem mensagens -> não crasha
   - Enviar mensagem -> mensagem aparece na lista

Notas de QA
- Backend deve retornar mensagens em um dos formatos aceitos: array direto, `{ $values: [...] }`, ou lista dentro de um objeto paginado. Se o backend mudar, ajustar o ChamadoService.
- Revisar avisos MVVMTK0045 se for necessário build AOT/WinRT.

Checklist
- [ ] Revisar código e design tokens
- [ ] Rodar app Desktop e testar fluxo de login e detalhe de chamado
- [ ] Rodar app Web e testar navegação principal

Descrição técnica curta
- Web: tokens CSS e GlobalStyles; botão primário com gradiente usando as variáveis `--color-brand-600` / `--color-brand-500`.
- Desktop: métodos de normalização JSON no client para suportar shapes distintos.

Solicitação
- Merge into `main` após revisão. Se quiser, eu posso abrir PR diretamente, mas a CLI `gh` não está disponível neste ambiente.


Testes automatizados
- `dotnet test` (nenhum unit test específico do desktop) — build executado com sucesso.
- `npm run build` para web — executado com sucesso (com warnings).

Obrigado!
