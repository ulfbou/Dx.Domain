## [ADR-0010](../adr/adr-0010-dxa022-domain-control-exception.md): [DXA022](../analyzers/dxa022.md) Domain Control Exception

**Status:** Accepted  
**Date:** 2026-02-03

### Context
Exceptions for domain control flow conflate expected failures with system faults.

### Decision
Implement [DXA022](../analyzers/dxa022.md) to flag exceptions used for domain control flow.

Triggers:
- Throw statements in domain layer with business-rule exception types
- Catch blocks handling domain exceptions as control flow
- Excludes system exceptions (ArgumentNullException, InvalidOperationException for bugs)

Severity: Warning

---

### Enforcement Coverage
**Enforced by:** [DXA022](../analyzers/dxa022.md)

**Coverage Level:** **Moderate**

---

### Enforcement Model
- **Type:** Static analyzer
- **Scope:** Domain layer
- **Strength:** Moderate

---

### Bypass Vectors
- Throwing base Exception types
- Infrastructure layer throws

---

### Guarantee
Expected failures use Result, not exceptions.

### Dependencies
- [ADR-0006](../adr/adr-0006-result-as-failure-model.md)

### Consequences
Forces Result<T>; separates failures from faults; requires refactoring.

### DPI Alignment
Items 2, 3, 5 — reduces complexity, compiler reasoning, errors as values.
