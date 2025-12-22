using Dx.Domain.Analyzers.Infrastructure;
using Dx.Domain.Analyzers.Infrastructure.Exceptions;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Flow;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;
using Dx.Domain.Analyzers.Infrastructure.Semantics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Moq;

using Xunit;

using Assert = Xunit.Assert;

namespace Dx.Domain.Analyzers.Tests.UnitTests
{
    public class AnalyzerServicesTests
    {
        [Fact]
        public void AnalyzerServices_Is_Sealed()
        {
            var type = typeof(AnalyzerServices);
            Assert.True(type.IsSealed);
        }

        [Fact]
        public void AnalyzerServices_Properties_Are_NonNull()
        {
            var services = CreateTestServices();

            Assert.NotNull(services.Scope);
            Assert.NotNull(services.Dx);
            Assert.NotNull(services.Semantic);
            Assert.NotNull(services.Exceptions);
            Assert.NotNull(services.Flow);
            Assert.NotNull(services.Generated);
        }

        [Fact]
        public void AnalyzerServices_Can_Be_Reused_Across_Multiple_Invocations()
        {
            var services = CreateTestServices();

            var scope1 = services.Scope;
            var scope2 = services.Scope;
            var dx1 = services.Dx;
            var dx2 = services.Dx;

            Assert.Same(scope1, scope2);
            Assert.Same(dx1, dx2);
        }

        [Fact]
        public void AnalyzerServices_Record_Is_Immutable()
        {
            var services = CreateTestServices();

            var type = typeof(AnalyzerServices);
            foreach (var property in type.GetProperties())
            {
                var setMethod = property.GetSetMethod();
                Assert.True(setMethod == null || setMethod.ReturnParameter.GetRequiredCustomModifiers()
                    .Any(m => m.Name == "IsExternalInit"));
            }
        }

        private static AnalyzerServices CreateTestServices()
        {
            var code = "namespace Test { public class Foo {} }";
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();
            mockConfig.Setup(c => c.GlobalOptions).Returns(mockOptions.Object);

            var scopeResolver = new ScopeResolver(mockConfig.Object);
            var facadeResolver = new DxFacadeResolver(compilation, mockConfig.Object);
            var semanticClassifier = new SemanticClassifier(compilation);
            var exceptionClassifier = new ExceptionIntentClassifier(compilation, mockConfig.Object);
            var flowWrapper = new ResultFlowEngineWrapper();
            var generatedDetector = new GeneratedCodeDetector(mockConfig.Object);

            return new AnalyzerServices(
                scopeResolver,
                facadeResolver,
                semanticClassifier,
                exceptionClassifier,
                flowWrapper,
                generatedDetector);
        }
    }
}
