# Architecture Decision Records

This index provides historical records of Dx.Domain design decisions. Each ADR documents the reasoning and context at the time of decision. **For current enforcement guarantees, enforcement strengths, and formal specifications, see [Dx.Domain Enforcement Specification](../enforcement-specification.md).**

| ADR | Title | Status | Date | Historical Decision |
| --- | --- | --- | --- | --- |
| [ADR-0001](adr-0001-utc-only-domaintime.md) | Temporal Authority (UTC / DomainTime) | Accepted | 2026-01-10 | DomainTime.Utc only; enforced by [DXA050](../../reference/diagnostics/DXA050.md) |
| [ADR-0002](adr-0002-empty-correlationid.md) | Empty CorrelationId Permitted | Accepted | 2026-01-16 | Runtime invariant enforcement for CorrelationId |
| [ADR-0003](adr-0003-dxa010-warning.md) | Construction Authority | Accepted | 2026-01-17 | Facade pattern required; enforced by [DXA010](../../reference/diagnostics/DXA010.md)/[DXA011](../../reference/diagnostics/DXA011.md)/[DXA080](../../reference/diagnostics/DXA080.md) |
| [ADR-0004](adr-0004-result-struct.md) | Result<T> Uses Struct Not Class | Accepted | 2026-01-18 | Result as value type; enforced by compiler and [DXA040](../../reference/diagnostics/DXA040.md) |
| [ADR-0005](adr-0005-no-public-setters.md) | No Public Setters on Domain Types | Accepted | 2026-01-19 | Readonly enforcement; design guideline |
| [ADR-0006](adr-0006-result-as-failure-model.md) | Result as Failure Model | Accepted | 2026-01-29 | Result as explicit failure propagation |
| [ADR-0007](adr-0007-system-hardening-sequence.md) | System Hardening Sequence | Accepted | 2026-01-29 | Hardening process and validation order |
| [ADR-0008](adr-0008-dxa011-public-factory-exposure.md) | DXA011 Public Factory Exposure | Accepted | 2026-02-01 | Factories internal; enforced by [DXA011](../../reference/diagnostics/DXA011.md) |
| [ADR-0009](adr-0009-dxa020-result-ignored.md) | DXA020 Result Ignored | Accepted | 2026-02-02 | Result values must not be silently ignored; enforced by [DXA020](../../reference/diagnostics/DXA020.md) — see current [enforcement specification](../enforcement-specification.md) for strength |
| [ADR-0010](adr-0010-dxa022-domain-control-exception.md) | DXA022 No Throw | Accepted | 2026-02-01 | Domain failures via Result or DomainError, not exceptions; enforced by [DXA022](../../reference/diagnostics/DXA022.md) |
| [ADR-0011](adr-0011-dxa030-unapproved-handler.md) | DXA030 Unapproved Handler | Accepted | 2026-02-01 | Result handlers must be registered; enforced by [DXA030](../../reference/diagnostics/DXA030.md) |
| [ADR-0012](adr-0012-dxa040-kernel-public-surface-freeze.md) | DXA040 Surface Freeze | Accepted | 2026-02-01 | Kernel public surface frozen; enforced by [DXA040](../../reference/diagnostics/DXA040.md) with baseline |
| [ADR-0013](adr-0013-dxa050-temporal-helper-usage.md) | DXA050 Temporal Helpers | Accepted | 2026-02-01 | UTC-only time enforcement; enforced by [DXA050](../../reference/diagnostics/DXA050.md) |
| [ADR-0014](adr-0014-dxa060-forbidden-vocabulary.md) | DXA060 Forbidden Vocabulary | Accepted | 2026-02-07 | Forbidden terms prevent semantic expansion; enforced by [DXA060](../../reference/diagnostics/DXA060.md) — see current [enforcement specification](../enforcement-specification.md) for strength |
| [ADR-0015](adr-0015-dxa070-generated-code-tagging.md) | DXA070 Generated Code | Accepted | 2026-02-08 | Generated code must be marked; enforced by [DXA070](../../reference/diagnostics/DXA070.md) — see current [enforcement specification](../enforcement-specification.md) for strength |
| [ADR-0016](adr-0016-dxa080-facade-invariant-enforcement.md) | DXA080 Facade Enforcement | Accepted | 2026-02-09 | Facade methods enforce invariants; enforced by [DXA080](../../reference/diagnostics/DXA080.md) — see current [enforcement specification](../enforcement-specification.md) for strength |
| [ADR-0017](adr-0017-suppress-governance.md) | Suppression Governance | Accepted | 2026-04-22 | Suppression discipline and audit process |
| [ADR-0018](adr-0018-kernel-public-surface.md) | Kernel Public Surface Contract | Accepted | 2026-04-23 | Kernel public surface contract and versioning |

## How to read these ADRs

Each ADR documents:
- **Decision:** The choice made and its context at the time
- **Rationale:** Why this decision was made
- **Enforcement Coverage:** Which analyzer or mechanism implements the decision (as of that ADR date)
- **Known Gaps / Bypass Vectors:** Limitations that were known or discovered

**Important:** ADRs are historical records of decisions. They are not automatically current authority:
- Enforcement strengths, guarantees, and scope may have been refined since the ADR was written
- For the current state of enforcement, see [Dx.Domain Enforcement Specification](../enforcement-specification.md)
- For explanatory content on how enforcement works, see [Enforcement Model](../enforcement-model.md)
- Reference diagnostic pages link to both the ADR and current specification for traceability

## Navigation
- [Public Overview](../../index.md)
- [Manifesto](../manifesto.md)

