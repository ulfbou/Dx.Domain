# CI/CD Pipeline System

> **DX-first, future-proof pipelines that are reliable, auditable, and fast**

This repository implements a sophisticated, modular CI/CD pipeline system designed for excellent developer experience (DX), reproducible builds, and continuous protection against drift.

## Table of Contents

- [Overview](#overview)
- [Pipeline Architecture](#pipeline-architecture)
- [Workflows](#workflows)
- [Getting Started](#getting-started)
- [Versioning Strategy](#versioning-strategy)
- [Release Process](#release-process)
- [Troubleshooting](#troubleshooting)

## Overview

The CI/CD system consists of five primary pipelines:

1. **Pre-merge CI** - Fast feedback on PRs (< 10 minutes)
2. **Integration CI** - Full builds with reproducible flags on main branch
3. **Release CD** - Staged promotion through staging → canary → production
4. **DocFX Pipeline** - Automated documentation generation and publishing
5. **Drift Protection** - Continuous infrastructure and configuration integrity checks

### Key Features

- ✅ **Fast Feedback**: PR checks complete in < 10 minutes for most changes
- ✅ **Reproducible Builds**: Deterministic builds with embedded commit metadata
- ✅ **Semantic Versioning**: Automated version management with GitVersion
- ✅ **Documentation as Code**: Auto-generated API docs with versioned publishing
- ✅ **Security First**: Dependency scanning, SBOM generation, package signing
- ✅ **Drift Protection**: Automated detection and remediation of configuration drift
- ✅ **Staged Deployments**: Safe promotion through staging, canary, and production
- ✅ **Automated Changelogs**: Generated from conventional commits

## Pipeline Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Developer Workflow                       │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Pre-merge CI (PR)                       │
│  • Lint & Format      • Fast Unit Tests                     │
│  • Security Scan      • DocFX Preview                        │
│  Goal: < 10 min       Status: ✅ PR checks                   │
└─────────────────────────────────────────────────────────────┘
                              │
                        [Merge to main]
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Integration CI (main)                     │
│  • Full Build         • All Tests (unit + integration)       │
│  • Reproducible       • Package Creation                     │
│  • SBOM Generation    • GitHub Packages (preview)            │
│  Artifacts: *.nupkg with commit SHA metadata                │
└─────────────────────────────────────────────────────────────┘
                              │
                    [Manual Release Trigger]
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Release CD                              │
│  Staging → Canary → Production                              │
│  • Signed Packages    • Changelog Generation                │
│  • GitHub Release     • NuGet.org Publishing                │
│  • Versioned Docs     • SBOM Artifacts                      │
└─────────────────────────────────────────────────────────────┘
                              │
                    [Continuous Monitoring]
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Drift Protection                          │
│  • Scheduled Scans    • Pre-deploy Checks                   │
│  • Issue Creation     • Auto-remediation PRs                │
│  • Critical Blocking  • Reporting                           │
└─────────────────────────────────────────────────────────────┘
```

## Workflows

### 1. Pre-merge CI (`pre-merge-ci.yml`)

**Trigger**: Pull requests to `main` or `develop`  
**Duration**: < 10 minutes (target)  
**Purpose**: Fast feedback for developers

**Jobs**:
- `lint-and-format`: Code formatting and analyzer checks
- `fast-tests`: Unit tests across .NET 8.0 and 9.0
- `security-scan`: Vulnerability scanning
- `docfx-preview`: Documentation preview generation

**Artifacts**:
- Test results
- Coverage reports
- DocFX preview site

### 2. Integration CI (`integration-ci.yml`)

**Trigger**: Push to `main` or `develop` branches  
**Duration**: 15-20 minutes  
**Purpose**: Full validation and artifact creation

**Jobs**:
- `build`: Reproducible build with version metadata
- `integration-tests`: Full test suite including integration tests
- `publish-preview`: Publish to GitHub Packages (develop only)

**Artifacts**:
- NuGet packages (`.nupkg`)
- Symbol packages (`.snupkg`)
- SBOM (Software Bill of Materials)
- Test results and coverage reports

**Build Metadata**:
```
Version: {semver}
NuGetVersion: {semver}+{commit-sha}
InformationalVersion: {semver}+{commit-sha}.{build-id}
RepositoryCommit: {full-commit-sha}
BuildId: {run-id}-{run-attempt}
```

### 3. Release CD (`release-cd.yml`)

**Trigger**: Manual workflow dispatch  
**Duration**: 30-45 minutes (depending on stages)  
**Purpose**: Staged production deployment

**Inputs**:
- `release-type`: patch | minor | major
- `environment`: staging | canary | production
- `skip-tests`: boolean (not recommended)

**Stages**:
1. **Validate**: Check branch and version
2. **Build**: Create release packages with signing
3. **Deploy Staging**: Deploy to staging environment
4. **Deploy Canary**: Deploy to canary (10% traffic)
5. **Deploy Production**: Full production deployment
6. **Create Release**: GitHub release with changelog

**Artifacts**:
- Signed packages with checksums
- Changelog
- Release notes

### 4. DocFX Documentation (`docfx.yml`)

**Trigger**: Changes to docs or source code  
**Duration**: 10-15 minutes  
**Purpose**: Automated documentation generation

**Jobs**:
- `build-docs`: Generate DocFX site with version metadata
- `publish-docs`: Publish to GitHub Pages
- `comment-pr`: Add preview link to PRs

**Features**:
- Versioned documentation (one snapshot per release)
- Link checking
- API reference generation from code
- PR preview comments

**Published Structure**:
```
https://your-site.github.io/Notes/
├── latest/           # Always points to newest docs
├── versions/
│   ├── 1.0.0/
│   ├── 1.1.0/
│   └── 2.0.0/
└── index.html        # Redirects to latest
```

### 5. Drift Protection (`drift-protection.yml`)

**Trigger**: 
- Daily schedule (3 AM UTC)
- PRs affecting infrastructure
- Manual dispatch

**Duration**: 5-10 minutes  
**Purpose**: Detect and remediate configuration drift

**Checks**:
- Workflow configuration drift
- Outdated dependencies
- Vulnerable dependencies
- Documentation drift

**Actions**:
- Create GitHub issues for drift detection
- Block deployments on critical drift
- Generate drift reports

**Severity Levels**:
- **Critical**: Vulnerable dependencies (blocks deployment)
- **High**: Significant drift (> 10 issues)
- **Medium**: Moderate drift (5-10 issues)
- **Low**: Minor drift (< 5 issues)

## Getting Started

### Prerequisites

1. .NET SDK 8.0+ and 9.0+
2. DocFX tool: `dotnet tool install -g docfx`
3. GitVersion (installed automatically in workflows)

### Local Development

**Build the solution**:
```bash
cd Dx.Domain.2
dotnet restore
dotnet build --configuration Release
```

**Run tests**:
```bash
# Fast unit tests only
dotnet test --filter "Category!=Integration&Category!=Slow"

# All tests
dotnet test
```

**Generate documentation**:
```bash
cd Dx.Domain.2
docfx docfx.json
# Serve locally
docfx serve _site
```

**Check for drift**:
```bash
# Check outdated packages
dotnet list package --outdated

# Check vulnerable packages
dotnet list package --vulnerable
```

### Creating a Pull Request

1. Create a feature branch: `git checkout -b feature/my-feature`
2. Make your changes
3. Commit using conventional commits: `git commit -m "feat: add new feature"`
4. Push and create PR
5. Wait for pre-merge CI to complete (< 10 min)
6. Address any failures
7. Review DocFX preview if documentation changed
8. Request review

## Versioning Strategy

This project uses **Semantic Versioning 2.0** with GitVersion for automated version management.

### Version Format

```
{major}.{minor}.{patch}[-{pre-release}]+{build-metadata}
```

**Examples**:
- `1.0.0` - Production release
- `1.1.0-alpha.1` - Alpha preview
- `1.2.0-rc.1` - Release candidate
- `1.2.0+abc1234` - Build with commit metadata

### Branch Versioning

| Branch Pattern | Version Tag | Increment | Example |
|---------------|-------------|-----------|---------|
| `main` | (none) | Patch | `1.0.1` |
| `develop` | `alpha` | Minor | `1.1.0-alpha.1` |
| `release/*` | `rc` | None | `2.0.0-rc.1` |
| `feature/*` | `alpha` | Minor | `1.1.0-alpha.2` |
| `hotfix/*` | `beta` | Patch | `1.0.2-beta.1` |

### Commit Message Versioning

Control version increments with commit messages:

```bash
# Patch bump
git commit -m "fix: resolve bug +semver:patch"

# Minor bump
git commit -m "feat: add feature +semver:minor"

# Major bump
git commit -m "feat!: breaking change +semver:major"

# No bump
git commit -m "docs: update readme +semver:none"
```

## Release Process

### Standard Release

1. **Prepare**: Ensure all PRs are merged to `main`
2. **Trigger**: Go to Actions → Release CD → Run workflow
3. **Configure**:
   - Release type: `patch`, `minor`, or `major`
   - Environment: Start with `staging`
4. **Validate**: Review staging deployment
5. **Promote**: Re-run for `canary` (10% traffic)
6. **Monitor**: Watch canary metrics
7. **Production**: Re-run for `production`
8. **Verify**: Check GitHub release and NuGet.org

### Hotfix Release

1. Create hotfix branch: `git checkout -b hotfix/fix-critical-bug main`
2. Fix the issue
3. Create PR to `main`
4. After merge, follow standard release process
5. Version will be automatically bumped (e.g., `1.0.1-beta.1`)

### Rollback

If issues are detected:

1. **Immediate**: Revert traffic to previous stable version
2. **GitHub**: Download previous release artifacts
3. **NuGet**: Previous versions remain available
4. **Docs**: Previous version docs remain at `/versions/{version}/`

## Troubleshooting

### Pre-merge CI Failures

**Lint/Format Failures**:
```bash
# Fix formatting locally
dotnet format

# Verify
dotnet format --verify-no-changes
```

**Test Failures**:
```bash
# Run specific test
dotnet test --filter "FullyQualifiedName~MyTest"

# Run with verbose output
dotnet test --logger "console;verbosity=detailed"
```

**Security Scan Failures**:
```bash
# Check for vulnerabilities
dotnet list package --vulnerable --include-transitive

# Update vulnerable packages
dotnet add package <PackageName> --version <SafeVersion>
```

### Integration CI Failures

**Build Failures**:
- Check for syntax errors
- Verify all dependencies are restored
- Check for version conflicts

**Integration Test Failures**:
- Verify test environment configuration
- Check for external service dependencies
- Review test logs in artifacts

### Release CD Failures

**Validation Failures**:
- Ensure you're on `main` branch
- Check version is valid semver
- Verify no uncommitted changes

**Deployment Failures**:
- Check environment secrets are configured
- Verify deployment target is accessible
- Review deployment logs

**Signing Failures**:
- Verify certificate is configured (if using)
- Check certificate is not expired
- Ensure correct certificate password

### Drift Protection

**High Drift Detected**:
1. Review drift report artifact
2. Address critical issues first (vulnerabilities)
3. Create remediation PR
4. Close drift issue after resolution

**False Positives**:
- Adjust thresholds in workflow
- Add exceptions for intentional drift
- Update drift detection logic

## Configuration

### Secrets Required

Configure these secrets in GitHub Settings → Secrets:

| Secret | Required | Purpose |
|--------|----------|---------|
| `NUGET_API_KEY` | Yes | Publishing to NuGet.org |
| `SIGNING_CERTIFICATE` | No | Package signing (base64) |
| `CERTIFICATE_PASSWORD` | No | Certificate password |

### Environment Variables

Key environment variables used across workflows:

- `DOTNET_SKIP_FIRST_TIME_EXPERIENCE`: Skip .NET welcome message
- `DOTNET_NOLOGO`: Hide .NET logo
- `DOTNET_CLI_TELEMETRY_OPTOUT`: Disable telemetry

## Pipeline Metrics

Monitor these metrics for pipeline health:

- **PR Check Duration**: Target < 10 minutes
- **Integration CI Duration**: Target < 20 minutes
- **Release CD Duration**: Target < 45 minutes
- **Test Success Rate**: Target > 95%
- **Drift Detection Frequency**: Daily
- **Critical Drift TTR**: Target < 24 hours

## Best Practices

1. **Keep PRs Small**: Enables faster CI and easier reviews
2. **Use Conventional Commits**: Enables automated changelog and versioning
3. **Test Locally First**: Run tests before pushing
4. **Monitor Drift Reports**: Address drift proactively
5. **Review Documentation**: Ensure docs stay in sync with code
6. **Use Feature Flags**: Enable safe canary deployments
7. **Regular Dependency Updates**: Minimize security vulnerabilities

## Support

For issues or questions:

1. Check this documentation
2. Review workflow logs in GitHub Actions
3. Check drift reports for configuration issues
4. Create an issue in the repository

## Contributing

See [CONTRIBUTING.md](../CONTRIBUTING.md) for details on contributing to this project and its CI/CD system.

---

**Last Updated**: 2024-12-18  
**Version**: 1.0.0  
**Maintained by**: DevOps Team
