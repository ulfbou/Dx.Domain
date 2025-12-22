# Conventional Commits Guide

This project follows the [Conventional Commits](https://www.conventionalcommits.org/) specification for commit messages.

## Why Conventional Commits?

- **Automated Versioning**: Semantic versioning based on commit messages
- **Automated Changelogs**: Generate release notes automatically
- **Better History**: Clear, searchable commit history
- **CI/CD Integration**: Workflows can react to commit types

## Commit Message Format

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

### Type

Must be one of the following:

| Type | Description | Version Bump |
|------|-------------|--------------|
| `feat` | New feature | Minor |
| `fix` | Bug fix | Patch |
| `docs` | Documentation only | None |
| `style` | Code style (formatting, semicolons, etc.) | None |
| `refactor` | Code refactoring | None |
| `perf` | Performance improvement | Patch |
| `test` | Adding or updating tests | None |
| `build` | Build system or dependencies | None |
| `ci` | CI/CD configuration | None |
| `chore` | Other changes | None |
| `revert` | Revert previous commit | None |

### Scope (Optional)

The scope provides additional contextual information:

```
feat(api): add new endpoint for user profile
fix(auth): resolve token expiration issue
docs(readme): update installation instructions
```

Common scopes in this project:
- `core`: Core domain library
- `api`: Public API
- `tests`: Test code
- `docs`: Documentation
- `ci`: CI/CD pipelines
- `deps`: Dependencies

### Description

- Use imperative, present tense: "add" not "added" nor "adds"
- Don't capitalize first letter
- No period (.) at the end
- Keep it under 72 characters

### Body (Optional)

- Provide additional context
- Explain the "why" not the "what"
- Use imperative mood
- Wrap at 72 characters

### Footer (Optional)

Used for:
- Breaking changes: `BREAKING CHANGE: <description>`
- Issue references: `Closes #123`, `Fixes #456`
- Version bump control: `+semver: major|minor|patch|none`

## Examples

### Simple Feature

```
feat: add result validation helper
```

### Feature with Scope and Body

```
feat(api): add support for async operations

Implement async/await pattern for I/O operations to improve
performance and scalability. This affects all API endpoints.
```

### Bug Fix

```
fix: resolve null reference in error handler

The error handler was not checking for null values before
accessing properties, causing crashes in edge cases.

Fixes #123
```

### Breaking Change

```
feat!: change Result<T> API signature

BREAKING CHANGE: Result<T> now requires explicit error type.
Users must update from Result<T> to Result<T, TError>.

Migration:
- Before: Result<string>
- After: Result<string, DomainError>

Closes #456
```

### Documentation

```
docs: update contributing guidelines

Add section on conventional commits and CI/CD workflows.
```

### Refactoring

```
refactor: extract validation logic to separate class

Improve code organization by moving validation logic from
domain entities to dedicated validator classes.
```

### Performance

```
perf: optimize database query in user lookup

Replace N+1 queries with single batch query, reducing
database round trips from N to 1.
```

### Version Control

```
feat: add new feature with explicit version bump

+semver: minor
```

```
fix: critical security fix

+semver: patch
```

```
docs: update readme

+semver: none
```

## Commit Message Checklist

Before committing, ensure:

- [ ] Type is correct and follows convention
- [ ] Scope is appropriate (if used)
- [ ] Description is clear and concise
- [ ] Imperative mood is used ("add" not "added")
- [ ] No period at the end of description
- [ ] Body provides context (if needed)
- [ ] Breaking changes are documented
- [ ] Issue references are included
- [ ] Version bump is specified (if needed)

## Tools

### Commitizen (Optional)

Interactive commit message generator:

```bash
# Install
npm install -g commitizen cz-conventional-changelog

# Use
git cz
```

### Commitlint (Optional)

Validate commit messages:

```bash
# Install
npm install -g @commitlint/cli @commitlint/config-conventional

# Setup
echo "module.exports = {extends: ['@commitlint/config-conventional']}" > commitlint.config.js

# Use with husky
npx husky add .husky/commit-msg 'npx --no -- commitlint --edit "$1"'
```

## Common Mistakes

### ❌ Wrong

```
Added new feature
```
- Not imperative mood
- Type missing

### ❌ Wrong

```
feat: Added new feature.
```
- Not imperative mood ("Added" should be "add")
- Period at end

### ❌ Wrong

```
updated docs
```
- Type missing
- Not imperative mood
- No description

### ✅ Correct

```
feat: add new feature
```

### ✅ Correct

```
docs: update installation guide
```

## Integration with CI/CD

Our CI/CD system uses conventional commits for:

1. **Automated Versioning**: GitVersion reads commit messages
2. **Changelog Generation**: Release notes are auto-generated
3. **Release Type**: Determines patch/minor/major releases
4. **PR Validation**: Checks commit message format

## Best Practices

1. **One Commit Per Logical Change**: Don't bundle multiple changes
2. **Write Clear Descriptions**: Others should understand without reading the code
3. **Use Body for Complex Changes**: Explain the reasoning
4. **Reference Issues**: Always link to relevant issues
5. **Mark Breaking Changes**: Use `!` or `BREAKING CHANGE:` footer
6. **Be Consistent**: Follow the convention strictly

## Quick Reference

```bash
# Feature
git commit -m "feat: add user authentication"
git commit -m "feat(api): add login endpoint"

# Bug Fix
git commit -m "fix: resolve memory leak"
git commit -m "fix(auth): handle expired tokens"

# Documentation
git commit -m "docs: update API reference"

# Refactoring
git commit -m "refactor: simplify error handling"

# Breaking Change
git commit -m "feat!: change API signature"

# With Issue
git commit -m "fix: resolve crash on startup" -m "Fixes #123"

# With Version Bump
git commit -m "feat: add feature" -m "+semver: minor"
```

## Resources

- [Conventional Commits Specification](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [Angular Commit Guidelines](https://github.com/angular/angular/blob/main/CONTRIBUTING.md#commit)
- [GitVersion Documentation](https://gitversion.net/docs/)

---

**Questions?** See [CI/CD Documentation](./.github/CI-CD-PIPELINE.md) or create an issue with the `ci-cd` label.
