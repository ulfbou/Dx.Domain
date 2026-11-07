# Alpha Diagnostic Metadata and Scope Matrix

**Release:** `0.1.0-alpha`
**Authority:** analyzer descriptors and implemented scope guards
**Related issue:** #65

This matrix records executable descriptor metadata and the currently implemented scope boundary. Historical ADR prose is subordinate when it disagrees with implementation and tests.

| ID | Category | Default | Enabled | Implemented scope |
| --- | --- | --- | --- | --- |
| DXA010 | `Domain.Architecture` | Warning | true | S1, S2, S3 |
| DXA011 | `Domain.Architecture` | Warning | true | S0, S1 |
| DXA020 | `Domain.ResultHandling` | Error | true | S1, S2 |
| DXA022 | `Domain.ExceptionHandling` | Warning | true | S1, S2 |
| DXA030 | `Domain.ResultHandling` | Warning | true | S0, S1, S2 |
| DXA040 | `Domain.Architecture` | Error | true | S0 |
| DXA050 | `Domain.Architecture` | Warning | true | S0, S1 |
| DXA060 | `Domain.Architecture` | Error | true | S0, S1 |
| DXA065 | `Documentation` | Warning | true | S0 public API |
| DXA070 | `Domain.CodeGeneration` | Warning | true | S1, S2 |
| DXA080 | `Domain.Architecture` | Warning | true | S1, S2 |

## Inventory boundary

DXA090 is planned and unshipped. It is intentionally absent from the descriptor-derived inventory.

## Validation

Run `python3 scripts/verify-diagnostic-conformance.py` and the analyzer test project. The verifier compares descriptors, generated JSON, public pages, indexes, routers, package documentation, changelog, and release notes.
