# **Dx.Domain.Analyzers — Release Candidate Specification**

**Status:** Release Candidate (RC)  
**Audience:** Architects, analyzer authors, Foundation maintainers, template authors, CI owners  
**Scope:** Roslyn analyzers enforcing Dx.Domain architectural semantics, role/layer constraints, discipline rules, and template contract validation.

***

# **0. Executive Summary**

**Dx.Domain.Analyzers** is the **compile‑time enforcement engine** of the Dx.Domain architecture. It ensures that domain code is:

*   Structurally correct
*   Layer‑consistent
*   Role‑sound
*   Result‑disciplined
*   Invariant‑aligned
*   Transport‑agnostic
*   Impossible to misuse silently

This analyzer suite is the *only* mechanism capable of enforcing the Dx.Domain “physics engine” across repositories, languages, and environments.

The RC specification defines:

*   The **diagnostic catalog** (DXK\*, DXA\*, DXT\*)
*   The **scope & authority model**
*   Non‑negotiable **governance semantics**
*   Required **infrastructure**, **configuration**, **tests**, and **CI behaviors**
*   Stability and lifecycle rules required for 1.0

***

# **1. Goals & Non‑Goals**

## 1.1 Goals

*   Enforce **strict architectural invariants** at compile-time.
*   Govern how domain models are **constructed**, **validated**, **composed**, and **propagated**.
*   Encode rules that runtime code **cannot** reliably enforce.
*   Provide deterministic, actionable diagnostics.
*   Maintain absolute **non-silence** in consumer projects.
*   Ensure **authority repositories** (Dx.Domain itself) remain clean, self-hosting, and discipline‑immune.
*   Integrate seamlessly with both **local development** and **CI pipelines**.

## 1.2 Non‑Goals

*   No runtime enforcement.
*   No implicit fallback modes.
*   No semantic expansion or business domain modeling.
*   No opining on application architecture outside the Dx.Domain contract.
*   No dependency on template implementation for functionality.

***

# **2. Scope & Authority Model**

Dx.Domain’s analyzers operate under a **strict authority hierarchy**:

| Scope            | Description                               | Analyzer Behavior                                                   |
| ---------------- | ----------------------------------------- | ------------------------------------------------------------------- |
| **S0 Authority** | Kernel, Primitives, Annotations           | DXA\* disabled (short‑circuited); DXT ignored; zero false positives |
| **S1/S2**        | Reserved for future layering              | Observational unless upgraded                                       |
| **S3 Consumer**  | Application, Host, Infrastructure, Shared | All DXA\*, DXK\*, DXT rules fully enforced                          |
| **Tests**        | Any project with `IsTestProject=true`     | Lightened enforcement; some DXA\* exempt                            |

### 2.1 Scope Determination

Analyzers resolve scope from:

1.  **MSBuild compiler‑visible properties**
    *   `DxLayer`
    *   `DxResolvedRole`
    *   `IsTestProject`

2.  `[assembly: DxLayer("…")]` (fallback)

3.  Assembly‑name heuristic (only for external adopters)

### 2.2 Authority Immunity (Non‑Negotiable)

In **Authority** projects:

*   DXA rules **must never run**.
*   DXT invariants must **not** be probed.
*   No DX\* diagnostic may appear without explicit opt‑in.
*   Foundation must compile with **zero suppressions**.

***

# **3. Diagnostic Catalog (Normative)**

The diagnostic catalog is **append‑only** and grouped into three families:

## 3.1 DXK\* — Role & Dependency Physics (Core Laws)

*   **DXK001 — Assembly Role Required**
*   **DXK002 — Role Dependency Matrix**
*   **DXK003 — Domain Purity**
*   **DXK004 — Primitive Obsession**
*   **DXK005 — Illegal Exception Flow (Consumer)**
*   **DXK006 — Outbox Fact Boundary**
*   **DXK007 — Contract Hygiene**
*   **DXK008 — Observability Invariant**
*   **DXK009 — Internal Dx.Domain Package Reference Forbidden**

