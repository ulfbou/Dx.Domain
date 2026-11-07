# Documentation Authority

**Status:** Normative for repository documentation
**Release:** `0.1.0-alpha`

## Canonical narrative

Dx.Domain is a small, compiler-assisted substrate for explicit invariants, results, errors, identities, and structural facts.

## Sources of truth

- **Product definition:** this document and the root README.
- **Package identities, target frameworks, dependencies, and public types:** evaluated project files and compiled assemblies.
- **Diagnostic IDs, titles, categories, default severities, and messages:** `DiagnosticDescriptor` declarations in `Dx.Domain.Analyzers`.
- **Enforcement boundary:** `docs/specifications/dx.domain-enforcement-specification.md`, subject to demonstrated implementation behavior.
- **Version:** `version.json`; the alpha documentation uses `0.1.0-alpha`.
- **Security reporting:** root `SECURITY.md`.
- **Release history:** root `CHANGELOG.md` and the matching public release note.

## Precedence

When sources disagree, use this order:

1. Compiled implementation and shipped package metadata
2. Analyzer descriptors and automated tests
3. Normative enforcement specification
4. This authority document
5. Public reference documentation
6. Public concepts and guides
7. Root README and release summaries
8. Historical ADRs and commentary

A lower source must not override a higher source. Contradictions are defects to correct, not alternatives to preserve.

## Resolved alpha decisions

- Consumers install `Dx.Domain.Analyzers` explicitly during alpha. Repository project references do not prove transitive NuGet behavior.
- Analyzer defaults come from descriptors: DXA020, DXA040, and DXA060 are errors; other shipped DXA rules are warnings.
- DXA065 is shipped and belongs in the diagnostic catalog. DXA090 is planned and must not be presented as shipped.
- Suppression is technically possible through standard compiler mechanisms. Repository policy may reject suppression, but documentation must not claim technical impossibility.
- Runtime package APIs are provisional. Architectural principles may be described as stable, but “frozen” is reserved for a verified API baseline.
- Public documentation is built only from `docs/public`. Internal governance remains repository-visible but outside public navigation.
- Security reports use GitHub Security Advisories. There is no public security email address.

## Claim labels

Technical claims use one of these labels where the enforcement mechanism matters:

- **Compiler-enforced**
- **Analyzer-enforced**
- **Runtime-enforced**
- **Process-governed**
- **Planned**
- **Not guaranteed**
