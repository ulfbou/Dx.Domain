# Documentation authority

**Status:** Normative for repository documentation.

## Canonical structure

- `docs/use/`: adoption, tasks, troubleshooting, limitations, stability, security, and releases.
- `docs/understand/`: architecture, concepts, enforcement, normative specifications, historical decisions, and roadmap.
- `docs/contribute/`: contribution, validation, authoring, CI/CD, and release procedures.
- `docs/reference/`: packages, diagnostics, configuration, and API lookup.
- `docs/index.md`: the single documentation entry page.
- `docs/toc.yml`: the single navigation authority.
- `docs/docfx.json`: the single DocFX configuration for authored documentation.

The public site is built from `docs/` through `docs/docfx.json` and `docs/toc.yml`. `docs/internal/` is transitional, excluded from publication, and reserved for Phase 3 reconciliation. No parallel public, learn, internal, analyzer, ADR, or specification tree is authoritative.

## Normative precedence

1. Compiled behavior and package metadata
2. Analyzer descriptors and automated tests
3. Current normative specifications
4. Current reference documentation
5. Explanatory concepts and guides
6. Historical ADRs
7. Roadmap and commentary

The [enforcement specification](../understand/enforcement-specification.md) supersedes conflicting ADR interpretation. Lower-precedence material must be corrected when it conflicts with a higher authority.

Generated artifacts are implementation outputs, not independent documentation authorities.
