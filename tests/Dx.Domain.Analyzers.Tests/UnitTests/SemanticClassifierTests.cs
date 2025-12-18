using Dx.Domain.Analyzers.Infrastructure.Semantics;

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
            // Non-generic Result IS in the list for kernel result types
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
            public class Result<T, E> {}
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

        [Fact]
        public void Non_Domain_Error_Type_Returns_False()
        {
            var code = """
        namespace MyApp
        {
            public class MyError {}
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var myError = compilation.GetTypeByMetadataName("MyApp.MyError");
            Assert.False(classifier.IsDomainErrorType(myError!));
        }

        [Fact]
        public void InvariantViolationException_Is_Detected()
        {
            var code = """
        namespace Dx.Domain
        {
            public class InvariantViolationException : System.Exception {}
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var exception = compilation.GetTypeByMetadataName("Dx.Domain.InvariantViolationException");
            Assert.True(classifier.IsInvariantException(exception!));
        }

        [Fact]
        public void Derived_InvariantViolationException_Is_Detected()
        {
            var code = """
        namespace Dx.Domain
        {
            public class InvariantViolationException : System.Exception {}
            public class SpecificInvariantException : InvariantViolationException {}
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var exception = compilation.GetTypeByMetadataName("Dx.Domain.SpecificInvariantException");
            Assert.True(classifier.IsInvariantException(exception!));
        }

        [Fact]
        public void Non_Invariant_Exception_Returns_False()
        {
            var code = """
        namespace MyApp
        {
            public class MyException : System.Exception {}
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var exception = compilation.GetTypeByMetadataName("MyApp.MyException");
            Assert.False(classifier.IsInvariantException(exception!));
        }

        [Fact]
        public void Consumer_Types_Are_Not_Misclassified_As_Domain_Types()
        {
            var code = """
        namespace MyApp.Models
        {
            public class UserResult {}
            public class ValidationError {}
        }
        """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var classifier = new SemanticClassifier(compilation);

            var userResult = compilation.GetTypeByMetadataName("MyApp.Models.UserResult");
            var validationError = compilation.GetTypeByMetadataName("MyApp.Models.ValidationError");

            Assert.False(classifier.IsKernelResultType(userResult!));
            Assert.False(classifier.IsDomainErrorType(validationError!));
        }

        [Fact]
        public void Classifier_Handles_Missing_Types_Gracefully()
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

            // Classifier should not throw when Dx.Domain types are not present
            var classifier = new SemanticClassifier(compilation);

            var foo = compilation.GetTypeByMetadataName("MyApp.Foo");
            Assert.False(classifier.IsKernelResultType(foo!));
            Assert.False(classifier.IsDomainErrorType(foo!));
            Assert.False(classifier.IsInvariantException(foo!));
        }
    }
}
