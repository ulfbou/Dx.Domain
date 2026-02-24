$ bash /f/scripts/concat_files.sh . csproj,props,targets /f/tmp/dx.domain.csproj.md
Starting concatenation...
Root Directory: .
Target Extensions: .csproj .props .targets
Output File: /f/tmp/dx.domain.csproj.md
Processing: ./builds/ci/Dx.CI.props
Processing: ./builds/common/Dx.Constants.props
Processing: ./builds/common/Dx.Defaults.props
Processing: ./builds/common/Dx.ProjectKinds.props
Processing: ./builds/common/Dx.Versions.props
Processing: ./builds/Dx.Props.hierarchical.props
Processing: ./builds/Dx.Props.Hierarchical.targets
Processing: ./builds/identity/Dx.Identity.props
Processing: ./builds/identity/Dx.ResolveIdentity.targets
Processing: ./builds/policy/Dx.DomainAnalyzerGovernance.targets
Processing: ./builds/policy/Dx.Forbidden.props
Processing: ./builds/policy/Dx.Governance.targets
Processing: ./Directory.Build.props
Processing: ./Directory.Build.targets
Processing: ./src/Dx.Domain.Analyzers/Dx.Domain.Analyzers.csproj
Processing: ./src/Dx.Domain.Annotations/Directory.Build.props
Processing: ./src/Dx.Domain.Annotations/Dx.Domain.Annotations.csproj
Processing: ./src/Dx.Domain.Facts/Dx.Domain.Facts.csproj
Processing: ./src/Dx.Domain.Generators/Dx.Domain.Generators.csproj
Processing: ./src/Dx.Domain.Kernel/build/Dx.Domain.props
Processing: ./src/Dx.Domain.Kernel/Directory.Build.props
Processing: ./src/Dx.Domain.Kernel/Dx.Domain.Kernel.csproj
Processing: ./src/Dx.Domain.Persistence/Dx.Domain.Persistence.csproj
Processing: ./src/Dx.Domain.Primitives/Directory.Build.props
Processing: ./src/Dx.Domain.Primitives/Dx.Domain.Primitives.csproj
Processing: ./src/Dx.Domain.Transport/Dx.Domain.Transport.csproj
Processing: ./tests/Directory.Build.props
Processing: ./tests/Dx.Domain.Analyzers.Tests/Dx.Domain.Analyzers.Tests.csproj
Processing: ./tests/Dx.Domain.Generators.Tests/Dx.Domain.Generators.Tests.csproj
Concatenation complete. Output written to /f/tmp/dx.domain.csproj.md

Path: /f/repos/ulfbou/dx.domain
$ # Rensa, restore, och bygg alla i Release
dotnet clean
dotnet restore

# 1) Analyzers + Annotations först (analyzers-dll behövs för de andra)
dotnet build -c Release ./src/Dx.Domain.Analyzers
dotnet build -c Release ./src/Dx.Domain.Annotations

# 2) Kernel och Primitives (packar in analyzers + har buildTransitive i Kernel)
dotnet build -c Release ./src/Dx.Domain.Kernel
dotnet build -c Release ./src/Dx.Domain.Primitives

# 3) Facts (beror på Kernel + Primitives + Annotations)
dotnet build -c Release ./src/Dx.Domain.Facts

# Packa nu – gör gärna pack separat för tydlighet
dotnet pack -c Release ./src/Dx.Domain.Annotations
dotnet pack -c Release ./src/Dx.Domain.Kernel
dotnet pack -c Release ./src/Dx.Domain.Primitives
dotnet pack -c Release ./src/Dx.Domain.Facts

Build succeeded in 2,0s
Restore complete (2,0s)

Build succeeded in 2,2s
Restore complete (0,5s)
  Dx.Domain.Annotations netstandard2.0 succeeded (1,6s) → src\Dx.Domain.Annotations\bin\Release\netstandard2.0\Dx.Domain.Annotations.dll
  Dx.Domain.Analyzers netstandard2.0 succeeded (1,5s) → src\Dx.Domain.Analyzers\bin\Release\netstandard2.0\Dx.Domain.Analyzers.dll

Build succeeded in 4,5s
Restore complete (0,4s)
  Dx.Domain.Annotations netstandard2.0 succeeded (0,2s) → src\Dx.Domain.Annotations\bin\Release\netstandard2.0\Dx.Domain.Annotations.dll

Build succeeded in 1,1s
Restore complete (0,7s)
  Dx.Domain.Annotations netstandard2.0 succeeded (0,2s) → src\Dx.Domain.Annotations\bin\Release\netstandard2.0\Dx.Domain.Annotations.dll
  Dx.Domain.Analyzers netstandard2.0 succeeded (0,3s) → src\Dx.Domain.Analyzers\bin\Release\netstandard2.0\Dx.Domain.Analyzers.dll
  Dx.Domain.Kernel net9.0 succeeded (2,6s) → src\Dx.Domain.Kernel\bin\Release\net9.0\Dx.Domain.Kernel.dll
  Dx.Domain.Kernel net8.0 succeeded (2,6s) → src\Dx.Domain.Kernel\bin\Release\net8.0\Dx.Domain.Kernel.dll
  Dx.Domain.Kernel net10.0 succeeded (2,6s) → src\Dx.Domain.Kernel\bin\Release\net10.0\Dx.Domain.Kernel.dll

