using Dx.Domain.Analyzers.Analyzers;
using Dx.Domain.Analyzers.Infrastructure.Generated;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

using Moq;

using System.Threading.Tasks;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.UnitTests
{
    public class GeneratedCodeDetectorTests
    {
        [Fact]
        public async Task Analyzer_Ignores_Generated_File_With_Direct_Construction()
        {
            var code = """
                using System.CodeDom.Compiler;

                namespace TestApp
                {
                    [GeneratedCode("tool", "1.0")]
                    public class Generated
                    {
                        public void Test()
                        {
                            var id = new CustomerId();
                        }
                    }

                    public class CustomerId {}
                }
                """;

            var test = new CSharpAnalyzerTest<DXA010_ConstructionAuthorityAnalyzer, DefaultVerifier>
            {
                TestCode = code
            };

            await test.RunAsync();
        }
        [Fact]
        public void Symbol_With_GeneratedCodeAttribute_IsGenerated_Returns_True()
        {
            var code = """
                using System.CodeDom.Compiler;

                [GeneratedCode("Tool", "1.0")]
                public class GeneratedClass
                {
                }
                """;

            var symbol = GetTypeSymbol(code, "GeneratedClass");
            var detector = CreateDetector(null);

            Assert.True(detector.IsGenerated(symbol));
        }

        [Fact]
        public void Symbol_With_CompilerGeneratedAttribute_IsGenerated_Returns_True()
        {
            var code = """
                using System.Runtime.CompilerServices;

                [CompilerGenerated]
                public class CompilerGeneratedClass
                {
                }
                """;

            var symbol = GetTypeSymbol(code, "CompilerGeneratedClass");
            var detector = CreateDetector(null);

            Assert.True(detector.IsGenerated(symbol));
        }

        [Fact]
        public void Symbol_In_Configured_Namespace_Marker_IsGenerated_Returns_True()
        {
            var code = """
                namespace GeneratedCode.Models
                {
                    public class Model
                    {
                    }
                }
                """;

            var symbol = GetTypeSymbol(code, "GeneratedCode.Models.Model");
            var detector = CreateDetector("GeneratedCode");

            Assert.True(detector.IsGenerated(symbol));
        }

        private static INamedTypeSymbol GetTypeSymbol(string code, string typeName)
        {
            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            return compilation.GetTypeByMetadataName(typeName)!;
        }

        private static GeneratedCodeDetector CreateDetector(string? markers)
        {
            var mockConfig = new Moq.Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Moq.Mock<AnalyzerConfigOptions>();
            if (markers != null)
            {
                mockOptions.Setup(o => o.TryGetValue("dx_generated_markers", out It.Ref<string>.IsAny))
                    .Returns((string k, out string v) => { v = markers; return true; });
            }
            mockConfig.Setup(c => c.GlobalOptions).Returns(mockOptions.Object);
            return new GeneratedCodeDetector(mockConfig.Object);
        }
    }
}
