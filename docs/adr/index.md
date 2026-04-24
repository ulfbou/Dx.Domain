# Architecture Decision Records

This index provides a single front door to all Dx.Domain decisions.

| ADR | Title | Status | Date | Enforcement |
| --- | --- | --- | --- | --- |
| [ADR-0001](adr-0001-utc-only-domaintime.md) | Temporal Authority (UTC / DomainTime) | Accepted | 2026-01-10 | [DXA050](../analyzers/dxa050.md) — Strong |
| [ADR-0002](adr-0002-empty-correlationid.md) | Empty CorrelationId Permitted | Accepted | 2026-01-16 | Runtime invariant — Partial |
| [ADR-0003](adr-0003-dxa010-warning.md) | Construction Authority | Accepted | 2026-01-17 | [DXA010](../analyzers/dxa010.md)/[DXA011](../analyzers/dxa011.md)/[DXA080](../analyzers/dxa080.md) — S1–S3 only |
| [ADR-0004](adr-0004-result-struct.md) | Result<T> Uses Struct Not Class | Accepted | 2026-01-18 | Compiler + [DXA040](../analyzers/dxa040.md) — Strong |
| [ADR-0005](adr-0005-no-public-setters.md) | No Public Setters on Domain Types | Accepted | 2026-01-19 | Design guideline — Moderate |
| [ADR-0006](adr-0006-result-as-failure-model.md) | Result as Failure Model | Accepted | 2026-01-29 | [DXA040](../analyzers/dxa040.md) — Strong |
| [ADR-0007](adr-0007-system-hardening-sequence.md) | System Hardening Sequence | Accepted | 2026-01-29 | Process |
| [ADR-0008](adr-0008-dxa011-public-factory-exposure.md) | DXA011 Public Factory Exposure | Accepted | 2026-02-01 | [DXA011](../analyzers/dxa011.md) — S1–S3 only |
| [ADR-0009](adr-0009-dxa020-result-ignored.md) | DXA020 Result Ignored | Accepted | 2026-02-01 | [DXA020](../analyzers/dxa020.md) |
| [ADR-0010](adr-0010-dxa022-domain-control-exception.md) | DXA022 No Throw | Accepted | 2026-02-01 | [DXA022](../analyzers/dxa022.md) |
| [ADR-0011](adr-0011-dxa030-unapproved-handler.md) | DXA030 Unapproved Handler | Accepted | 2026-02-01 | [DXA030](../analyzers/dxa030.md) |
| [ADR-0012](adr-0012-dxa040-kernel-public-surface-freeze.md) | DXA040 Surface Freeze | Accepted | 2026-02-01 | [DXA040](../analyzers/dxa040.md) |
| [ADR-0013](adr-0013-dxa050-temporal-helper-usage.md) | DXA050 Temporal Helpers | Accepted | 2026-02-01 | [DXA050](../analyzers/dxa050.md) |
| [ADR-0014](adr-0014-dxa060-forbidden-vocabulary.md) | DXA060 Forbidden Vocabulary | Accepted | 2026-02-01 | [DXA060](../analyzers/dxa060.md) |
| [ADR-0015](adr-0015-dxa070-generated-code-tagging.md) | DXA070 Generated Code | Accepted | 2026-02-01 | [DXA070](../analyzers/dxa070.md) |
| [ADR-0016](adr-0016-dxa080-facade-invariant-enforcement.md) | DXA080 Facade Enforcement | Accepted | 2026-02-01 | [DXA080](../analyzers/dxa080.md) |
| [ADR-0017](adr-0017-suppress-governance.md) | Suppression Governance | Accepted | 2026-04-22 | Process |
| [ADR-0018](adr-0018-kernel-public-surface.md) | Kernel Public Surface Contract | Accepted | 2026-04-23 | DXA040 + S0 exemption — Strong |

## How to read these ADRs

Each ADR now includes:
- **Enforcement Coverage** — which analyzer or mechanism enforces it
- **Coverage Level** — Strong, Moderate, Partial, or Process
- **Known Gaps / Bypass Vectors** — what is not guaranteed
- **Guarantee** — what you can rely on
- **Dependencies** — links to other ADRs

This structure addresses the professional reader gaps identified earlier: single source of truth, trust signals, and decision-making information.

## Navigation
- [Public Overview](../public/overview.md)
- [Manifesto](../manifesto.md)

