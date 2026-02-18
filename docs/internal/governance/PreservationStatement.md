<!-- path: docs/internal/governance/PreservationStatement.md -->
---
id: preservation-statement
title: Preservation Statement — Dx.Domain.Analyzers
status: Accepted
audience: Maintainers
owners: [AnalyzersOwner]
---

# Preservation Statement — Dx.Domain.Analyzers

**The following must not be altered without explicit approval:**

1) **Rule IDs and semantics**  
   - DXA010 (Construction Authority), DXA020 (Result Ignored), DXA022 (Result vs Throw), DXA040 (Kernel API Freeze), DXK00* family (role/dependency).  
   - *Reason:* stable contracts across many repos; changing IDs, default severities, or scopes risks breaking builds.

2) **Scope model (S0–S3) & resolution facts**  
   - `dx.scope.map`, `dx.scope.rootNamespaces`, `dx_generated_markers` and resolution precedence.  
   - *Reason:* deterministic classification and predictable analyzer behavior.

3) **Non‑Silence Axiom**  
   - No global downgrade/suppression of DX diagnostics via `.editorconfig`.  
   - *Reason:* governance integrity and deterministic builds.

4) **Fail‑open & performance budgets**  
   - < 5ms/method average; infra failures must not hard‑fail analysis.  
   - *Reason:* developer experience and CI stability.

5) **Public analyzer API surface**  
   - Types/namespaces that appear in PublicAPI baseline.  
   - *Reason:* ABI compatibility for downstream tooling; enforced by API baseline checks.
