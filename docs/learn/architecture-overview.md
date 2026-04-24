# Dx.Domain — Architecture Overview

**Audience**: consumers and integrators of Dx.Domain  
**Status**: Informational (summarizes the normative spec)

## What Dx.Domain Is
A small, opinionated **substrate** for invariants, results, errors, identities, and structural history — designed so **incorrect domain models are hard to express**. If it **compiles**, **passes analyzers**, and the **Kernel accepts it**, the state is valid. 

## The Four Packages
- **Annotations (Abstractions)** — pure vocabulary and metadata; **no runtime logic**. 
- **Primitives** — immutable, side-effect-free value types (IDs, tracing). 
- **Kernel** — the runtime **judge** of invariants, results, errors, facts; no I/O/infrastructure. 
- **Facts** — structural, lineage-aware, **meaning-agnostic** history; **not** domain events. 

Details: [Annotations](../public/packages/annotations.md) | [Primitives](../public/packages/primitives.md) | [Kernel](../public/packages/kernel.md) | [Facts](../public/packages/facts.md)

## Dependency Rules (strict)
- Kernel **must** depend on Annotations; **must not** depend on Analyzers.  
- Analyzers depend on Annotations; never on Kernel at runtime.  
- Meta-packages may bundle; assemblies remain decoupled. 

## Analyzer Governance (Non-Silence)

Analyzers are mandatory and cannot be suppressed. They implement the spec:

**Construction**
- [DXA010](../public/packages/analyzers.md#dxa010) Construction Authority
- [DXA011](../public/packages/analyzers.md#dxa011) Public Factory Exposure
- [DXA080](../public/packages/analyzers.md#dxa080) Facade Invariant Enforcement

**Result Handling**
- [DXA020](../public/packages/analyzers.md#dxa020) Result Ignored (Error)
- [DXA030](../public/packages/analyzers.md#dxa030) Unapproved Handler

**Exception Discipline**
- [DXA022](../public/packages/analyzers.md#dxa022) No throw in Result methods

**Kernel Governance**
- [DXA040](../public/packages/analyzers.md#dxa040) Kernel Surface Freeze (Error)
- [DXA050](../public/packages/analyzers.md#dxa050) No temporal helpers
- [DXA060](../public/packages/analyzers.md#dxa060) Forbidden vocabulary (Error)

**Code Generation**
- [DXA070](../public/packages/analyzers.md#dxa070) Generated code tagging

## Scope & Authority (How Rules Apply)
- Scope resolution (authoritative → fallback): `dx.layer` → `[assembly: DxLayer("…")]` → assembly name → default Consumer.  
- Authority modes: **Definitional** (Kernel), **Structural** (Primitives/Annotations), **Constraining** (Consumer), **Observational** (Kernel-aware). 

## Two Rule Examples

### DXA010 — Construction Discipline
- **What**: Enforce centralized construction behind an approved boundary (not necessarily a Dx-provided facade).  
- **Where**: Consumer only (Constraining).  
- **How**: Domain type detection precedence (markers → base interfaces → namespace); approval via marker attributes, allow-lists, or known patterns. 

### DXA022 — Result vs Throw Discipline
- **What**: Contract-facing `Result/Result<T>` methods must not throw **domain outcomes**.  
- **Where**: Consumer only (Constraining); Kernel remains definitional/observational.  
- **How**: Conservative classification (markers/base types → domain-error namespace → else allowed). 

## What We Explicitly Exclude
Functional wrappers like `Option`, `Either`, `NonEmpty` are excluded from the Kernel: more governance cost than semantic gain in this architecture. Use **Result** and explicit invariants. 

## What Success Looks Like
- Kernel/Primitives/Annotations build clean under analyzers.  
- Consumer misuse **fails deterministically** at build time. 

