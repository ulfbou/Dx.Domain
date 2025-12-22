# CI/CD Quick Reference

Quick reference for common CI/CD tasks and commands.

## Daily Developer Tasks

### Creating a PR
```bash
# Create feature branch
git checkout -b feature/my-feature

# Make changes and commit
git add .
git commit -m "feat: add new feature"

# Push and create PR
git push origin feature/my-feature
# Then create PR in GitHub UI
```

### Checking PR Status
- Go to Actions tab in GitHub
- Look for "Pre-merge CI" workflow
- Check all jobs are green ✅
- Review DocFX preview if docs changed

### Fixing CI Failures

**Format Issues**:
```bash
cd Dx.Domain.2
dotnet format
git add .
git commit -m "style: fix formatting"
git push
```

**Test Failures**:
```bash
# Run failed tests locally
dotnet test --filter "FullyQualifiedName~FailedTest"

# Fix and verify
dotnet test
git add .
git commit -m "fix: resolve test failure"
git push
```

**Vulnerable Dependencies**:
```bash
# List vulnerabilities
dotnet list package --vulnerable --include-transitive

# Update package
dotnet add package PackageName --version SafeVersion
git add .
git commit -m "fix: update vulnerable dependency"
git push
```

## Release Tasks

### Triggering a Release

1. **Navigate**: Actions → Release CD → Run workflow
2. **Select**:
   - Branch: `main`
   - Release type: `patch` | `minor` | `major`
   - Environment: `staging`
3. **Click**: Run workflow
4. **Wait**: ~30-45 minutes
5. **Verify**: Check staging deployment
6. **Promote**: Re-run for `canary`, then `production`

### Release Checklist

- [ ] All PRs merged to main
- [ ] Integration CI passing
- [ ] No critical drift detected
- [ ] Changelog reviewed
- [ ] Version bump correct
- [ ] Secrets configured (NUGET_API_KEY)

### Emergency Rollback

```bash
# Option 1: Revert last commit
git revert HEAD
git push origin main

# Option 2: Download previous release from GitHub
# Go to Releases → Download assets → Redeploy

# Option 3: NuGet users can downgrade
dotnet add package PackageName --version PreviousVersion
```

## Documentation Tasks

### Building Docs Locally

```bash
cd Dx.Domain.2

# Build solution first
dotnet build --configuration Release

# Build docs
docfx docfx.json

# Serve locally at http://localhost:8080
docfx serve _site
```

### Updating API Docs

API docs are auto-generated from XML comments:

```csharp
/// <summary>
/// Description of the method
/// </summary>
/// <param name="param1">Description of parameter</param>
/// <returns>Description of return value</returns>
public Result<string> MyMethod(string param1)
{
    // Implementation
}
```

### Adding Conceptual Docs

1. Create markdown file in `Dx.Domain.2/docs/`
2. Add to `toc.yml`
3. Build and preview
4. Create PR

## Monitoring Tasks

### Checking Pipeline Health

**Recent Runs**:
- Actions tab → View all workflows
- Look for patterns of failures

**Drift Status**:
- Actions → Drift Protection → Latest run
- Review drift report artifact

**Documentation Status**:
- Visit: `https://your-org.github.io/Notes/`
- Check latest version published

### Key Metrics to Watch

```bash
# PR check duration (target: <10 min)
# Integration CI duration (target: <20 min)
# Test success rate (target: >95%)
# Critical drift (target: 0)
```

## Common Commands

### Build Commands

```bash
# Restore dependencies
dotnet restore Dx.Domain.sln

# Build release configuration
dotnet build Dx.Domain.sln --configuration Release

# Build with version
dotnet build -p:Version=1.2.3

# Clean build
dotnet clean && dotnet build
```

### Test Commands

```bash
# Run all tests
dotnet test

# Run fast tests only
dotnet test --filter "Category!=Integration&Category!=Slow"

# Run specific test
dotnet test --filter "FullyQualifiedName~MyTestClass.MyTest"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Package Commands

```bash
# Create packages
dotnet pack --configuration Release

# Create with specific version
dotnet pack -p:PackageVersion=1.2.3

# Publish to GitHub Packages
dotnet nuget push package.nupkg --source github --api-key TOKEN

# Publish to NuGet.org
dotnet nuget push package.nupkg --source https://api.nuget.org/v3/index.json --api-key KEY
```

### Git Commands

```bash
# Check status
git status

# View diff
git --no-pager diff

# View log
git --no-pager log --oneline -10

# Create tag
git tag -a v1.0.0 -m "Release 1.0.0"
git push origin v1.0.0

# View tags
git tag -l
```

## Workflow File Locations

```
.github/
├── workflows/
│   ├── pre-merge-ci.yml       # PR checks
│   ├── integration-ci.yml     # Main branch builds
│   ├── release-cd.yml         # Release deployments
│   ├── docfx.yml             # Documentation
│   ├── drift-protection.yml   # Drift detection
│   └── sign-packages.yml      # Package signing
├── GitVersion.yml             # Version configuration
└── CI-CD-PIPELINE.md         # Full documentation
```

## Troubleshooting Quick Fixes

### "Workflow not found"
- Check file is in `.github/workflows/`
- Verify YAML syntax is valid
- Push to repository

### "Permission denied"
- Check workflow has correct `permissions:`
- Verify secret is configured
- Check secret name matches

### "Version calculation failed"
- Verify GitVersion.yml exists
- Check branch name matches patterns
- Ensure fetch-depth: 0 in checkout

### "Tests hang"
- Add `timeout-minutes:` to job
- Check for infinite loops
- Review test logs for blocking operations

### "Drift false positive"
- Review drift report
- Adjust thresholds in workflow
- Add exception if intentional

## Getting Help

1. **Documentation**: Read `.github/CI-CD-PIPELINE.md`
2. **Workflow Logs**: Check Actions tab for detailed logs
3. **Artifacts**: Download reports and logs from workflow runs
4. **Issues**: Create GitHub issue with `ci-cd` label
5. **Discussion**: Use GitHub Discussions for questions

## Useful Links

- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [GitVersion Docs](https://gitversion.net/docs/)
- [DocFX Docs](https://dotnet.github.io/docfx/)
- [Semantic Versioning](https://semver.org/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

**Quick Access**: Bookmark this page for fast reference!
