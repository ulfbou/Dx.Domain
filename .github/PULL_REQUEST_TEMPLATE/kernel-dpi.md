# Dx.Domain PR Review — Design Pressure Index

## DPI Checklist
- [ ] Enforces an invariant
- [ ] Removes accidental complexity
- [ ] Increases what the compiler can prove
- [ ] Makes misuse impossible
- [ ] Upholds the Manifesto
- [ ] Violates no Non‑Goals
- [ ] Correct placement under DPI (`Core` / `Adapter` / `Generator` / `Analyzer`)

---

## Decision Rule
- If any box is unchecked → not `Core`
- If any Non‑Goal is violated → rejected
- If unsure → move it out

---

## PR Summary
<!-- One‑line summary: what changed and why this is the right place. -->

---

## Kernel / Abstractions / Analyzer Scope
> Reference:
> - `docs/internal/specs/kernel.refactorization.specification.md`
> - `docs/internal/governance/kernel.admission-test.md`

- [ ] This PR does **not** touch Kernel / Abstractions / Analyzers internals.
- [ ] This PR **does** touch one or more of:
  - [ ] `Dx.Domain.Abstractions`
  - [ ] `Dx.Domain` (Kernel)
  - [ ] `Dx.Domain.Analyzers`

If the first box is checked, you may skip the remaining Kernel‑specific checklist.

---

## Kernel Admission Checklist (if applicable)
- [ ] Abstractions purity (no runtime logic, control flow, or extensions)
- [ ] No new Abstractions → Kernel/Analyzers dependencies
- [ ] Kernel purity (no I/O/logging/DI/orchestration)
- [ ] No convenience facades added to Kernel
- [ ] Identities follow rules (readonly struct, guarded creation, `IIdentity`, no implicit conversions)
- [ ] Results & invariants semantics preserved
- [ ] Facts/causation remain structural (no dispatch/behavior)
- [ ] Diagnostics canon respected (codes naming/grouping)

If any item cannot be checked, document under **Deviations**.

---

## Deviations from Kernel Admission Test (if any)
- _Deviations:_

---

## Testing
- [ ] Unit tests (`Dx.Domain.*.Tests`) passing
- [ ] Analyzer / generator tests (where relevant)
- [ ] Additional tests (manual/integration) as needed

_Notes:_
