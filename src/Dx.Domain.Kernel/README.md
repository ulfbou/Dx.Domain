# Dx.Domain.Kernel

Functional core for Dx.Domain.

## Purpose

Provide the minimal, frozen set of types required to express domain outcomes without exceptions for control flow.

## Guarantees

- Result<T> must be handled (DXA020)
- no exceptions for domain control flow
- UTC-only time representation
- kernel surface is frozen
- zero external dependencies
