# Alpha Post-Merge Documentation Conformance Audit

**Issue:** `docs(alpha): correct post-merge documentation conformance gaps`
**Base audited:** `release/0.1.0-alpha` after PR #64
**Allowed results:** `met`, `partially met`, `not met`, `not applicable`

This audit is the mandatory first implementation artifact. A criterion marked `partially met` or `not met` identifies the evidence, correction, and proving validation. The final review must update every applicable criterion to `met` before the pull request is finalized.

## Confirmed P0 defects

| ID | Initial result | Evidence | Required correction | Proving validation |
| --- | --- | --- | --- | --- |
| P0.1 | met | The descriptor-derived generated inventory contains every shipped descriptor field for all 11 shipped diagnostics, including DXA065. | Completed in Phase 4. | `python3 scripts/verify-diagnostic-conformance.py` passes. |
| P0.2 | met | DXA065 is present in the retained index, TOC, router, generated inventory, package page, changelog, and release notes. | Completed in Phases 3 and 4. | Diagnostic conformance and documentation-link validation pass. |
| P0.3 | partially met | Canonical public pages require explicit analyzer installation, while retained legacy guidance contradicts it. | State explicit alpha installation consistently. | Repository-wide claim audit passes. |
| P0.4 | partially met | Retained configuration prose overstates suppression as impossible. | Distinguish compiler mechanisms from repository policy. | Repository-wide claim audit passes. |
| P0.5 | partially met | Retained pages use unqualified frozen-API language. | Use provisional alpha stability language unless a verified baseline supports a stronger claim. | Stability claim audit passes. |
| P0.6 | partially met | Workflows use `main`, `master`, and the alpha branch for different purposes. | Document default, integration, validation, publication, and release gates separately. | Workflow-to-documentation review passes. |
| P0.7 | not met | `docfx.yml` duplicates publication upload and deployment steps. | Keep one artifact upload and one deployment operation. | Workflow structure check passes. |
| P0.8 | partially met | Maintainer release prose does not map every active branch gate and trigger. | Rewrite from executable workflow files. | Maintainer workflow audit passes. |
| P0.9 | not met | `docs-snippets-compile.sh` performs brace counting and labels it structure validation. | Keep structural validation honestly named and add real compilation for the material Quickstart. | Clean Quickstart project build passes. |

## Issue #55 audit

| Criterion | Initial result | Evidence and required action |
| --- | --- | --- |
| One coherent public information architecture | met | `docs/public` is the public DocFX content root. |
| Public Markdown links resolve | partially met | Run lint after repairing legacy and DXA065 routes. |
| Public DocFX TOC targets resolve | partially met | Rebuild DocFX after navigation corrections. |
| No obsolete or empty published page remains | met | Public lint reported 41 substantive pages after PR #64. |
| Landing page explains the product quickly | met | `docs/public/index.md` provides status, start path, enforcement boundaries, and reference routes. |
| Getting Started documents prerequisites and actual installation | partially met | Canonical page is correct; retained legacy installation prose must be reconciled. |
| Supported target frameworks match implementation | met | Public package pages distinguish runtime .NET 8/9/10 from .NET Standard 2.0 packages. |
| Analyzer installation and minimum configuration match shipped behavior | partially met | Explicit installation is canonical; contradictory legacy guidance must be removed. |
| Quickstart uses shipped namespaces and signatures | partially met | Source inspection supports the sample; compilation evidence is still required. |
| Quickstart is compile-clean in a clean consumer project | not met | Structural fence inspection is not compilation; add an executable Quickstart project. |
| A failing diagnostic can be triggered and corrected | partially met | Diagnostic pages describe remediation, but a clean consumer proof is required. |
| Troubleshooting covers required areas | met | `docs/public/troubleshooting.md` covers analyzer, classification, Result, construction, and suppression concerns. |
| Internal governance is excluded from the public site | met | `docfx.public.json` selects public content only. |

## Issue #56 audit

