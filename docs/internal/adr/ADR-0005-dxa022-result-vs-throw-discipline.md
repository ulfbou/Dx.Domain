# ADR-0005: DXA022 — Result vs Throw Discipline

**Status**: Accepted  
**Date**: 2026-01-29  
**Spec Reference**: Rule §8.2 DXA022. 

## Context
Public or “public enough” methods returning `Result/Result<T>` must not throw **domain outcomes**. Invariants/guards/programming errors are allowed. Kernel defines semantics; consumers are constrained. 

## Decision
- **Consumer-only** (Constraining).  
- Applies to `public`, `protected`, `protected internal` only.  
- Outcome classification precedence: markers/base types → known domain-error namespace → else **not** a domain outcome (allowed).  
- Policy: prefer **under-reporting** to avoid false positives. 

## Consequences
- Contract-facing APIs are disciplined; kernel self-hosting remains unimpeded.
