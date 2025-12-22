using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Dx;
using Dx.Domain;
using Dx.Domain.Analyzers.Infrastructure.Facades;

using FluentAssertions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Moq;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.IntegrationTests
{
    public class FacadeReflectionGateTests
    {
        [Fact]
        public void DxFacadeResolver_Enumerates_Public_Static_Factories()
        {
            var compilation = CreateCompilation();

            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();
            mockConfig
                .Setup(c => c.GlobalOptions)
                .Returns(mockOptions.Object);

            var resolver = new DxFacadeResolver(compilation, mockConfig.Object);

            var factories = resolver.FacadeFactories;

            factories.Should().NotBeNull();
        }

        [Fact]
        public void DxFacadeResolver_Can_Resolve_Factory_By_Return_Type_When_Present()
        {
            var compilation = CreateCompilation();

            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();
            mockConfig
                .Setup(c => c.GlobalOptions)
                .Returns(mockOptions.Object);

            var resolver = new DxFacadeResolver(compilation, mockConfig.Object);

            var actorIdMetadataName = typeof(Domain.ActorId).FullName!;
            var actorIdSymbol = compilation.GetTypeByMetadataName(actorIdMetadataName);
            if (actorIdSymbol is not null)
            {
                var factory = resolver.FindFacadeFactoryForType(actorIdSymbol);
                factory.Should().NotBeNull();
            }
        }

        private static Compilation CreateCompilation()
        {
            var dxDomainAssembly = typeof(Result<>).Assembly;
            var dxFacadeAssembly = dxDomainAssembly;

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(dxDomainAssembly.Location)
            };

            var facadeLocation = dxFacadeAssembly.Location;
            if (!string.IsNullOrEmpty(facadeLocation) && File.Exists(facadeLocation))
            {
                if (!references.Any(r => string.Equals(r.Display, facadeLocation, System.StringComparison.OrdinalIgnoreCase)))
                {
                    references.Add(MetadataReference.CreateFromFile(facadeLocation));
                }
            }

            return CSharpCompilation.Create(
                assemblyName: "DxFacadeTestHost",
                syntaxTrees: System.Array.Empty<SyntaxTree>(),
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }
    }
}