| Criterion | Initial result | Evidence and required action |
| --- | --- | --- |
| Documentation exists for all five shipped packages | met | Five package pages exist under `docs/public/packages`. |
| Package IDs match generated artifacts | partially met | Verify packed artifacts during final validation. |
| Target frameworks match package metadata | partially met | Prose matches project intent; inspect generated packages. |
| Dependencies match evaluated project graph | partially met | Architecture summary exists; record evaluated graph evidence. |
| Public types and namespaces match shipped APIs | partially met | Source-based pages exist; compile material examples. |
| Analyzer packaging and installation are accurate | partially met | Canonical explicit-installation guidance conflicts with retained legacy prose. |
| Applicable packages have compile-tested examples | not met | No package-level compilation harness proves the claim. |
| Alpha stability is explicit | met | Package pages mark public API and composition provisional. |
| Related diagnostics are linked | partially met | Re-run full link validation after DXA065 reconciliation. |
| Planned functionality is not described as shipped | met | DXA090 is planned; shipped inventory excludes it. |
| Package links and TOC entries resolve | partially met | Re-run lint and DocFX build. |

## Issue #57 audit

| Criterion | Initial result | Evidence and required action |
| --- | --- | --- |
| Public diagnostic IDs equal packaged descriptors | met | Executable descriptor comparison proves the same 11 shipped IDs across generated and public inventories. |
| Default severities equal descriptors | met | Executable comparison validates every public page and index severity against its descriptor. |
| S0-S3 applicability matches tested behavior | met | The Phase 4 scope matrix records implemented scope and the verifier guards the source-derived scope contract; analyzer tests pass. |
| DXA065 is documented as shipped | met | DXA065 appears in every applicable shipped inventory and route. |
| DXA090 is planned and unshipped | met | Documentation authority and public reference separate it from shipped diagnostics. |
| Every shipped diagnostic has actionable remediation | met | Public diagnostic pages include remediation sections. |
| Failing and corrected examples exist where meaningful | not met | Current pages generally describe rather than compile paired examples. |
| Suppression mechanisms and policy are distinguished | partially met | Canonical pages are correct; retained configuration prose is contradictory. |
| Known analyzer limitations are stated | met | Public diagnostic pages include limits. |
| Router and individual references agree | met | Descriptor-derived router, TOC, generated inventory, public index, and individual pages contain the same shipped ID set. |
| Planned diagnostics cannot be mistaken for shipped behavior | met | DXA090 is excluded from the shipped public table. |

## Issue #58 audit

| Criterion | Initial result | Evidence and required action |
| --- | --- | --- |
| Results and errors guidance matches shipped APIs | partially met | Claims are plausible; compile the material examples. |
| Invariant guidance matches actual enforcement | met | Runtime execution limits are stated. |
| Construction-authority guidance is analyzer-safe | partially met | Guidance states the boundary; focused consumer compilation is required. |
| S0-S3 terminology is consistent | partially met | Canonical pages agree; retained trees require claim audit. |
| Structural facts are distinguished from domain events | met | Public concepts explicitly deny event-bus semantics. |
| Analyzer configuration is task-oriented and usable | partially met | Configuration is present; prove values against analyzer behavior. |
| Result handling and propagation use shipped APIs | partially met | Compile the documented patterns. |
| Diagnostic-response guidance links to the authority | met | Guide routes to the public diagnostic index. |
| Pages identify audience, purpose, prerequisites, and stability | not met | Several substantive concept and guide pages do not provide all four fields. |
| Examples are compile-tested or explicitly conceptual | not met | Structural inspection does not satisfy compilation. |
| No blank page remains publicly navigable | met | Public lint reports substantive pages. |

## Final-review gate

Before PR finalization:

1. repeat every row above against the changed repository;
2. change each applicable result to `met` only when its proving validation has run;
3. retain command output as PR evidence;
4. do not convert an unverified criterion to `met`;
5. keep this issue distinct from #50 and cross-reference shared files without absorbing unrelated normalization work.
