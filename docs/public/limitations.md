# Limitations

Dx.Domain provides compiler-assisted architectural governance, not formal verification.

## Analyzer boundary

Analyzers inspect statically visible source and symbols in participating compilations. They do not comprehensively cover:

- reflection and dynamic invocation,
- ORM or serializer materialization,
- code compiled without the analyzer package,
- behavior across opaque assembly boundaries,
- transitive correctness of Result handling,
- completeness of invariants,
- semantic validity of business rules.

## Runtime boundary

Readonly structs can still have a `default` value. Reflection and serializers may bypass intended factories. `Invariant.That` can detect a failing condition only when execution reaches the check.

## Alpha boundary

Package composition, APIs, configuration keys, analyzer detection, and documentation may evolve. No long-term support window or response-time commitment exists for this alpha.
