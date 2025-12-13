# Dx.Domain Non-Goals

## A Declaration of Permanent Refusal

Dx.Domain is defined as much by what it excludes as by what it enforces.  
These are not “out of scope for now.”  
These are structural refusals.

---

## What Dx.Domain Will Never Be

### A General-Purpose Utility Library

Dx.Domain will not accumulate helpers, extensions, or convenience methods.  
If a construct does not enforce domain correctness, invariants, or semantic clarity, it does not belong.

### A DDD Pattern Museum

Dx.Domain does not re-implement aggregates, repositories, factories, or services as textbook abstractions.  
Patterns are emergent consequences of constraints, not APIs to be memorized.

### A Persistence Framework

Databases are replaceable.  
Serialization formats are replaceable.  
Invariants are not.  

Persistence exists only as adapters, never as a design center.

### A Runtime-First Safety Net

Correctness is not deferred to execution.  
Runtime validation exists only where the compiler cannot reach — and that boundary must be explicit and minimal.

### A Convenience Layer

Dx.Domain does not optimize for “easy.”  
Ergonomics serve correctness, never the reverse.  
If a shortcut weakens guarantees, it is rejected outright.

### A Kitchen Sink

No creeping scope.  
No “while we’re here.”  

Time, causation, invariants, results, values — this is the spine.  
Everything else lives outside or on top.

---

## Why These Non-Goals Exist

They prevent drift.  
They prevent accidental authority.  
They prevent the slow erosion of a clean kernel into a bloated framework.

Any proposal that violates these Non‑Goals is not a “feature request.”  
It is a request to change the project’s identity — and must be treated as such.
