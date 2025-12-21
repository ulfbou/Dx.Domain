using Dx.Domain.Analyzers.Infrastructure.Facades;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Moq;

using System.Linq;

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

            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);

            Assert.Single(resolver.FacadeFactories);
            Assert.Equal("Ok", resolver.FacadeFactories.First().Name);
        }

        [Fact]
        public void Internal_Methods_Are_Not_Discovered()
        {
            var code = """
        public static class Dx
        {
            public static class Result
            {
                internal static int InternalMethod() => 0;
            }
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);

            Assert.Empty(resolver.FacadeFactories);
        }

        [Fact]
        public void Private_Methods_Are_Not_Discovered()
        {
            var code = """
        public static class Dx
        {
            public static class Result
            {
                private static int PrivateMethod() => 0;
            }
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);

            Assert.Empty(resolver.FacadeFactories);
        }

        [Fact]
        public void Instance_Methods_Are_Not_Discovered()
        {
            var code = """
        public static class Dx
        {
            public static class Result
            {
                public int InstanceMethod() => 0;
            }
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);

            Assert.Empty(resolver.FacadeFactories);
        }

        [Fact]
        public void Multiple_Facade_Methods_Are_All_Discovered()
        {
            var code = """
        public static class Dx
        {
            public static class Result
            {
                public static int Ok() => 0;
                public static int Error() => 1;
            }

            public static class Option
            {
                public static int Some() => 2;
                public static int None() => 3;
            }
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);

            Assert.Equal(4, resolver.FacadeFactories.Count);
            Assert.Contains(resolver.FacadeFactories, m => m.Name == "Ok");
            Assert.Contains(resolver.FacadeFactories, m => m.Name == "Error");
            Assert.Contains(resolver.FacadeFactories, m => m.Name == "Some");
            Assert.Contains(resolver.FacadeFactories, m => m.Name == "None");
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

            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);
            var okMethod = resolver.FacadeFactories.First();

            Assert.True(resolver.IsDxFacadeFactory(okMethod));
        }

        [Fact]
        public void IsDxFacadeFactory_Returns_False_For_Undiscovered_Methods()
        {
            var code = """
        public static class Dx
        {
            public static class Result
            {
                public static int Ok() => 0;
            }
        }

        public static class Other
        {
            public static int NotAFacade() => 1;
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);

            var otherType = compilation.GetTypeByMetadataName("Other");
            var notFacadeMethod = otherType!.GetMembers("NotAFacade").First() as IMethodSymbol;

            Assert.False(resolver.IsDxFacadeFactory(notFacadeMethod!));
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

            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);
            var myType = compilation.GetTypeByMetadataName("MyType");

            var factory = resolver.FindFacadeFactoryForType(myType!);

            Assert.NotNull(factory);
            Assert.Equal("CreateMyType", factory!.Name);
        }

        [Fact]
        public void FindFacadeFactoryForType_Returns_Null_For_Non_Facade_Type()
        {
            var code = """
        public static class Dx
        {
            public static class Result
            {
                public static int Ok() => 0;
            }
        }

        public class OtherType {}
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);
            var otherType = compilation.GetTypeByMetadataName("OtherType");

            var factory = resolver.FindFacadeFactoryForType(otherType!);

            Assert.Null(factory);
        }

        [Fact]
        public void Resolver_Handles_Missing_Dx_Type_Gracefully()
        {
            var code = """
        namespace MyApp
        {
            public class Foo {}
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            // Should not throw when Dx type is not present
            var config = Mock.Of<AnalyzerConfigOptionsProvider>();
            var resolver = new DxFacadeResolver(compilation, config);

            Assert.Empty(resolver.FacadeFactories);
        }
    }
}