## 3.2 DXA\* — Consumer Discipline Rules

*   **DXA010 — Construction Discipline**
*   **DXA011 — Public Factory Exposure**
*   **DXA020 — Result Ignored**
*   **DXA022 — Result vs Throw Discipline**
*   **DXA030 — Unapproved Handler**
*   **DXA040 — Kernel Public Surface Freeze (opt‑in)**
*   **DXA050 — Temporal Helper Misuse**
*   **DXA060 — Forbidden Vocabulary**
*   **DXA070 — Generated Code Tagging**
*   **DXA080 — Facade Missing Invariant Enforcement**
*   **DXA090 — Forbidden Analyzer Suppression (Non‑Silence Guard)**

## 3.3 DXT\* — Template Contract Rules

*   **DXT001 — Template Role Completeness**
*   **DXT002 — Template Required Reference Missing**
*   **DXT003 — Template Forbidden Reference Present**
*   **DXT004 — `.dx/invariants.json` Missing for Consumer Solutions**

***

# **4. Diagnostic Semantics**

Each rule must define:

*   **Intent**
*   **Applies To** (scope-specific precision)
*   **Never Applies To**
*   **Classification** (syntax / symbol / flow / CFG)
*   **Examples (Violation)**
*   **Examples (Valid)**
*   **Remediation**
*   **Optional Code‑fixes**
*   **Configuration Options** (if any)

### 4.1 Severity Model (Non‑Silence Axiom)

*   In governed consumer projects:  
    **All DX* diagnostics are Errors.*\*

*   Attempts to modify severities through:
    *   `.editorconfig`
    *   suppressions
    *   disabled analyzers  
        …must produce **DXA090** and fail the build.

*   Test projects may receive lowered severity according to charter rules, but **cannot fully disable** analyzers.

### 4.2 Exemption & Generated Code

Generated code is exempt via:

*   `[GeneratedCode]`
*   Custom markers defined in editorconfig (`dx_generated_markers`)
*   Namespace‑level exemptions for generated source generators

***

# **5. Infrastructure & Internal Architecture**

The analyzer suite is composed of the following components:

### 5.1 ScopeResolver (Critical)

*   Reads compiler-visible properties
*   Computes S0/S3/Test status
*   Ensures rules behave identically under deterministic conditions
*   Must never assume directory or solution structure

### 5.2 SemanticClassifier

Identifies:

*   Domain primitives
*   Domain errors
*   Result types
*   Invariant violations
*   Identity/value types
*   Kernel classes vs consumer classes

### 5.3 DxFacadeResolver

Discovers:

*   Approved construction boundaries
*   Static factories
*   Facade attributes
*   Configurable roots

### 5.4 ExceptionIntentClassifier

Categorizes exceptions into:

*   Argument Validation
*   Invariant Violation
*   Control Flow
*   Domain Control
*   Infrastructure/System

Used by DXA022.

### 5.5 ResultFlowEngine (CFG)

Tracks:

*   Result creation
*   Propagation
*   Termination
*   Ignoring (DXA020)

### 5.6 GeneratedCodeDetector

Provides configurable generated-code detection.

***

# **6. API & Configuration Surface**

### 6.1 Required `.editorconfig` Interface

```ini
[*.cs]
build_property.DxLayer = Consumer
build_property.DxResolvedRole = Domain
dx_generated_markers = Generated;__generated
```

### 6.2 MSBuild Contract

One ingress, one choke‑point:

*   `Directory.Build.props`
*   `Directory.Build.targets` (identity → governance → analyzer governance)

### 6.3 Template Contract Validation (DXT004)

Consumer solutions **must** contain:

    /.dx/invariants.json

Absent → immediate DX error.

***

# **7. Performance & Determinism Guarantees**

### 7.1 Deterministic Output

*   No randomness
*   No ambient environment reading
*   No use of host time
*   No use of file system except for DXT

### 7.2 Performance Target

*   < **5 ms per analyzed method** average
*   Hard CI thresholds
*   CI trend gates

