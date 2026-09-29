# Development setup

## Prerequisites

- .NET SDK 8.0 or later (required for build and tests)
- Visual Studio 2022 or Visual Studio Code (recommended)
- Git for version control
- PowerShell or Bash for script execution

## Initial environment setup

Clone the repository and restore dependencies:

```bash
git clone https://github.com/ulfbou/Dx.Domain.git
cd Dx.Domain
dotnet tool restore
dotnet restore
```

The `dotnet tool restore` command installs pinned analyzer tooling and DocFX. The `dotnet restore` command fetches all package dependencies.

## Building and testing

From the repository root:

```bash
dotnet build -c Release
dotnet test -c Release --no-build
```

The Release configuration enables analyzer enforcement and optimizations. The `--no-build` flag reuses the build output.

## Before submitting contributions

Always run the complete [local validation](local-validation.md) sequence before committing. This ensures your changes pass:

- Code compilation and runtime tests
- Documentation link validation
- Code snippet compilation
- Documentation example material compilation
- Full DocFX publication test

Stop at the first failure, correct it, and restart the sequence.

## IDE setup

**Visual Studio 2022:**
- Open `Dx.Domain.sln`
- Tools → Options → Text Editor → C# → Code Style → Naming → use the checked-in `.editorconfig`
- Run tests with Test Explorer

**Visual Studio Code:**
- Install the C# extension
- Open the repository root folder
- Run `.vscode/tasks.json` tasks or use the terminal with the commands above

## Frequently checked practices

- Follow [style guide](style-guide.md) for documentation and code commentary
- Follow [Conventional Commits](conventional_commits.md) for commit messages
- Ensure all changes pass the canonical [local validation](local-validation.md) sequence
- Verify analyzer metadata with `scripts/verify-diagnostic-conformance.py`
