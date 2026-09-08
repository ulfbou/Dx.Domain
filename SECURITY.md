# Security Policy

## Supported versions

Dx.Domain is pre-release software. Only the latest published alpha receives best-effort security fixes.

## Report a vulnerability

Use the repository's **GitHub Security Advisories** reporting form:

<https://github.com/ulfbou/Dx.Domain/security/advisories/new>

Do not report security vulnerabilities in a public issue. Include the affected package and version, impact, reproduction steps, and any suggested mitigation. No response-time or remediation-time guarantee is made during alpha.

## Alpha considerations

- Do not assume analyzers provide runtime protection.
- Reflection, serialization, dynamic invocation, and `default` struct values can bypass intended construction paths.
- Validate external input before creating domain values.
- Review the public [limitations](docs/public/limitations.md) before production use.
