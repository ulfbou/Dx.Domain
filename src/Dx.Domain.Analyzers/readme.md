# Dx.Domain.Analyzers

Compile-time enforcement of Dx.Domain invariants.

## Purpose

Ensure that domain code adheres to:

- construction authority rules
- Result handling discipline
- exception usage constraints

## Key Diagnostics

- DXA010 — Construction Authority Violation
- DXA011 — Public Factory Exposure
- DXA020 — Result Ignored
- DXA022 — Domain Control Exception
- DXA030 — Unapproved Handler Usage

## Enforcement Model

- violations produce diagnostics
- no runtime fallback exists
- correctness is enforced at compile time

## Scope Awareness

Rules are applied based on scope:

- S0 (kernel) — trusted
- S1–S3 — enforced

See:

- `docs/public/specification/core-platform.md`

