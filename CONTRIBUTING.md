# Contributing

Welcome! This repository follows a **governance-first** approach. Please read the short rationale below before opening a PR.

## Read these first

- **Manifesto** — why the project exists and what it refuses to become  
  → `docs/internal/MANIFESTO.md`

- **Non‑Goals** — scope guardrails; proposals conflicting with these are out of bounds  
  → `docs/internal/NON_GOALS.md`

- **DPI (Design Pressure Index)** — mechanical test for changes; use this to justify placement  
  → `docs/internal/DPI.md`

## PR expectations (foundation)

- Keep PRs **small and single-purpose**.
- Use **conventional commits** (`feat:`, `fix:`, `chore:`, `docs:`, etc.).
- If your change touches Kernel/Analyzers semantics, include a **DPI paragraph** explaining:
  1) which invariant is enforced or misuse removed,
  2) why Kernel vs edge (analyzers/generators/adapters), and
  3) how the compiler/proofs improve.

## Analyzer severities

- Maintainers use `.editorconfig.maintainers` (Dx* = **errors**).
- Consumers use `.editorconfig.consumers` (Dx* = **warnings**).
- Tests may relax severities to avoid build noise from intentional patterns in test code.

Thanks for keeping the Kernel small, explicit, and strictly governed.
