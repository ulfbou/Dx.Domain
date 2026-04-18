using System.Runtime.CompilerServices;

using Dx.Domain.Annotations;

[assembly: InternalsVisibleTo("Dx.Domain.Analyzers")]
[assembly: InternalsVisibleTo("Dx.Domain.Facts")]
[assembly: InternalsVisibleTo("Dx.Domain.Generators")]
[assembly: InternalsVisibleTo("Dx.Domain.Tests")]
[assembly: InternalsVisibleTo("Dx.Domain.Analyzers.Tests")]
[assembly: InternalsVisibleTo("Dx.Domain.Generators.Tests")]
[assembly: DxLayer("Kernel")]
[assembly: DpiJustified("Kernel API baseline")]
[assembly: DxAssemblyRole(DxAssemblyRole.Domain)]
