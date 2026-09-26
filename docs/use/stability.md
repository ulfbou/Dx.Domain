# Alpha Stability

**Applies to:** `0.1.0-alpha`

## Classification

- **Principle-stable:** explicit failures, typed identities, constrained construction, small kernel, and edge-oriented integrations.
- **API provisional:** public signatures, package composition, and extension methods may change before stable release.
- **Analyzer provisional:** detection scope, false-positive handling, configuration keys, messages, and non-error severities may change.
- **Experimental:** source-generation and suppression governance work not listed as shipped diagnostics.
- **Deferred:** framework adapters, persistence integration, localization, and convenience APIs.

## Compatibility promise

The alpha does not provide semantic-versioning compatibility guarantees. A later alpha may require source changes. Release notes will identify known changes, but exhaustive migration guidance is not guaranteed before a stable release.

## Production use

Evaluate the package in non-critical workloads first. Consumers are responsible for tests, input validation, serialization behavior, runtime monitoring, and upgrade review.
