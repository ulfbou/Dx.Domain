# Security Policy

**Last reviewed:** 2026-04-22  
**Applies to:** All Dx.Domain packages

## Supported versions

Only the latest alpha is supported for security updates. Given the pre-release status, security fixes are best-effort.

| Version | Supported |
| --- | --- |
| 0.1.0-alpha.1 | Yes |
| < 0.1.0-alpha.1 | No |

## Reporting vulnerabilities

Do not open public issues for security vulnerabilities.

Email: security@dxdomain.dev (placeholder — replace with actual contact before public release)

Include:
- Description of vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix if known

We will acknowledge within 48 hours and provide timeline for fix.

## Security considerations for alpha

Dx.Domain is pre-release software. Do not use in production systems handling sensitive data.

Specific risks in alpha:
- **Construction bypass:** Reflection can bypass private constructors and create invalid objects
- **Invariant bypass:** Direct struct initialization via `default` bypasses invariant checks
- **Analyzer bypass:** DXA010 is warning only, can be suppressed or ignored
- **Serialization risks:** No built-in protection against deserialization attacks

These are known limitations, not vulnerabilities. They are documented in [Enforcement Map](learn/enforcement-map.md).

## Design for security

Security in Dx.Domain is achieved through:
- **Explicit construction:** Private constructors prevent arbitrary instantiation
- **Invariant enforcement:** Invalid state fails fast
- **No hidden state:** All data is explicit in types
- **No reflection in core:** Kernel does not use reflection for core logic

## Dependencies

Dx.Domain has zero runtime dependencies outside .NET base class library. This minimizes supply chain risk.

Analyzers depend on Roslyn, which is part of .NET SDK.

## Best practices for consumers

1. Treat DXA010 warnings as errors in your build
2. Do not use reflection to bypass construction authority
3. Validate all external input before creating domain objects
4. Do not persist primitive formats (they will change)
5. Review ADRs for security-relevant decisions

