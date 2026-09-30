# Roadmap

**Last reviewed:** 2026-04-23
**Current version:** 0.1.0-alpha

## Alpha status

Dx.Domain `0.1.0-alpha` is the current shipped prerelease. Public APIs, package composition, analyzer behavior, configuration, and non-error diagnostic severities remain provisional until `0.1.0` stable.

## Philosophy

Roadmap items are classified by their effect on guarantees, not by feature completeness.

- **Committed:** Will be in 0.1.0 stable. Changes core guarantees.
- **Experimental:** May be removed. Explores enforcement mechanisms.
- **Deferred:** Valuable but violates Non-Goals if placed in core.

## Committed for 0.1.0 stable

### 1. Finalize primitive public surfaces

**Existing contract:** decisions/adr-0018-kernel-public-surface.md establishes the S0 public construction contract and exempts the substrate types from DXA010, DXA011, and DXA080.

**Problem:** The complete API signatures and default-value behavior of the identity primitives remain provisional for the alpha.

**Goal:** Finalize and baseline the public surfaces of `CorrelationId`, `TraceId`, `UserId`, `FactId`, and `SpanId`.

**Trade-off:** Finalizing the signatures constrains future representation and API changes.

### 2. `Result<T>` API finalization

**Problem:** Match, Map, Bind, default-value behavior, and related Result signatures remain provisional.

**Goal:** Define and baseline the stable API and behavior for success and failure handling.

**Trade-off:** Verbosity is accepted where it preserves explicit failure semantics.

### 3. DXA010 default severity to Error

**Current state:** The DXA010 descriptor defaults to Warning for consumers. Repository maintainer builds already promote applicable DXA diagnostics, including DXA010, to errors through maintainer configuration and build governance.

**Goal:** Change the stable-release descriptor-default severity from Warning to Error so direct construction outside the authorized substrate or construction boundary fails consumer builds by default.

**Trade-off:** Existing experiments that rely on direct construction will require an approved construction boundary.

## Experimental

### Analyzer for unchecked `Result.Value`

**Problem:** Callers can access `Result.Value` without proving that the Result succeeded.

**Exploration:** Add a Roslyn analyzer that detects unchecked access patterns not covered by DXA020.

**Risk:** Flow analysis may produce false positives in legitimate scenarios.

### Test clock for `DomainTime`

**Problem:** `DomainTime.Now()` reads the live UTC system clock and is nondeterministic in tests.

**Exploration:** Introduce an internal test-time mechanism without exposing clock policy through the public Kernel API.

**Risk:** The mechanism could leak into the public surface or introduce ambient state if not tightly bounded.

### Generator for ID factories

**Problem:** Repeating identity-factory implementations is mechanical but verbose.

**Exploration:** Implement a source generator that emits auditable creation code while preserving the existing identity semantics.

**Risk:** Generated code must remain visible, deterministic, and consistent with analyzer and package contracts. `Dx.Domain.Generators` is not shipped in `0.1.0-alpha`.

## Deferred intentionally

- **EF Core integration:** Conflicts with non-goals.md#3-a-persistence-framework. Persistence support belongs in an adapter package, not the core.
- **JSON converters in core:** Conflicts with non-goals.md#1-a-general-purpose-utility-library. Serialization support belongs in a separate package.
- **Localization of errors:** Application-level localization must not alter the Kernel’s stable error identities and explicit failure semantics.

## What will not change

These principles are fixed by the manifesto.md and supporting architectural decisions:

- Strongly typed identities will remain.
- `Result` will remain the representation for expected domain failure.
- Construction authority will remain.
- UTC-only domain time will remain.

The mechanisms, signatures, package composition, and analyzer implementation may evolve until stable release. The principles do not.