Build succeeded in 4,3s
Restore complete (0,9s)
  Dx.Domain.Annotations netstandard2.0 succeeded (0,1s) → src\Dx.Domain.Annotations\bin\Release\netstandard2.0\Dx.Domain.Annotations.dll
  Dx.Domain.Primitives net9.0 succeeded (0,4s) → src\Dx.Domain.Primitives\bin\Release\net9.0\Dx.Domain.Primitives.dll
  Dx.Domain.Primitives net10.0 succeeded (0,5s) → src\Dx.Domain.Primitives\bin\Release\net10.0\Dx.Domain.Primitives.dll
  Dx.Domain.Primitives net8.0 succeeded (0,6s) → src\Dx.Domain.Primitives\bin\Release\net8.0\Dx.Domain.Primitives.dll

Build succeeded in 2,0s
Restore complete (0,7s)
  Dx.Domain.Annotations netstandard2.0 succeeded (0,2s) → src\Dx.Domain.Annotations\bin\Release\netstandard2.0\Dx.Domain.Annotations.dll
  Dx.Domain.Primitives net10.0 succeeded (0,1s) → src\Dx.Domain.Primitives\bin\Release\net10.0\Dx.Domain.Primitives.dll
  Dx.Domain.Analyzers netstandard2.0 succeeded (0,1s) → src\Dx.Domain.Analyzers\bin\Release\netstandard2.0\Dx.Domain.Analyzers.dll
  Dx.Domain.Kernel net10.0 succeeded (0,1s) → src\Dx.Domain.Kernel\bin\Release\net10.0\Dx.Domain.Kernel.dll
  Dx.Domain.Facts net10.0 succeeded (0,2s) → src\Dx.Domain.Facts\bin\Release\net10.0\Dx.Domain.Facts.dll

Build succeeded in 2,0s
Restore complete (0,4s)
  Dx.Domain.Annotations netstandard2.0 succeeded (0,5s) → src\Dx.Domain.Annotations\bin\Release\netstandard2.0\Dx.Domain.Annotations.dll

Build succeeded in 1,5s
Restore complete (0,7s)
  Dx.Domain.Annotations netstandard2.0 succeeded (0,1s) → src\Dx.Domain.Annotations\bin\Release\netstandard2.0\Dx.Domain.Annotations.dll
  Dx.Domain.Analyzers netstandard2.0 succeeded (0,1s) → src\Dx.Domain.Analyzers\bin\Release\netstandard2.0\Dx.Domain.Analyzers.dll
  Dx.Domain.Kernel net8.0 succeeded (0,2s) → src\Dx.Domain.Kernel\bin\Release\net8.0\Dx.Domain.Kernel.dll
  Dx.Domain.Kernel net10.0 succeeded (0,2s) → src\Dx.Domain.Kernel\bin\Release\net10.0\Dx.Domain.Kernel.dll
  Dx.Domain.Kernel net9.0 succeeded (0,3s) → src\Dx.Domain.Kernel\bin\Release\net9.0\Dx.Domain.Kernel.dll
  Dx.Domain.Kernel failed with 1 error(s) (0,2s)
    C:\Program Files\dotnet\sdk\10.0.102\NuGet.Build.Tasks.Pack.targets(222,5): error Could not find a part of the path 'F:\repos\ulfbou\dx.domain\src\Dx.Domain.Kernel\buildTransitive'.

Build failed with 1 error(s) in 2,0s
Restore complete (0,7s)
  Dx.Domain.Annotations netstandard2.0 succeeded (0,1s) → src\Dx.Domain.Annotations\bin\Release\netstandard2.0\Dx.Domain.Annotations.dll
  Dx.Domain.Primitives net10.0 succeeded (0,2s) → src\Dx.Domain.Primitives\bin\Release\net10.0\Dx.Domain.Primitives.dll
  Dx.Domain.Primitives net9.0 succeeded (0,2s) → src\Dx.Domain.Primitives\bin\Release\net9.0\Dx.Domain.Primitives.dll
  Dx.Domain.Primitives net8.0 succeeded (0,3s) → src\Dx.Domain.Primitives\bin\Release\net8.0\Dx.Domain.Primitives.dll

Build succeeded in 2,0s
Restore complete (0,7s)
  Dx.Domain.Annotations netstandard2.0 succeeded (0,2s) → src\Dx.Domain.Annotations\bin\Release\netstandard2.0\Dx.Domain.Annotations.dll
  Dx.Domain.Primitives net10.0 succeeded (0,2s) → src\Dx.Domain.Primitives\bin\Release\net10.0\Dx.Domain.Primitives.dll
  Dx.Domain.Analyzers netstandard2.0 succeeded (0,1s) → src\Dx.Domain.Analyzers\bin\Release\netstandard2.0\Dx.Domain.Analyzers.dll
  Dx.Domain.Kernel net10.0 succeeded (0,1s) → src\Dx.Domain.Kernel\bin\Release\net10.0\Dx.Domain.Kernel.dll
  Dx.Domain.Facts net10.0 succeeded (0,4s) → src\Dx.Domain.Facts\bin\Release\net10.0\Dx.Domain.Facts.dll

Build succeeded in 2,4s

