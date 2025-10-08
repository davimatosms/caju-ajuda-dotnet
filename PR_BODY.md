
PR: feat/ui-design-tokens-buttons — Design tokens, reusable Button, and logo integration

What this PR does
- Adds CSS design tokens and global styles for the web app, plus a reusable gradient `Button` component used across the site.
- Harmonizes many web pages to use the new Button component for consistent visuals.
- Hardens the .NET MAUI desktop client to accept several JSON shapes returned by the API (arrays, `{ $values }`, and paged wrappers), fixes a page lifecycle bug by using `OnAppearing`, and removes blocking `.Result` calls at startup.
- Adds the official Caju logo asset to both web and desktop and wires it into the navigation headers so the logo appears on every page.

Why the SVG change matters (short)
- The provided SVG is complex and contains an embedded `<style>` block with many CSS class rules. When imported as a module, the CRA bundler attempted to parse/inline it and failed. To avoid bundler parse errors and unblock the build, the SVG was copied to `src/web/public/assets/caju-logo.svg` and the React components now reference it as a static URL (`/assets/caju-logo.svg`).

Files touched (high level)
- Web: `src/web/src/styles/tokens.css`, `src/web/src/styles/global.css`, `src/web/src/components/UI/*`, various pages updated to use the new `Button`.
- Web (logo): `src/web/public/assets/caju-logo.svg` (static public copy). Components updated: `src/web/src/components/PublicNavbar/PublicNavbar.tsx`, `src/web/src/components/Navbar/Navbar.tsx` (now reference `/assets/caju-logo.svg`).
- Desktop: `src/desktop/Services/ChamadoService.cs`, `src/desktop/Views/DetalheChamadoPage.xaml.cs`, `src/desktop/App.xaml.cs`, plus other small fixes.

How to verify (QA / reviewer guide)
1) Web (quick):
   - From repo root:
     ```powershell
     cd src\web
     npm install   # if needed
     npm run build
     ```
   - Expectation: build completes and `build/` is produced. The logo file should be referenced by `/assets/caju-logo.svg` in the built app. Open `build/index.html` served via a static server (e.g., `serve -s build`) to visually verify the logo is present in the navbar and on auth pages.

2) Web (dev):
   - Run `npm start` and open the app at http://localhost:3000. Navigate to public pages (Landing, Login, Register) and authenticated pages (Dashboard) to confirm the logo appears and Button styles look correct.

3) Desktop:
   - Build and run the MAUI app locally with the backend running or mocked. Verify:
     - Opening a chamado detail page with no messages does not crash.
     - Sending a message appends it to the UI list.

Notes for reviewers
- The SVG was intentionally moved to the `public/` folder to prevent bundler parse errors. If you prefer the SVG inlined as a React component, I can optimize it (remove the <style> block and inline fills or run SVGO) and convert it to an inline component in a follow-up.
- There are a few ESLint/TS warnings in the web project (unused variables). They do not block the build — consider cleaning them before merging if you want a warning-free CI.

Checklist before merge
- [ ] Reviewer inspects visual changes in web (run build/start and look at navbars & buttons)
- [ ] Desktop flows verified manually (login, open chamado, send mensagem)
- [ ] Optional: Address ESLint warnings shown during `npm run build`

If you want I can update this PR body further, optimize the SVG for inline usage, or open the PR on GitHub for you. (The changes are already pushed to branch `feat/ui-design-tokens-buttons`.)

---
Generated: automated PR body update to explain the SVG handling and verification steps.
