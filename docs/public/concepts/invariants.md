# Invariants

Use `Invariant.That` for conditions whose violation indicates an invalid program state. Use `Dx.Require` when failure is a recoverable domain outcome represented as a Result. Runtime checks act only on executed paths and do not prove invariant completeness.

See [Limitations](../limitations.md).
