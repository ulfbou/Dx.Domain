using Dx.Domain.Analyzers.Infrastructure.Facades;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

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
                public struct Unit {}
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

        [Fact]
        public void Non_Generic_Result_Is_Detected_As_Kernel_Result()
        {
            var code = """
            namespace Dx.Domain
            {
                public class Result {}
            }
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var result = compilation.GetTypeByMetadataName("Dx.Domain.Result");
            Assert.True(classifier.IsKernelResultType(result!));
        }

        [Fact]
        public void Generic_Result_With_One_Type_Parameter_Is_Kernel_Result()
        {
            var code = """
            namespace Dx.Domain
            {
                public class Result<T> {}
            }
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var result = compilation.GetTypeByMetadataName("Dx.Domain.Result`1");
            Assert.True(classifier.IsKernelResultType(result!));
        }

        [Fact]
        public void Generic_Result_With_Two_Type_Parameters_Is_Kernel_Result()
        {
            var code = """
            namespace Dx.Domain
            {
                public class Result<T,E> {}
            }
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var result = compilation.GetTypeByMetadataName("Dx.Domain.Result`2");
            Assert.True(classifier.IsKernelResultType(result!));
        }

        [Fact]
        public void Unit_Is_Detected_As_Kernel_Result_Type()
        {
            var code = """
            namespace Dx.Domain
            {
                public struct Unit {}
            }
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var unit = compilation.GetTypeByMetadataName("Dx.Domain.Unit");
            Assert.True(classifier.IsKernelResultType(unit!));
        }

        [Fact]
        public void DomainError_Type_Is_Detected()
        {
            var code = """
            namespace Dx.Domain
            {
                public class DomainError {}
            }
            """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var domainError = compilation.GetTypeByMetadataName("Dx.Domain.DomainError");
            Assert.True(classifier.IsDomainErrorType(domainError!));
        }
    }
}
