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

<!-- Author: one-line summary of what changed and why it belongs where you placed it. -->

---

## Kernel / Abstractions / Analyzer Scope

> Reference: [ `docs/kernel.refactorization.specification.md` ](../docs/kernel.refactorization.specification.md) and `docs/kernel.admission-test.md`

- [ ] This PR does **not** touch Kernel / Abstractions / Analyzers internals.
- [ ] This PR **does** touch one or more of:
  - [ ] `Dx.Domain.Annotations`
  - [ ] `Dx.Domain` (Kernel)
  - [ ] `Dx.Domain.Analyzers`

If the first box is checked, you may skip the remaining Kernel-specific checklist.

---

## Kernel Admission Checklist (if applicable)

Confirm that you have reviewed and satisfied the relevant items in:

- [ `docs/kernel.admission-test.md` ](../docs/kernel.admission-test.md)

Then tick all that apply:

- [ ] Annotations purity: no runtime logic, control flow, or extension methods were added to `Dx.Domain.Annotations`.
- [ ] Assembly boundaries: no new dependency from Abstractions to Kernel or Analyzers was introduced.
- [ ] Kernel purity: no I/O, logging, DI, or orchestration logic was added to `Dx.Domain`.
- [ ] Facade separation: no new ergonomic facade APIs were added to Kernel; any such APIs live in an outer package.
- [ ] Identities: new/modified identity primitives comply with the identity rules (readonly struct, guarded construction, `IIdentity`, no implicit conversions).
- [ ] Results & invariants: changes to `Result`, `Invariant`, `Require`, or error types respect the canonical semantics (failures as values, invariants throw only `InvariantViolationException`).
- [ ] Facts & causation: changes to `Fact{TPayload}`, `Causation`, or `TransitionResult{TState}` keep them structural and side-effect free.
- [ ] Diagnostics canon: any new diagnostics or error codes follow the existing naming and grouping rules.

If any item above cannot be checked, describe why under **Deviations from Kernel Admission Test**.

---

## Deviations from Kernel Admission Test (if any)

> Describe any intentional deviations from `docs/kernel.admission-test.md`.
> These are treated as **defects to be resolved**, not new precedents.

- _Deviations:_

---

## Testing

Describe the testing you performed:

- [ ] Unit tests (`Dx.Domain.*.Tests`) run and passing.
- [ ] Analyzer / generator tests updated where relevant.
- [ ] Additional testing (manual / integration) as needed.

Details:

- _Tests executed:_

---

## Additional Notes

Optional context for reviewers (migration notes, follow-ups, links to design docs, etc.):

- _Notes:_
