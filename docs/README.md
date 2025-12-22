# GitHub Configuration

This directory contains all GitHub-specific configuration including workflows, templates, and documentation.

## Directory Structure

```
.github/
├── workflows/              # GitHub Actions workflows
│   ├── pre-merge-ci.yml           # Fast PR checks
│   ├── integration-ci.yml         # Full builds on main
│   ├── release-cd.yml             # Staged release deployments
│   ├── docfx.yml                  # Documentation generation
│   ├── drift-protection.yml       # Configuration drift detection
│   ├── security-scan.yml          # Security scanning
│   ├── sign-packages.yml          # Package signing (reusable)
│   └── workflow-validation.yml    # Workflow validation
├── actions/                # Custom GitHub Actions (future)
├── ISSUE_TEMPLATE/        # Issue templates
│   ├── bug_report.md
│   ├── feature_request.md
│   └── cicd_issue.md
├── pull_request_template.md  # PR template
├── GitVersion.yml         # Semantic versioning config
├── CONVENTIONAL_COMMITS.md  # Commit message guide
├── CI-CD-PIPELINE.md      # Full pipeline documentation
└── CI-CD-QUICK-REFERENCE.md  # Quick reference guide
```

## Workflows

### Pre-merge CI (`pre-merge-ci.yml`)
**Purpose**: Fast feedback on pull requests  
**Trigger**: PR open/sync to main or develop  
**Duration**: < 10 minutes (target)  
**Jobs**: Lint, fast tests, security scan, DocFX preview

### Integration CI (`integration-ci.yml`)
**Purpose**: Full validation and package creation  
**Trigger**: Push to main or develop  
**Duration**: 15-20 minutes  
**Jobs**: Build, all tests, package creation, SBOM generation

### Release CD (`release-cd.yml`)
**Purpose**: Staged production deployment  
**Trigger**: Manual workflow dispatch  
**Duration**: 30-45 minutes  
**Jobs**: Validate, build, staging → canary → production

### DocFX Documentation (`docfx.yml`)
**Purpose**: Automated documentation  
**Trigger**: Doc/API changes  
**Duration**: 10-15 minutes  
**Jobs**: Build docs, publish to GitHub Pages, PR comments

### Drift Protection (`drift-protection.yml`)
**Purpose**: Configuration integrity  
**Trigger**: Daily schedule + PR with infra changes  
**Duration**: 5-10 minutes  
**Jobs**: Detect drift, create issues, block critical drift

### Security Scanning (`security-scan.yml`)
**Purpose**: Comprehensive security analysis  
**Trigger**: PR, push to main, weekly schedule  
**Duration**: 20-30 minutes  
**Jobs**: CodeQL, dependency review, secret scan, package scan

### Workflow Validation (`workflow-validation.yml`)
**Purpose**: Validate workflow files  
**Trigger**: Changes to workflow files  
**Duration**: < 5 minutes  
**Jobs**: Syntax validation, best practices, security checks

### Package Signing (`sign-packages.yml`)
**Purpose**: Reusable package signing  
**Trigger**: Called from other workflows  
**Type**: Reusable workflow

## Templates

### Pull Request Template
Standardized PR description with:
- Change type checklist
- Testing requirements
- Documentation checklist
- CI/CD verification
- Breaking change documentation

### Issue Templates

**Bug Report**: Structured bug reporting with environment details  
**Feature Request**: Feature proposals with DPI checklist  
**CI/CD Issue**: Pipeline and workflow problem reporting

## Configuration Files

