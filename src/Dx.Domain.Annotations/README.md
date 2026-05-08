# Dx.Domain.Annotations

Semantic vocabulary for Dx.Domain.

## Purpose

Provide pure metadata markers that classify domain concepts for compile-time analysis. Attributes express intent without introducing runtime behavior.

## Guarantees

- attributes impose no runtime semantics
- identifiers are part of the public governance contract
- classification is stable across minor versions
- analyzers treat attributes as authoritative source of truth

## Constraints

- no constructors with logic
- no dependencies on Dx.Domain.Kernel
- no reflection-based dispatch at runtime
- must not alter program behavior

## Role in System

- consumed by Dx.Domain.Analyzers for scope and construction authority checks
- consumed by Dx.Domain.Generators for code generation
- ignored by the runtime
