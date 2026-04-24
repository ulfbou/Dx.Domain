# Dx.Domain.Primitives

Strongly typed domain building blocks.

## Purpose

Provide invariant-safe value representations used by the kernel.

## Guarantees

- immutability
- explicit construction
- structural equality
- no invalid states

## Constraints

- no implicit conversions
- no empty identity values
- no runtime-dependent behavior

## Role in System

- consumed by Kernel
- not responsible for domain orchestration

See:

- `docs/public/packages/primitives.md`

