# Dx.Domain — Architecture Overview

**Audience**: consumers and integrators of Dx.Domain  
**Status**: Informational (summarizes the normative spec)

## What Dx.Domain Is
A small, opinionated **substrate** for invariants, results, errors, identities, and structural history — designed so **incorrect domain models are hard to express**. If it **compiles**, **passes analyzers**, and the **Kernel accepts it**, the state is valid. 

## The Four Packages
- **Annotations** — pure vocabulary and metadata; **no runtime logic**. 
- **Primitives** — immutable, side-effect-free value types (IDs, tracing). 
- **Kernel** — the runtime **judge** of invariants, results, errors, facts; no I/O/infrastructure. 
- **Facts** — structural, lineage-aware, **meaning-agnostic** history; **not** domain events. 

## Dependency Rules (strict)
- Kernel **must** depend on Annotations; **must not** depend on Analyzers.  
- Analyzers depend on Annotations; never on Kernel at runtime.  
- Meta-packages may bundle; assemblies remain decoupled. 

## Analyzer Governance (Non-Silence)
Analyzers are mandatory: you cannot disable them or downgrade `DX*` diagnostics via `.editorconfig` or `#pragma`; attempts fail the build. Non-test projects treat `DX*` diagnostics as errors. Governance is imported once from `Directory.Build.targets`. 

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
