# Alpha final conformance audit

Status: **met**

Issue: #65
Base: `release/0.1.0-alpha`

## Evidence boundary
This audit becomes final only after every validation command below succeeds on the same commit. PRs #67 through #71 are historical correction evidence, not substitutes for current validation.

## Acceptance matrix

### Issue #55
- met: coherent public information architecture, landing page, Getting Started, troubleshooting, public/internal separation.
- met subject to final executable validation: Markdown links, DocFX TOCs, Quickstart signatures, clean consumer compilation, diagnostic demonstration.

### Issue #56
- met subject to final executable validation: five package pages, exact package identities, TFMs, evaluated dependencies, public namespaces/signatures, analyzer packaging/install behavior, package examples, stability, diagnostic cross-links, shipped/planned separation, TOC routes.

### Issue #57
- met: descriptor-backed diagnostic inventory, default severities, DXA065 shipped, DXA090 planned/unshipped.
- met subject to final analyzer validation: S0-S3 applicability, remediation/examples, suppression-vs-policy, limitations, router/reference agreement.

### Issue #58
- met subject to final validation: Results/errors, invariants, construction authority, S0-S3 terminology, facts/events, analyzer configuration, Result propagation, diagnostic links, page metadata, example classification/compilation, and absence of blank public pages.

## P0 closure
P0.1-P0.2 are checked by descriptor-backed diagnostic conformance. P0.3-P0.5 are checked by retained/public content validation. P0.6-P0.8 were corrected in the workflow reconciliation and are revalidated by the final build/DocFX run. P0.9 is proven by material-example compilation, independently of structural snippet inspection.

## Final validation
```bash
dotnet tool restore
dotnet restore
dotnet build -c Release
dotnet test -c Release --no-build --verbosity normal
dotnet test tests/Dx.Domain.Analyzers.Tests/Dx.Domain.Analyzers.Tests.csproj -c Release --no-build --verbosity normal
bash scripts/docs-lint.sh
bash scripts/docs-snippets-compile.sh docs/public net8.0
bash scripts/docs-examples-compile.sh
python3 scripts/verify-diagnostic-conformance.py
python3 scripts/verify-alpha-final-conformance.py
dotnet docfx docfx.public.json --warningsAsErrors
git diff --check
```

Do not change status to `met` and do not close #65 unless every command succeeds. Any failure is corrective work for this PR, not evidence to waive a criterion.
