# Copilot Instructions

## General Guidelines
- First general instruction
- Second general instruction
- Repository instructions must be overridden by `docs/kernel.refactorization.specification.md` for this task.
- Changes must align with actual `Dx.Domain.Kernel` APIs and project files, avoiding divergence from real kernel code.

## Code Style
- Use specific formatting rules
- Follow naming conventions

## Project-Specific Rules
- DXA020 should apply to all methods returning Result, but only when the caller ignores the Result (kernel creation should not be flagged).
- `Dx.Domain.Abstractions` is deprecated; use `Dx.Domain.Annotations` instead.
- Changes must align with actual `Dx.Domain.Annotations`, `Dx.Domain.Primitives`, and `Dx.Domain.Facts` APIs and project files, avoiding divergence from real code.
