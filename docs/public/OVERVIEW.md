# Dx.Domain — Overview

Dx.Domain is a small, opinionated substrate providing invariants, results, errors, identities, and structural history — designed so that **incorrect domain modeling is difficult or impossible to express**.

If code **compiles**, **passes analyzers**, and the **Kernel accepts it**, the state is valid.

This document is the high‑level public overview.  
Internal governance documents (Manifesto, Non‑Goals, DPI, ADRs, Spec) live under `docs/internal/` and are not published publicly.

---

## Philosophy

Dx.Domain enforces a strict architectural separation:

- **Annotations (Abstractions)**: semantic vocabulary, attributes, markers  
- **Primitives**: immutable identity/value types  
- **Kernel**: runtime judge of invariants, results, errors, facts  
- **Facts**: structural lineage, meaning‑agnostic history

Consumers of the library use these components to model valid states, transitions, and errors.

The Kernel is intentionally minimal, explicit, and highly disciplined.

---

## Rationale (Public Summary)

This repository’s internal governance establishes:

- Clear layer boundaries  
- Mandatory analyzers enforcing correctness  
- Result‑based error semantics  
- Explicit invariants  
- Frozen Kernel surface area  
- Deterministic runtime semantics  

The public overview remains intentionally brief.  
Full rationale and governance remain internal (`docs/internal/*`).

---

## The Repository Structure (Public Summary)

The repository contains the following primary packages:

- `Dx.Domain.Annotations`  
- `Dx.Domain.Primitives`  
- `Dx.Domain.Kernel`  
- `Dx.Domain.Facts`  

Additional packages such as analyzers and generators reinforce discipline but are not part of the public runtime surface.

---

## Using Dx.Domain (Sketch)

```csharp
using Dx.Domain.Kernel;

public static Result<OrderId> CreateOrder(string? customerId)
{
    if (string.IsNullOrWhiteSpace(customerId))
        return Result.Failure<OrderId>(DomainErrors.CustomerIdMissing);

    return OrderId.New(customerId);
}
````

`Result<T>` and `DomainError` are designed to cross boundaries (APIs, persistence, tests).  
The Kernel enforces invariants and provides structural correctness primitives.

***

## Documentation

Public documentation includes:

*   This overview (`OVERVIEW.md`)
*   The architecture overview (`architecture-overview.md`)
*   Public changelog and release notes

Internal documents (Manifesto, Non‑Goals, DPI, ADRs, kernel spec, governance laws) are present in the repository but **not published publicly**.

***

## Stability and Evolution

If a feature does not belong in the Kernel, it may fit as:

*   an analyzer
*   a source generator
*   a persistence adapter
*   or outside this repository entirely

The Kernel remains small, strict, and stable.

***

## License

Licensed under the MIT License.  
See ./LICENSE for details.