***

# **8. Test Matrix (Mandatory for RC)**

### 8.1 Analyzer Unit Tests

*   Positive / negative cases
*   Generated code exemptions
*   Authority immunity tests
*   Test scope behavior
*   DXT presence/absence
*   Non-silence suppression attempts

### 8.2 Repo‑Shape Tests

Multi-project synthetic solutions validating:

*   Role declaration
*   Dependency geometry
*   Contract hygiene
*   Template contract correctness

### 8.3 Performance Regression Tests

Benchmark projects executed in CI:

*   Method‑heavy
*   Fact‑heavy
*   Result‑intensive
*   High CFG complexity

***

# **9. CI/CD Requirements**

### 9.1 Pull Request Stage

*   Analyzer compilation
*   Analyzer tests
*   Non‑Silence test
*   Perf smoke
*   DocFX validation

### 9.2 Main Branch

*   Extended perf
*   Missing DXT allowed (authority-only)
*   Verify “no DXA in authority”
*   Publish docs preview

### 9.3 Release Tag

*   Deterministic rebuild
*   Pack transitive analyzer assets
*   Sign packages
*   Generate SBOM
*   Publish documentation & rule catalog

***

# **10. Documentation Set (Normative)**

Required published documents:

*   **Analyzer Suite Index**
*   **Full Rule Charters (all DXK\*, DXA\*, DXT\*)**
*   **Layer Awareness Model**
*   **MSBuild Governance Contract**
*   **Enforcement Guarantees**
*   **Authority Admission Test**
*   **Analyzer Behavior Guarantees**

Each document must be cross-linked and versioned.

***

# **11. Backwards Compatibility & Evolution**

*   Diagnostic IDs are **append-only**.
*   Breaking rules require a major version bump.
*   Rule behavior tightening permitted only when:
    *   Non‑silence is preserved
    *   Authority remains immune
    *   DXT contract remains stable

***

# **12. RC Exit Criteria**

A release is accepted when the following are true:

1.  Authority layers build with **zero** DXA\* diagnostics and no DXT probing.
2.  Consumer misuse triggers expected diagnostics deterministically.
3.  All Must-Have rules implemented and test‑green.
4.  Code‑fixes present for DXK001, DXA020, DXA010, DXT004.
5.  Perf target met (<5ms/method).
6.  Documentation is complete and published.
7.  Analyzer package included transitively and non-packable standalone.
8.  Attempted suppression → DXA090 → CI failure.
9.  CHANGELOG updated and frozen.

***

# **13. Canonical Diagnostic Example**

**DXA020 — Result Ignored**

**Message:**

> This method returns a `Result` value that is not handled or returned. Silent discard is not permitted in consumer scope.

**How to Fix:**  
Assign the `Result` and check `IsFailure`, or return the value directly to propagate it.

**Scope:**  
Consumer (S3); authority and test scopes exempt.

**Category:** Control Flow / Result Lifecycle

***

# **14. Final Statement**

**Dx.Domain.Analyzers** is the **compiler of the architecture itself**.  
Where the Kernel defines semantics, the analyzers define **governance**—ensuring the architecture never decays, the domain language remains coherent, and misuse becomes mechanically impossible.

This Release Candidate specification formalizes the rules, scope model, governance, and testing required to freeze the analyzers for the first stable release of Dx.Domain.

***

# Dx.Domain.Analyzers Specification (world-class, publication-ready)

> Everything is **layer‑aware**, **non‑silent**, and **DX‑first**—fully aligned with the ADRs, charters, and governance in your docs. 

***

## 1) Tone & Editorial Standard (Analyzer Voice)

**Non‑negotiables**

