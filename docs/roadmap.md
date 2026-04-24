# Roadmap

**Last reviewed:** 2026-04-22  
**Current version:** 0.1.0-alpha.1

## Philosophy

Roadmap items are classified by their effect on guarantees, not by feature completeness.

- **Committed:** Will be in 0.1.0 stable. Changes core guarantees.
- **Experimental:** May be removed. Explores enforcement mechanisms.
- **Deferred:** Valuable but violates Non-Goals if placed in core.

## Committed for 0.1.0 stable

### 1. Stabilize primitive shapes
**Problem:** Current struct layouts may change.  
**Goal:** Freeze public surface of CorrelationId, TraceId, ActorId, FactId, SpanId.  
**Trade-off:** Locks in representation, limits future optimization.

### 2. `Result<T>` API finalization
**Problem:** Match and Map signatures are provisional.  
**Goal:** Define stable API for success/failure handling.  
**Trade-off:** Verbosity accepted for explicitness.

### 3. DXA010 to Error severity
**Problem:** Construction authority currently warns, allowing bypass.  
**Goal:** Make direct construction a build error outside kernel.  
**Trade-off:** Breaks existing experiments, forces factory adoption.

## Experimental

### Analyzer for unchecked Result.Value
**Problem:** Callers can ignore `Result` and access .Value directly.  
**Exploration:** Roslyn analyzer to flag unchecked access.  
**Risk:** May produce false positives in legitimate scenarios.

### Test clock for DomainTime
**Problem:** `DomainTime.Now()` is non-deterministic in tests.  
**Exploration:** Internal test clock injection, not exposed publicly.  
**Risk:** Could leak into public API if not carefully bounded.

### Generator for ID factories
**Problem:** Manual factory implementation is repetitive.  
**Exploration:** Source generator that emits Create methods with invariant checks.  
**Risk:** Generated code must remain visible and auditable.

## Deferred (intentionally)

- **EF Core integration:** Violates [Non-Goal #3](NON_GOALS.md#3-a-persistence-framework) (Persistence Framework). Belongs in adapter, not core.
- **JSON converters in core:** Violates [Non-Goal #1](NON_GOALS.md#1-a-general-purpose-utility-library) (Utility Library). Belongs in separate package.
- **Localization of errors:** Violates [Manifesto](manifesto.md) demand for centralized error semantics. Application concern.

## What will not change

These are philosophically fixed, per [Manifesto](manifesto.md):
- Strongly typed identities will remain
- `Result` over exceptions for domain failure will remain
- Construction authority will remain
- UTC-only time will remain

Mechanics may evolve. Principles will not.
