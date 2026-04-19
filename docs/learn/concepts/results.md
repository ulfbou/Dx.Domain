# Results

1. Purpose: Explain the mechanical discipline behind Result<T> and why explicit handling is required.
2. When to use:
   - Modeling operations that can fail in predictable ways
   - Returning domain outcomes without using exceptions for control flow
3. When NOT to use:
   - For unexpected infrastructure failures
   - For performance-critical hot paths that require exceptions
4. Guarantees:
   - Result is a value type that is never null in normal use
   - Success and failure are mutually exclusive
5. Constraints:
   - Analyzers enforce handling via DXA020
   - Only approved handlers count as handling

## What a Result is

Result<T> carries either a success value of type T or a DomainError. Result without T represents success with no value. Both are immutable and comparable by value.

## Why explicit handling

Silent failures are a common source of bugs. Dx.Domain makes handling explicit at compile time. If a Result is produced and not observed, the build warns or errors depending on your severity configuration.

## Approved handling patterns

- Return it to the caller
- Transform it with Map or Bind
- Terminalize it at a boundary with Match
- Observe it with an approved handler such as Tee

These patterns satisfy the analyzer. Anything else is treated as discarded.

## Relationship to errors

DomainError is a stable value with Code and Message. It is created via DomainError.Create and propagated through Result.Failure. Mapping between error types is explicit via MapError or ToDomainError.

## Related diagnostics

- DXA020 Result must be handled
- DXA030 Exceptions in Result-returning methods
- DXA040 Discard without intent

## Related links

- Up: concepts/index.md
- Guide: guides/handle-errors.md
- Reference: reference/diagnostics/DXA020.md