*   **Non‑silence.** Consumers **cannot** disable or downgrade DX diagnostics. Analyzer governance must fail builds when suppression is attempted. (Authority layers are explicitly immune.) 
*   **Layer‑aware.** Rules are **Constraining** in consumer scope, **Observational** or **Exempt** in authority scope (Kernel/Primitives/Annotations). Scope is resolved via compiler‑visible MSBuild properties, not heuristics. 
*   **Explain, don’t moralize.** Diagnostics are **specific, mechanical, and actionable**—they name *what*, *where*, and *how to fix* with minimal prose. Charters define intent, applicability, examples, and remediation. 
*   **Determinism and zero surprises.** Identical inputs → identical diagnostics; generated code is exempt; infrastructure issues never “flake” into consumer builds. 
*   **Tight performance budget.** < **5 ms** per method analysis on average; regressions fail the CI perf gate. 

**Message style**

*   **Title**: short, noun‑phrase.
*   **Message**: “Because **X**, **Y** is not allowed here.”
*   **Fix**: a single, imperative suggestion (and optional code‑fix).
*   **Scope hint**: when a rule is skipped due to authority scope, it stays silent—no informational noise. 

***

## 2) Dx.Domain.Analyzers — Specification (Release Candidate)

### 2.1 Goals & Non‑Goals

**Goals**

*   Provide a **compile‑time enforcement layer** for Dx.Domain that makes misuse **impossible to ignore** and correct usage **obvious**.
*   Enforce **role/layer geometry**, **result discipline**, **construction discipline**, **observability invariants**, and **template contract (DXT)** in consumer code. 

**Non‑Goals**

*   No runtime behaviors, logging, or policy; analyzers are **pure** Roslyn analysis.
*   No dependency on templates for the framework to build; **authority repos** must compile **without** any `.dx/invariants.json`. Consumers must have it (DXT004). 

***

### 2.2 Scoping Model (Normative)

*   **Compiler‑visible properties**: `DxLayer`, `DxResolvedRole`, `IsTestProject` exposed via MSBuild → analyzers (no name heuristics).  
    *Authority precedence*: DxLayer value wins; attribute fallback then naming. 
*   **Authority (S0)** = Kernel, Primitives, Annotations → **skip** consumer DXA rules.
*   **Consumer (S3)** → **enforce** DXA/DXK/DXT rules; non‑silence axiom applies.
*   **Tests** → lightened discipline; some consumer rules are exempt by scope. 

***

### 2.3 Distribution & Packaging

*   **Target**: `netstandard2.0` analyzer assemblies; Roslyn `Microsoft.CodeAnalysis.CSharp 4.12.x`. 
*   **Shipping model**: analyzers are **transitive assets** of Kernel, Primitives, and Annotations packages (IncludeAssets=analyzers; PrivateAssets=all). Local repo falls back to `ProjectReference` with `OutputItemType=Analyzer`. 
*   **Analyzer package** (`Dx.Domain.Analyzers`) is **not packable** standalone by default; governance prevents accidental publish. 

***

### 2.4 Diagnostic Taxonomy (IDs & Families)

*   **DXK**\* — Core role/law rules (role presence, dependency geometry, purity, outbox boundary). 
*   **DXA**\* — Consumer discipline (construction, result handling, exception intent, vocabulary, generated code tagging, facade invariants). 
*   **DXT**\* — Template contract (presence/validity of `.dx/invariants.json`), **consumer‑only**. 
*   **DX100*/DX700*\*\* — Generator invariants & sandbox (documented here for completeness; live with Generators). 

**Stability contract**: IDs are **append‑only**; never repurpose an ID. Titles MAY evolve; categories/severities default are stable unless a major version bump documents a governance change. 

***

### 2.5 Severity & Governance (Non‑Silence)

*   **Consumer scope**: all DX diagnostics are **errors** by central policy; `.editorconfig` cannot downgrade or suppress them; attempts **fail the build**. 
*   **Authority scope**: DXA\* never trigger; DXK\* are observational/disabled per charter; DXT presence is **never** probed. 

***

### 2.6 Configuration Surface

**EditorConfig signals (read‑only in authority)**

