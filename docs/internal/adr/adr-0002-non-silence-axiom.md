# ADR-0002: Non-Silence Axiom (Analyzer Governance)

**Status**: Accepted  
**Date**: 2026-01-29  
**Spec Reference**: Axiom 2; Governance Targets. 

## Context
Analyzer enforcement must never be optional. Configuration supplies **facts**, not switches. Attempts to disable analyzers or downgrade Dx diagnostics create silent failure risks. 

## Decision
- `RunAnalyzers=false` and `EnableNETAnalyzers=false` cause build errors.  
- `.editorconfig` cannot suppress or downgrade `DX*`; attempts fail the build.  
- DX diagnostics are **WarningsAsErrors** for non-test projects.  
- Governance imported once from `Directory.Build.targets` as single choke point. 

## Consequences
- Deterministic, portable enforcement in both first-party and OSS consumption.  
- No “escape hatches” that erode architectural integrity.
