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

            // Simulate multiple rule invocations
            var scope1 = services.Scope;
            var scope2 = services.Scope;
            var dx1 = services.Dx;
            var dx2 = services.Dx;

            // Same instance should be returned (immutable)
            Assert.Same(scope1, scope2);
            Assert.Same(dx1, dx2);
        }

        [Fact]
        public void AnalyzerServices_Record_Is_Immutable()
        {
            var services = CreateTestServices();

            // Verify that properties are init-only (cannot be set after construction)
            var type = typeof(AnalyzerServices);
            foreach (var property in type.GetProperties())
            {
                var setMethod = property.GetSetMethod();
                // In C# records with init accessors, SetMethod will be non-null but special
                // We verify immutability by ensuring we can't reassign after construction
                Assert.True(setMethod == null || setMethod.ReturnParameter.GetRequiredCustomModifiers()
                    .Any(m => m.Name == "IsExternalInit"));
            }
        }

        private static AnalyzerServices CreateTestServices()
        {
            var code = "namespace Test { public class Foo {} }";
            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();
            mockConfig.Setup(c => c.GlobalOptions).Returns(mockOptions.Object);

            var scopeResolver = new ScopeResolver(mockConfig.Object);
            var facadeResolver = new DxFacadeResolver(compilation);
            var semanticClassifier = new SemanticClassifier(compilation);
            var exceptionClassifier = new ExceptionIntentClassifier();
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
