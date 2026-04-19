# Dx.Domain.Facts

Monotonic domain knowledge representation.

## Purpose

Represent immutable, append-only facts derived from domain state.

## Guarantees

- facts are immutable
- facts are non-contradictory
- facts are deterministic

## Constraints

- no mutation
- no workflow logic
- no runtime decision making

## Role in System

- produced by Kernel
- consumed by analyzers and generators

See:

- `docs/public/packages/facts.md`

