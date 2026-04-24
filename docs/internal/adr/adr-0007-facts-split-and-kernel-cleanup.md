# ADR-0007: Facts Split → Kernel Cleanup → Primitives Tightening → Analyzer Hardening

**Status**: Accepted  
**Date**: 2026-01-29  
**Spec Reference**: Final Validation, Rule §9 Tests, §10 Packaging & Release. 

## Context
To reach a “closed under explanation” architecture, we must split Facts cleanly, finalize Kernel laws, tighten Primitives, and harden analyzers under the layer/authority model. 

## Decision (order of work)
1. **Facts split**
2. **Kernel cleanup**
3. **Primitives tightening**
4. **Analyzer hardening** (DXA010, DXA022; add DXA090 for suppression detection) 

## Consequences
- The substrate is stable; consumer misuse fails deterministically at build time.
