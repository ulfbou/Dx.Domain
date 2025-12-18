using Dx.Domain.Analyzers.Infrastructure.Semantics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Assert = Xunit.Assert;

namespace Dx.Domain.Analyzers.Tests.UnitTests
{
    public class SemanticClassifierTests
    {
        [Fact]
        public void Detects_All_Result_Shapes()
        {
            var code = """
        namespace Dx.Domain
        {
            public class Result {}
            public class Result<T> {}
            public class Result<T,E> {}
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var result1 = compilation.GetTypeByMetadataName("Dx.Domain.Result`1");
            Assert.True(classifier.IsKernelResultType(result1!));
        }
    }
}
