using Dx.Domain.Analyzers.Infrastructure.Facades;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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

            var resolver = new DxFacadeResolver(compilation);

            Assert.Single(resolver.FacadeFactories);
            Assert.Equal("Ok", resolver.FacadeFactories.First().Name);
        }
    }
}