```ini
# scope signals (project properties mirrored by the build)
build_property.DxLayer = Consumer
build_property.DxResolvedRole = Domain
# optional: freeze kernel API (observational)
build_property.DxKernelApiFreeze = true
# generated code markers
dx_generated_markers = Generated;__generated
# optional severity tuning (consumers may not downgrade in governed repos)
dotnet_diagnostic.DXA010.severity = error
```

 

**MSBuild (compiler‑visible)**

```xml
<PropertyGroup>
  <DxLayer>Consumer</DxLayer>
</PropertyGroup>
<ItemGroup>
  <CompilerVisibleProperty Include="DxLayer" />
  <CompilerVisibleProperty Include="DxResolvedRole" />
  <CompilerVisibleProperty Include="IsTestProject" />
</ItemGroup>
```

 

***

### 2.7 Performance & Reliability

*   Budget: **< 5 ms** per method on average; CI enforces trend thresholds.
*   Generated code exemption recognizes `[GeneratedCode]` and configured markers.
*   Deterministic output: identical syntax trees → identical diagnostics. 

***

### 2.8 No‑Telemetries & Privacy

*   No telemetry or external calls. Analyzers operate fully offline. (Reinforces determinism and supply‑chain predictability implied by governance.) 

***

### 2.9 Compatibility & Versioning

*   **SemVer** for the analyzer package; **append‑only** diagnostic catalog; rule behavior changes documented in CHANGELOG.
*   Analyzer distribution guarantees documented and enforced (transitive shipping, non‑packable analyzer project by default). 

***

## 3) Release‑Readiness Plan & Checklist (Analyzers)

**A. Scope & Authority hardening**

*   [ ] ScopeResolver uses **only** compiler‑visible properties; no heuristics. **Unit tests** cover S0 vs S3 behavior. 
*   [ ] Confirm **Authority immunity**: DXA\* never run for Kernel/Primitives/Annotations; no probing of DXT. (Negative tests included.) 

**B. Rule set for v1.0**

*   **Must‑have** (blocking): `DXA010`, `DXA020`, `DXA022`, `DXA060`, `DXT004`, `DXK001`, `DXK002`, `DXK003`, `DXK006`, `DXK007`, `DXK008`. 
*   **Nice‑to‑have**: `DXA011`, `DXA030`, `DXA040` (opt‑in via property), `DXA050`, `DXA070`, `DXA080`, and **suppression detection** `DXA090` (from ADR‑0007). 

**C. Code‑fix providers**

*   [ ] Provide at least one safe fix for each **Must‑have** rule (where actionable):
    *   DXA020 → wrap/propagate Result or assign/return it.
    *   DXA010/011 → suggest moving construction to approved facade.
    *   DXK001 → insert `[assembly: DxAssemblyRole(...)]`.
    *   DXT004 → add `.dx/invariants.json` skeleton (template). 

**D. Tests**

*   [ ] **Analyzer unit tests** per rule (Roslyn Testing): positive/negative, generated‑code exemption, scope switching.
*   [ ] **Repo‑shape tests**: synthetic solutions validating DXK001/002 graph and DXT004 presence.
*   [ ] **Performance benches**: method‑dense projects; fail if > 5 ms/method average. 

**E. CI/CD**

*   [ ] PR pipeline: build analyzers, run analyzer tests, run perf smoke, verify **non‑silence** (attempt to disable → CI fails). 
*   [ ] Release pipeline: pack analyzer assets into Kernel/Primitives/Annotations nupkgs; verify SourceLink, symbol packages, SBOM; publish docs (DocFX). 

**F. Documentation**

*   [ ] **Charters** complete for each rule (Intent • Applies to • Never applies • Classification • Examples • Remediation).
*   [ ] **Layer‑awareness** page and **MSBuild contract** published.
*   [ ] **Enforcement guarantees** and **Admission test** docs finalized. 

**G. Packaging**

*   [ ] Analyzer project targets `netstandard2.0`; no runtime deps beyond Roslyn.
*   [ ] Analyzer referenced **transitively** by Kernel/Primitives/Annotations with `IncludeAssets="analyzers"`.
*   [ ] Analyzer project **non‑packable** by default (guard rule DXB004). 

