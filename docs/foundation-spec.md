# Foundation Specification (Normative) — Kernel • Primitives • Annotations

**Axioms**
1. **Kernel Authority** — defines invariants, result algebra, error taxonomy, causal structure.
2. **Non‑Silence** — analyzers are mandatory; config conveys **facts**, not switches.
3. **Consumer Discipline** — analyzers constrain *consumers*, never authority layers.

**Assemblies & Dependencies**
- **Annotations** — pure vocabulary; no behavior or control flow.
- **Kernel** — judge of validity/failure; references Annotations; no analyzers; no I/O/logging/policies.
- **Primitives** — immutable identity & tracing value types; parse/format friendly; side‑effect free.

**Kernel Laws**
- No ambient context; results as exclusive flow; diagnostics as data; final public primitives; no policies/opinions.

**Primitives Discipline**
- `readonly struct`, value equality, guarded construction, explicit parse/format, no implicit casts.

**Annotations Purity**
- Sealed attributes with primitive params; immutable records; no reflection helpers.

**Admission Tests** (gate release): purity, scope, identity discipline, result algebra, error canon.
