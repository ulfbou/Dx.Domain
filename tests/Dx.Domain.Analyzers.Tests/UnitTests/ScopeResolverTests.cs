using AnalyzerScope = Dx.Domain.Analyzers.Infrastructure.Scopes.Scope;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Moq;

using Xunit;

using Assert = Xunit.Assert;

namespace Dx.Domain.Analyzers.Tests.UnitTests
{
    public class ScopeResolverTests
    {
        [Fact]
        public void DxLayer_Config_Kernel_Maps_To_S0()
        {
            var config = CreateConfig(new Dictionary<string, string>
            {
                ["build_property.DxLayer"] = "Kernel"
            });
            var resolver = new ScopeResolver(config);

            var scope = resolver.ResolveAssembly(CreateAssembly("SomeLibrary"));

            Assert.Equal(AnalyzerScope.S0, scope);
        }

        [Fact]
        public void DxLayerAttribute_Is_Respected_When_Config_Missing()
        {
            var config = CreateConfig(new Dictionary<string, string>());
            var resolver = new ScopeResolver(config);

            var assembly = CreateAssemblyWithAttribute("DxLayer", "Kernel");
            var scope = resolver.ResolveAssembly(assembly);

            Assert.Equal(AnalyzerScope.S0, scope);
        }

        [Fact]
        public void Authority_Layer_Is_Treated_As_S0()
        {
            var config = CreateConfig(new Dictionary<string, string>
            {
                ["build_property.DxLayer"] = "Authority"
            });
            var resolver = new ScopeResolver(config);

            var scope = resolver.ResolveAssembly(CreateAssembly("SomeLibrary"));

            Assert.Equal(AnalyzerScope.S0, scope);
        }

        [Fact]
        public void Default_Is_Consumer_When_No_Signal()
        {
            var config = CreateConfig(new Dictionary<string, string>());
            var resolver = new ScopeResolver(config);

            var scope = resolver.ResolveAssembly(CreateAssembly("SomeLibrary"));

            Assert.Equal(AnalyzerScope.S3, scope);
        }

        [Fact]
        public void Test_Project_Is_Excluded_From_Consumer()
        {
            var config = CreateConfig(new Dictionary<string, string>
            {
                ["build_property.IsTestProject"] = "true"
            });
            var resolver = new ScopeResolver(config);

            var scope = resolver.ResolveAssembly(CreateAssembly("SomeLibrary"));

            Assert.Equal(AnalyzerScope.S0, scope);
        }

        [Fact]
        public void ResolveSymbol_Uses_Containing_Assembly()
        {
            var config = CreateConfig(new Dictionary<string, string>
            {
                ["build_property.DxLayer"] = "Kernel"
            });
            var resolver = new ScopeResolver(config);

            var code = "namespace TestNs { public class TestClass {} }";
            var compilation = CSharpCompilation.Create(
                "TestAssembly",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var symbol = compilation.GetTypeByMetadataName("TestNs.TestClass");
            Assert.NotNull(symbol);

            var scope = resolver.ResolveSymbol(symbol!);
            Assert.Equal(AnalyzerScope.S0, scope);
        }

        private static AnalyzerConfigOptionsProvider CreateConfig(Dictionary<string, string> values)
        {
            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();

            mockOptions.Setup(o => o.TryGetValue(It.IsAny<string>(), out It.Ref<string?>.IsAny))
                .Returns((string key, out string value) =>
                {
                    if (values.TryGetValue(key, out var configured))
                    {
                        value = configured;
                        return true;
                    }

                    value = string.Empty;
                    return false;
                });

            mockConfig.Setup(c => c.GlobalOptions).Returns(mockOptions.Object);
            return mockConfig.Object;
        }

        private static IAssemblySymbol CreateAssembly(string name)
        {
            var code = "namespace Test { public class Foo {} }";
            var compilation = CSharpCompilation.Create(
                name,
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            return compilation.Assembly;
        }

        private static IAssemblySymbol CreateAssemblyWithAttribute(string attributeName, string layer)
        {
            var code = $@"""
using Dx.Domain.Annotations;

[assembly: {attributeName}(""{layer}"")]

namespace Test {{ public class Foo {{ }} }}""";

            var compilation = CSharpCompilation.Create(
                "TestAssembly",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(DxLayerAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return compilation.Assembly;
        }

    }
}
