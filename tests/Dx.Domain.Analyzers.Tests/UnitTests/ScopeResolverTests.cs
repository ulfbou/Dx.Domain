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
        public void Explicit_Map_Maps_DxDomain_To_S0()
        {
            var config = CreateConfigWithMap("Dx.Domain=S0");
            var resolver = new ScopeResolver(config);

            var assembly = CreateAssembly("Dx.Domain");
            var scope = resolver.ResolveAssembly(assembly);

            Assert.Equal(Scope.S0, scope);
        }

        [Fact]
        public void Missing_Map_Returns_S3_For_Arbitrary_Assemblies()
        {
            var config = CreateConfigWithMap(null);
            var resolver = new ScopeResolver(config);

            var assembly = CreateAssembly("SomeLibrary");
            var scope = resolver.ResolveAssembly(assembly);

            Assert.Equal(Scope.S3, scope);
        }

        [Fact]
        public void Assembly_Not_In_Map_Returns_S3()
        {
            var config = CreateConfigWithMap("Dx.Domain=S0");
            var resolver = new ScopeResolver(config);

            var assembly = CreateAssembly("UnmappedAssembly");
            var scope = resolver.ResolveAssembly(assembly);

            Assert.Equal(Scope.S3, scope);
        }

        [Fact]
        public void Root_Namespace_Prefix_Match_Returns_S3()
        {
            var config = CreateConfigWithRootNamespaces("MyCompany.Domain");
            var resolver = new ScopeResolver(config);

            var assembly = CreateAssemblyWithNamespace("MyCompany.Domain.Orders");
            var scope = resolver.ResolveAssembly(assembly);

            Assert.Equal(Scope.S3, scope);
        }

        [Fact]
        public void Multiple_Root_Namespaces_Are_Supported()
        {
            var config = CreateConfigWithRootNamespaces("MyCompany.Domain;MyCompany.Shared");
            var resolver = new ScopeResolver(config);

            var assembly1 = CreateAssemblyWithNamespace("MyCompany.Domain.Orders");
            var assembly2 = CreateAssemblyWithNamespace("MyCompany.Shared.Utils");

            Assert.Equal(Scope.S3, resolver.ResolveAssembly(assembly1));
            Assert.Equal(Scope.S3, resolver.ResolveAssembly(assembly2));
        }

        [Fact]
        public void Whitespace_In_Config_Is_Handled()
        {
            var config = CreateConfigWithMap(" Dx.Domain = S0 ; OtherLib = S1 ");
            var resolver = new ScopeResolver(config);

            var assembly = CreateAssembly("Dx.Domain");
            var scope = resolver.ResolveAssembly(assembly);

            Assert.Equal(Scope.S0, scope);
        }

        [Fact]
        public void Invalid_Enum_Values_Do_Not_Throw()
        {
            var config = CreateConfigWithMap("Dx.Domain=InvalidScope");
            var resolver = new ScopeResolver(config);

            var assembly = CreateAssembly("Dx.Domain");
            // Should not throw, should return S3 as default
            var scope = resolver.ResolveAssembly(assembly);

            Assert.Equal(Scope.S3, scope);
        }

        [Fact]
        public void Multiple_Assemblies_Can_Be_Mapped()
        {
            var config = CreateConfigWithMap("Dx.Domain=S0;Dx.Shared=S1;MyApp=S2");
            var resolver = new ScopeResolver(config);

            Assert.Equal(Scope.S0, resolver.ResolveAssembly(CreateAssembly("Dx.Domain")));
            Assert.Equal(Scope.S1, resolver.ResolveAssembly(CreateAssembly("Dx.Shared")));
            Assert.Equal(Scope.S2, resolver.ResolveAssembly(CreateAssembly("MyApp")));
        }

        [Fact]
        public void ResolveSymbol_Uses_Containing_Assembly()
        {
            var config = CreateConfigWithMap("TestAssembly=S0");
            var resolver = new ScopeResolver(config);

            var code = "namespace TestNs { public class TestClass {} }";
            var compilation = CSharpCompilation.Create(
                "TestAssembly",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var symbol = compilation.GetTypeByMetadataName("TestNs.TestClass");
            Assert.NotNull(symbol);

            var scope = resolver.ResolveSymbol(symbol!);
            Assert.Equal(Scope.S0, scope);
        }

        [Fact]
        public void Empty_Config_Values_Are_Ignored()
        {
            var config = CreateConfigWithMap(";;Dx.Domain=S0;;");
            var resolver = new ScopeResolver(config);

            var assembly = CreateAssembly("Dx.Domain");
            var scope = resolver.ResolveAssembly(assembly);

            Assert.Equal(Scope.S0, scope);
        }

        [Fact]
        public void Root_Namespaces_Empty_Values_Are_Ignored()
        {
            var config = CreateConfigWithRootNamespaces(";;MyCompany.Domain;;");
            var resolver = new ScopeResolver(config);

            var assembly = CreateAssemblyWithNamespace("MyCompany.Domain");
            var scope = resolver.ResolveAssembly(assembly);

            Assert.Equal(Scope.S3, scope);
        }

        private static AnalyzerConfigOptionsProvider CreateConfigWithMap(string? mapValue)
        {
            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();

            if (mapValue != null)
            {
                mockOptions.Setup(o => o.TryGetValue("dx.scope.map", out It.Ref<string?>.IsAny))
                    .Returns((string key, out string value) =>
                    {
                        value = mapValue;
                        return true;
                    });
            }

            mockConfig.Setup(c => c.GlobalOptions).Returns(mockOptions.Object);
            return mockConfig.Object;
        }

        private static AnalyzerConfigOptionsProvider CreateConfigWithRootNamespaces(string namespaces)
        {
            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();

            mockOptions.Setup(o => o.TryGetValue("dx.scope.rootNamespaces", out It.Ref<string?>.IsAny))
                .Returns((string key, out string value) =>
                {
                    value = namespaces;
                    return true;
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

        private static IAssemblySymbol CreateAssemblyWithNamespace(string namespaceName)
        {
            var code = $"namespace {namespaceName} {{ public class Foo {{}} }}";
            var compilation = CSharpCompilation.Create(
                "TestAssembly",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            return compilation.Assembly;
        }
    }
}
