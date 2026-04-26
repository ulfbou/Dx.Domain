using Dx.Domain.Analyzers.Infrastructure.Facades;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Linq;

using Moq;

using Xunit;

using Assert = Xunit.Assert;

namespace Dx.Domain.Analyzers.Tests.UnitTests
{
    public class DxFacadeResolverTests
    {
        [Fact]
        public void Only_Public_Static_Facade_Methods_Are_Discovered()
        {
            var code = """
            public static class Dx
            {
                public static class Result
                {
                    public static int Ok() => 0;
                    internal static int Hidden() => 1;
                }

                internal static class Internal
                {
                    public static int Nope() => 2;
                }
            }
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var resolver = CreateResolver(compilation);

            Assert.Single(resolver.FacadeFactories);
            Assert.Equal("Ok", resolver.FacadeFactories.First().Name);
        }

        [Fact]
        public void Facade_Recognized_Via_Config_Root()
        {
            var code = """
            namespace MyCompany.Facades
            {
                public static class MyDx
                {
                    public static class Factories
                    {
                        public static string Create() => "";
                    }
                }
            }
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();
            mockOptions.Setup(o => o.TryGetValue("dx_facade_root", out It.Ref<string>.IsAny))
               .Returns((string key, out string value) => { value = "MyCompany.Facades.MyDx"; return true; });
            mockConfig.Setup(c => c.GlobalOptions).Returns(mockOptions.Object);

            var resolver = new DxFacadeResolver(compilation, mockConfig.Object);

            Assert.Single(resolver.FacadeFactories);
            Assert.Equal("Create", resolver.FacadeFactories.First().Name);
        }

        [Fact]
        public void Facade_Recognized_Via_Attribute()
        {
            var code = """
            namespace Dx.Domain.Annotations
            {
                public class DxFacadeAttribute : System.Attribute {}
            }

            namespace TestApp
            {
                [Dx.Domain.Annotations.DxFacade]
                public static class CustomFacade
                {
                    public static int Make() => 42;
                    internal static int Hidden() => 0;
                }
            }
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var resolver = CreateResolver(compilation);

            Assert.Single(resolver.FacadeFactories);
            var method = resolver.FacadeFactories.First();
            Assert.Equal("Make", method.Name);
        }

        [Fact]
        public void IsDxFacadeFactory_Returns_True_For_Discovered_Factories()
        {
            var code = """
            public static class Dx
            {
                public static class Result
                {
                    public static int Ok() => 0;
                }
            }
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var resolver = CreateResolver(compilation);
            var okMethod = resolver.FacadeFactories.First();

            Assert.True(resolver.IsDxFacadeFactory(okMethod));
        }

        [Fact]
        public void FindFacadeFactoryForType_Returns_Matching_Factory()
        {
            var code = """
            public static class Dx
            {
                public static class Result
                {
                    public static MyType CreateMyType() => new MyType();
                }
            }

            public class MyType {}
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var resolver = CreateResolver(compilation);
            var myType = compilation.GetTypeByMetadataName("MyType");

            var factory = resolver.FindFacadeFactoryForType(myType!);

            Assert.NotNull(factory);
            Assert.Equal("CreateMyType", factory!.Name);
        }

        private static DxFacadeResolver CreateResolver(Compilation compilation)
        {
            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();
            mockConfig.Setup(c => c.GlobalOptions).Returns(mockOptions.Object);

            return new DxFacadeResolver(compilation, mockConfig.Object);
        }
    }
}
