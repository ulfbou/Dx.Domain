# Dx.Domain

Dx.Domain is a deterministic, invariant-driven domain modeling platform for .NET.

It enforces correctness through:

- compile-time analyzers
- controlled construction
- Result-based error semantics
- a strictly bounded core architecture

## Core Packages

- Dx.Domain.Annotations
- Dx.Domain.Primitives
- Dx.Domain.Kernel
- Dx.Domain.Facts

See:

- `docs/public/specification/core-platform.md`
- `docs/public/packages/*`

## Design Principles

- Invalid states are unrepresentable
- Domain failures are values, not exceptions
- Construction is centralized and auditable
- All behavior is deterministic

## Status

Pre-release (v0.x)

- APIs may change
- Core invariants are stable

## Non-Goals

The platform does not provide:

- persistence
- workflows
- application services
- policy engines

## Tooling

Enforcement is performed via:

- Dx.Domain.Analyzers

Violations are compile-time diagnostics.