***

## 4) Rule Catalog Matrix (v1.0 defaults)

> **Scope**: S0 = Authority (exempt or observational), S3 = Consumer (enforced)  
> **Severity default**: errors in governed consumer builds (non‑silence); informational/disabled in authority as chartered. 

| ID     | Title (short)                         | Scope | Default | Code‑Fix | Summary                                                                                                                                                                                                                                                                        |
| ------ | ------------------------------------- | ----- | ------- | -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| DXK001 | Assembly Role Required                | S3    | Error   | Yes      | Require `[assembly: DxAssemblyRole(...)]` in consumer projects. 
| DXK002 | Role Dependency Matrix                | S3    | Error   | No       | Enforce geometry (e.g., Domain ↛ Infrastructure, Contracts ↛ Kernel). 
| DXK003 | Domain Purity                         | S3    | Error   | No       | Block forbidden namespaces in Domain role (e.g., EF/HTTP). 
| DXK006 | Outbox Fact Boundary                  | S3    | Error   | No       | Outbox/message boundaries accept only `IDomainFact`. 
| DXK007 | Contract Hygiene                      | S3    | Error   | No       | Contracts must not reference Kernel. 
| DXK008 | Observability Invariant               | S3    | Error   | No       | Host must propagate `CorrelationId`. 
| DXA010 | Construction Discipline               | S3    | Error   | Yes      | Centralize construction behind approved facades. 
| DXA011 | Public Factory Exposure               | S3    | Error   | Yes      | No public constructors/factories on domain types (use facades). 
| DXA020 | Result Ignored                        | S3    | Error   | Yes      | Results must be handled, returned, or explicitly flowed. 
| DXA022 | Result vs Throw Discipline            | S3    | Error   | Suggest  | Public Result methods must not throw domain outcomes (guards/invariants allowed). 
| DXA030 | Unapproved Handler                    | S3    | Error   | No       | `Result` flows only through approved handlers/adapters. 
| DXA040 | Kernel Public Surface Freeze (opt‑in) | S0    | Warn    | No       | Observational safety net when `DxKernelApiFreeze=true`. 
| DXA050 | Temporal Helper Usage                 | S3    | Warn    | No       | Move policy/time helpers to adapters. 
| DXA060 | Forbidden Vocabulary                  | S3    | Error   | No       | Ban pattern‑museum vocabulary in consumer code. 
| DXA070 | Generated Code Tagging                | S3    | Error   | Yes      | Require `[GeneratedCode]` or configured markers. 
| DXA080 | Facade Invariant Enforcement Missing  | S3    | Error   | Suggest  | Facade factories must enforce invariants (often via `Result`). 
| DXA090 | Suppression Detection                 | S3    | Error   | No       | Flag attempts to mute DX rules (per ADR‑0007). 
| DXT004 | DXT Invariants Required               | S3    | Error   | Yes      | Require `.dx/invariants.json` at solution root for consumers. 

> **Note**: DX1001..DX1004 & DX7001..DX7002 live with Generators but are listed in docs for completeness. 

***

## 5) Example Diagnostic (canonical structure)

    Id:       DXA020
    Title:    Result Ignored
    Message:  Because this method returns a Result, the produced Result must be handled or returned; silent discard is not allowed in consumer scope.
    HowToFix: Assign the Result to a variable and check IsFailure, or return/propagate it.
    Scope:    Consumer (S3) only; generated code exempt.

(Non‑silence escalation to **error** in governed repos.) 

***

## 6) Reference Index (where this spec draws from)

*   **Analyzer charters & index** (DXK/DXA/DXT/Generator families) and **layer awareness** docs. 
*   **Non‑silence axiom**, **DXT contract**, **authority immunity**, and **MSBuild governance**. 
*   **ADR‑0001/2/3/4/5/7** (substrate, non‑silence, scope/authority, construction/result discipline, facts split & analyzer hardening). 
*   **Analyzer README** (targets, perf budget, infra components). 
