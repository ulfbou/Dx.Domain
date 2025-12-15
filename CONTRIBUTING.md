# Contributing to Dx.Domain

If you have not read the Manifesto, Non‑Goals, and the Design Pressure Index, your contribution will be rejected.

## Process (Non‑Negotiable)

1. Read the Manifesto and Non‑Goals.
2. Place your change in the correct package (`Core` / `Values` / `Analyzers` / `Generators` / `Persistence`).
3. Open a pull request using the repository PR template. Complete the DPI checklist in the template.
4. Reviewers will enforce placement and Non‑Goals. If a PR fails the DPI, it does not belong in `Core`.

## Placement Guidance

  - **Core**: only executable invariants, error primitives, and minimal kernel primitives.
  - **Values**: strongly typed value objects and identity types.
  - **Analyzers**: static enforcement that can be applied without runtime impact.
  - **Generators**: code generation to remove repetition without changing semantics.
  - **Persistence**: adapters and integrations only.

## Enforcement

  - Pull requests must include the DPI checklist and a one-line justification for placement.
  - Maintainers will move misplaced changes to the correct package and may close pull requests that violate Non‑Goals.
