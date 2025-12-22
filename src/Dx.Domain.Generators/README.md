# Dx.Domain.Generators

## Overview

The Dx.Domain.Generators package provides a formally specified, deterministic code generation framework that guarantees referential transparency, monotonic knowledge, and supply chain integrity. This implementation strictly adheres to the specification defined in [dx.domain.generators.md](../../docs/internal/dx.domain.generators.md).

## Key Features

### Generator Invariants (Hard Guarantees)

1. **Referential Transparency (DX1001)**
   - Deterministic generation: identical inputs produce byte-identical outputs
   - SHA256-based InputFingerprint calculation
   - Canonicalization of all inputs to remove machine-specific data
   - Forbidden: timestamps, machine paths, random GUIDs, undeclared environment variables

2. **Monotonic Knowledge (DX1002)**
   - Pipeline stages can add facts but never contradict prior stages
   - StageAssertionSet validation across the entire pipeline
   - Automatic detection of incompatible assertions

3. **No Semantic Guessing (DX1003)**
   - Explicit failure on ambiguous intent
   - ResolutionRequest generation for developer guidance
   - Policy-driven resolution, never silent defaults

4. **No Hidden Coupling (DX1004)**
   - All dependencies must be explicitly declared
   - Capability declaration for each stage
   - Pipeline orchestrator enforces declarations

### Failure Taxonomy

All failures are classified into one of the following categories:

- **DX1xxx**: Generator Invariant Violations
- **DX2xxx**: Intent Violations
- **DX3xxx**: Policy Violations
- **DX4xxx**: Inference Failures
- **DX5xxx**: Compatibility Failures
- **DX6xxx**: System Failures
- **DX7xxx**: Cache Violations
- **DX8xxx**: Trust Violations

### Incrementality & Caching

- Deterministic cache key generation: `SHA256(InputFingerprint || stageName || stageVersion || policyVersions)`
- Stage metadata declarations: name, inputKeys, outputKeys, version, cacheable
- Automatic detection of:
  - DX7001: Undeclared external inputs
  - DX7002: Non-deterministic cacheable stages

### Trust Boundaries

Three trust zones with specific rules:

1. **Authoritative**: Signed generators, policies, CI keys (must be signed)
2. **Declarative**: Source intent, manifests (must be hashed into InputFingerprint)
3. **Derived**: Generated artifacts (must not be used as inputs)

Provenance tokens track:
- InputFingerprint
- Generator version
- Policy versions
- Artifact hashes
- CI signer identity

### Automation Governance

Remediation levels control fix application:

- **Safe**: No behavior change (auto-apply in CI)
- **Behavioral**: Reversible behavior change (suggest in IDE)
- **Breaking**: Public API or semantic change (require manual approval)

AutoFixLevel policy:
- `None`: Report only
- `Suggest`: Show fixes but don't apply
- `Safe`: Auto-apply safe fixes only
- `Apply`: Auto-apply up to configured impact level

## Core Components

### InputFingerprint

```csharp
var fingerprint = InputFingerprint.Compute(
    canonicalizedIntent,
    canonicalizedManifest,
    canonicalizedPolicies,
    generatorVersion);
```

### Canonicalization

```csharp
// Canonicalize JSON, removing non-deterministic properties
var canonical = Canonicalization.CanonicalizeJson(jsonInput);

// Canonicalize paths, removing machine-specific prefixes
var canonical = Canonicalization.CanonicalizePath(filePath);

// Remove timestamps and GUIDs
var cleaned = Canonicalization.RemoveTimestamps(input);
var cleaned = Canonicalization.RemoveGuids(input);
```

### StageDeclaration

```csharp
var stage = new StageDeclaration(
    stageName: "TransformEntities",
    inputKeys: new[] { "schema", "policies" },
    outputKeys: new[] { "entities" },
    stageVersion: "1.0.0",
    cacheable: true,
    declaredDependencies: new[] { "SchemaParser" },
    capabilities: new[] { "EntityGeneration" });

var cacheKey = stage.ComputeCacheKey(fingerprint, policyVersions);
```

### StageAssertionSet

```csharp
var assertions = new StageAssertionSet("stage1", new Dictionary<string, object>
{
    { "schema", "v1" },
    { "entityCount", 10 }
});

// Validate compatibility with prior stage
var isCompatible = currentStage.IsCompatibleWith(priorStage, out var contradictions);
```

### GeneratorDiagnostic

```csharp
var diagnostic = new GeneratorDiagnostic(
    id: DiagnosticCodes.ReferentialTransparency,
    @class: FailureClass.IntentViolation,
    title: "Referential Transparency Violation",
    message: "Non-deterministic input detected: DateTime.Now",
    inputFingerprint: fingerprint,
    stage: "ParseIntent",
    location: new DiagnosticLocation("file.cs", 42, 10),
    remediation: new[] { "Replace DateTime.Now with a fingerprint-derived value" },
    fixPreview: "Use InputFingerprint for deterministic values",
    impact: ImpactLevel.Safe);
```

### AutomationPolicy

```csharp
// Create strict CI policy
var ciPolicy = AutomationPolicy.DefaultCiPolicy;

// Check if fix can be auto-applied
if (ciPolicy.CanAutoApply(ImpactLevel.Safe, isCiContext: true))
{
    // Apply fix automatically
}
```

### ProvenanceToken

```csharp
var token = new ProvenanceToken(
    inputFingerprint: fingerprint,
    generatorVersion: "1.0.0",
    policyVersions: new Dictionary<string, string> { { "Policy1", "1.0" } },
    artifactHashes: new Dictionary<string, string> { { "Output.cs", "abc123..." } },
    ciSigner: "CI/CD System",
    signature: "signature_data",
    timestamp: DateTimeOffset.UtcNow);

var isValid = token.VerifySignature(publicKey);
```

## Analyzer Rules

The package includes Roslyn analyzers to enforce invariants at compile time:

- **DX1001**: Detects non-deterministic inputs (DateTime.Now, Guid.NewGuid, etc.)
- **DX1002**: Validates monotonic knowledge in pipeline stages
- **DX1003**: Enforces explicit resolution of ambiguous intent
- **DX1004**: Detects undeclared stage dependencies
- **DX7001**: Detects undeclared external inputs in cacheable stages
- **DX7002**: Detects non-deterministic operations in cacheable stages

## Testing

The package includes a comprehensive test suite with 35+ unit tests covering:

- InputFingerprint computation and equality
- Canonicalization utilities for deterministic fingerprinting
- StageAssertionSet compatibility validation
- StageDeclaration and cache key generation
- AutomationPolicy rules and enforcement

Run tests:
```bash
dotnet test tests/Dx.Domain.Generators.Tests
```

## Compliance

This implementation guarantees:

✅ Deterministic generation  
✅ Explainable failures  
✅ Safe extensibility  
✅ Auditable releases  
✅ Supply-chain integrity  
✅ Zero-boilerplate developer experience  

For the complete formal specification, see [dx.domain.generators.md](../../docs/internal/dx.domain.generators.md).

## License

Copyright © 2025 Dx.Domain Team. All rights reserved.

This software is licensed under the MIT License.
