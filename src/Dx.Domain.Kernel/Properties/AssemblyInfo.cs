using System.Runtime.CompilerServices;

using Dx.Domain;

[assembly: InternalsVisibleTo("Dx.Domain.Analyzers")]
[assembly: InternalsVisibleTo("Dx.Domain.Facts")]
[assembly: InternalsVisibleTo("Dx.Domain.Generators")]
[assembly: InternalsVisibleTo("Dx.Domain.Tests")]
[assembly: InternalsVisibleTo("Dx.Domain.Analyzers.Tests")]
[assembly: InternalsVisibleTo("Dx.Domain.Generators.Tests")]
[assembly: DxAssemblyRole(DxAssemblyRole.Domain)]
