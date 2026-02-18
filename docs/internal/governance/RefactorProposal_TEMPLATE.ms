<!-- path: docs/internal/governance/RefactorProposal_TEMPLATE.md -->
---
id: refactor-proposal-template
title: Refactor Proposal Template
status: Draft
audience: Contributors
owners: [AnalyzersOwner]
---

# Refactor Proposal — <Short Title>

## 1. Problem Statement
Describe the specific deficiency (correctness, stability, performance, compatibility, ergonomics).

## 2. Proposed Change
Summarize code/doc changes. **No public API or diagnostic behavior changes** unless justified.

## 3. Why This Elevates the Framework
- Correctness: <measurable effect>
- Stability: <impact>
- Performance: <expected % or absolute budget>
- Compatibility: <no breaks / opt-in path>
- Developer ergonomics: <reduced friction / clearer behavior>

## 4. Risk Assessment
- Technical risks
- False positive/negative risk (for analyzers)
- Mitigations

## 5. Rollback Plan
How to revert safely.

## 6. Estimated Effort
Story points: <N>

## 7. Test Plan
- Unit tests (diagnostics & code‑fixes if any)
- Integration (scope resolution across S0–S3)
- Performance (per‑method analysis time)
- Compatibility (PublicAPI baseline, golden diagnostics)

## 8. Required Approvals
- Analyzers Owner
- Kernel Owner (if DXA040 or Kernel‑touching)
- Release Manager

## 9. Acceptance Criteria
Concrete, testable conditions to declare success.

## 10. Links
- Rule Catalog: docs/internal/rules/analyzers/rule-catalog.md  
- Non‑Silence Axiom: docs/internal/governance/non-silence-axiom.md