### GitVersion.yml
Defines semantic versioning strategy:
- **main**: Release versions (e.g., 1.0.0)
- **develop**: Alpha versions (e.g., 1.1.0-alpha.1)
- **release/***: RC versions (e.g., 1.0.0-rc.1)
- **feature/***: Alpha versions
- **hotfix/***: Beta versions

## Documentation

### CI-CD-PIPELINE.md
Comprehensive documentation covering:
- Pipeline architecture
- Workflow details
- Getting started guide
- Versioning strategy
- Release process
- Troubleshooting

### CI-CD-QUICK-REFERENCE.md
Quick reference for:
- Daily developer tasks
- Release tasks
- Documentation tasks
- Monitoring tasks
- Common commands

### CONVENTIONAL_COMMITS.md
Guide for commit message format:
- Commit types and structure
- Examples and patterns
- Version control
- Best practices
- CI/CD integration

## Secrets Required

Configure these in GitHub Settings → Secrets → Actions:

| Secret | Required | Purpose |
|--------|----------|---------|
| `NUGET_API_KEY` | Yes | Publishing to NuGet.org |
| `SIGNING_CERTIFICATE` | No | Package signing (base64) |
| `CERTIFICATE_PASSWORD` | No | Certificate password |
| `GITHUB_TOKEN` | Auto | GitHub API access (provided) |

## Environment Configuration

### Staging
- URL: https://staging.example.com
- Auto-deploy from develop
- No approval required

### Canary
- URL: https://canary.example.com
- 10% traffic split
- Approval: DevOps team

### Production
- URL: https://production.example.com
- Full traffic
- Approval: DevOps + Product team

## Best Practices

### Workflow Development
1. Test locally with [act](https://github.com/nektos/act)
2. Validate syntax with workflow-validation
3. Use explicit permissions
4. Set timeout-minutes
5. Enable concurrency control
6. Cache dependencies

### Commit Messages
1. Follow conventional commits
2. Reference issues with #number
3. Mark breaking changes with `!` or footer
4. Use `+semver:` for explicit version control

### Pull Requests
1. Keep PRs small and focused
2. Fill out PR template completely
3. Wait for all CI checks to pass
4. Request reviews from appropriate teams
5. Address feedback promptly

### Releases
1. Ensure all tests pass on main
2. Review and update CHANGELOG.md
3. Use workflow dispatch for releases
4. Start with staging environment
5. Monitor canary before production
6. Verify NuGet.org publication

## Monitoring

### Pipeline Metrics
- PR check duration: Target < 10 minutes
- Integration CI: Target < 20 minutes
- Release CD: Target < 45 minutes
- Test success rate: Target > 95%

### Drift Detection
- Runs daily at 3 AM UTC
- Creates issues for detected drift
- Blocks critical drift in deployments

### Security Scanning
- CodeQL: Weekly + PR
- Dependencies: PR + weekly
- Secrets: Every PR
- Packages: Every build

## Troubleshooting

### Workflow Not Running
1. Check trigger conditions match your branch/event
2. Verify workflow file is in `.github/workflows/`
3. Check YAML syntax is valid
4. Review workflow permissions

### Job Failures
1. Check job logs in Actions tab
2. Download artifacts for reports
3. Test locally when possible
4. Verify secrets are configured

### Permission Errors
1. Check workflow has required permissions
2. Verify secrets exist and are accessible
3. Check token scopes for GitHub API calls

### Timeout Issues
1. Increase `timeout-minutes` if legitimate
2. Optimize slow steps
3. Use caching for dependencies
4. Split large jobs into smaller ones

## Contributing

When adding new workflows:

1. Follow existing patterns
2. Add appropriate documentation
3. Test thoroughly
4. Update this README
5. Run workflow-validation
6. Get review from DevOps team

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [GitVersion Documentation](https://gitversion.net/)
- [DocFX Documentation](https://dotnet.github.io/docfx/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Keep a Changelog](https://keepachangelog.com/)

## Support

For help with CI/CD:
1. Check [CI-CD-PIPELINE.md](CI-CD-PIPELINE.md)
2. Check [CI-CD-QUICK-REFERENCE.md](CI-CD-QUICK-REFERENCE.md)
3. Review workflow logs
4. Create issue with `ci-cd` label
5. Contact DevOps team

---

**Maintained by**: DevOps Team  
**Last Updated**: 2024-12-18  
**Version**: 1.0.0
