using Dx.Domain.Analyzers.Infrastructure.Flow;
using Dx.Domain.Analyzers.ResultFlow;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Moq;

using System.Threading;

using Xunit;

using Assert = Xunit.Assert;

namespace Dx.Domain.Analyzers.Tests.UnitTests
{
    public class ResultFlowEngineWrapperTests
    {
        [Fact]
        public void Analyze_Returns_Deterministic_Results_For_Same_Input()
        {
            var code = """
                namespace Test
                {
                    public class TestClass
                    {
                        public int SimpleMethod()
                        {
                            return 42;
                        }
                    }
                }
                """;

            var (method, compilation, options) = CreateMethodSymbol(code, "Test.TestClass", "SimpleMethod");
            var wrapper = new ResultFlowEngineWrapper();

            var result1 = wrapper.Analyze(method, compilation, options, CancellationToken.None);
            var result2 = wrapper.Analyze(method, compilation, options, CancellationToken.None);

            // Results should be the same instance (cached)
            Assert.Same(result1, result2);
        }

        [Fact]
        public void Analyze_Caches_Results_For_Same_Method()
        {
            var code = """
                namespace Test
                {
                    public class TestClass
                    {
                        public void Method1() { }
                        public void Method2() { }
                    }
                }
                """;

            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var type = compilation.GetTypeByMetadataName("Test.TestClass");
            var method1 = type!.GetMembers("Method1").First() as IMethodSymbol;
            var method2 = type!.GetMembers("Method2").First() as IMethodSymbol;

            var options = CreateMockOptions();
            var wrapper = new ResultFlowEngineWrapper();

            var result1a = wrapper.Analyze(method1!, compilation, options, CancellationToken.None);
            var result1b = wrapper.Analyze(method1!, compilation, options, CancellationToken.None);
            var result2 = wrapper.Analyze(method2!, compilation, options, CancellationToken.None);

            // Same method should return cached result
            Assert.Same(result1a, result1b);

            // Different methods should have different results
            Assert.NotSame(result1a, result2);
        }

        [Fact]
        public void Analyze_Returns_FlowGraph_For_Valid_Method()
        {
            var code = """
                namespace Test
                {
                    public class TestClass
                    {
                        public int Calculate(int x)
                        {
                            if (x > 0)
                                return x * 2;
                            return 0;
                        }
                    }
                }
                """;

            var (method, compilation, options) = CreateMethodSymbol(code, "Test.TestClass", "Calculate");
            var wrapper = new ResultFlowEngineWrapper();

            var result = wrapper.Analyze(method, compilation, options, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotNull(result.ResultNodes);
            Assert.NotNull(result.Diagnostics);
            // IsValid property is available for rules to check fail-open semantics
        }

        [Fact]
        public void Analyze_Handles_Empty_Method()
        {
            var code = """
                namespace Test
                {
                    public class TestClass
                    {
                        public void EmptyMethod() { }
                    }
                }
                """;

            var (method, compilation, options) = CreateMethodSymbol(code, "Test.TestClass", "EmptyMethod");
            var wrapper = new ResultFlowEngineWrapper();

            var result = wrapper.Analyze(method, compilation, options, CancellationToken.None);

            Assert.NotNull(result);
        }

        [Fact]
        public void Different_Method_Bodies_Produce_Different_Cache_Keys()
        {
            // Create two compilations with methods that have different bodies
            var code1 = """
                namespace Test
                {
                    public class TestClass
                    {
                        public int Method() { return 1; }
                    }
                }
                """;

            var code2 = """
                namespace Test
                {
                    public class TestClass
                    {
                        public int Method() { return 2; }
                    }
                }
                """;

            var compilation1 = CSharpCompilation.Create(
                "Test1",
                new[] { CSharpSyntaxTree.ParseText(code1) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var compilation2 = CSharpCompilation.Create(
                "Test2",
                new[] { CSharpSyntaxTree.ParseText(code2) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var type1 = compilation1.GetTypeByMetadataName("Test.TestClass");
            var method1 = type1!.GetMembers("Method").First() as IMethodSymbol;

            var type2 = compilation2.GetTypeByMetadataName("Test.TestClass");
            var method2 = type2!.GetMembers("Method").First() as IMethodSymbol;

            var options = CreateMockOptions();
            var wrapper = new ResultFlowEngineWrapper();

            var result1 = wrapper.Analyze(method1!, compilation1, options, CancellationToken.None);
            var result2 = wrapper.Analyze(method2!, compilation2, options, CancellationToken.None);

            // Different method bodies should not return the same cached result
            Assert.NotSame(result1, result2);
        }

        private static (IMethodSymbol method, Compilation compilation, AnalyzerConfigOptions options) CreateMethodSymbol(
            string code,
            string typeName,
            string methodName)
        {
            var compilation = CSharpCompilation.Create(
                "Test",
                new[] { CSharpSyntaxTree.ParseText(code) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

            var type = compilation.GetTypeByMetadataName(typeName);
            var method = type!.GetMembers(methodName).First() as IMethodSymbol;

            var options = CreateMockOptions();

            return (method!, compilation, options);
        }

        private static AnalyzerConfigOptions CreateMockOptions()
        {
            return Mock.Of<AnalyzerConfigOptions>();
        }
    }
}
