using Dx.Domain.Analyzers.Analyzers;

using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

using System.Threading.Tasks;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.IntegrationTests
{
    public class SampleDemo1Tests
    {
        [Fact]
        public async Task SampleDemo1_Line13_ResultSuccess_No_DXA010_But_DXA020()
        {
            const string code = """
                using Dx.Domain;

                namespace Dx.Domain
                {
                    public class Result
                    {
                        public static Result Success() => new Result();
                    }
                }

                namespace SampleDemo1
                {
                    public class Demo
                    {
                        public void Test()
                        {
                            Result.Success();
                        }
                    }
                }
                """;

            var test = new CSharpAnalyzerTest<DXA020_ResultIgnoredAnalyzer, DefaultVerifier>
            {
                TestCode = code,
                ReferenceAssemblies = ReferenceAssemblies.Default,
            };
            test.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerError("DXA020").WithSpan(17, 13, 17, 29));

            await test.RunAsync();

            var test2 = new CSharpAnalyzerTest<DXA010_ConstructionAuthorityAnalyzer, DefaultVerifier>
            {
                TestCode = code,
                ReferenceAssemblies = ReferenceAssemblies.Default
            };

            await test2.RunAsync(); // should have 0 diagnostics
        }

        [Fact]
        public async Task SampleDemo1_Line16_NewCustomerId_Triggers_DXA010()
        {
            const string code = """
                namespace SampleDemo1.Domain
                {
                    public class CustomerId {}

                    public class Demo
                    {
                        public void Test()
                        {
                            var id = new CustomerId();
                        }
                    }
                }
                """;

            var test = new CSharpAnalyzerTest<DXA010_ConstructionAuthorityAnalyzer, DefaultVerifier>
            {
                TestCode = code,
                ReferenceAssemblies = ReferenceAssemblies.Default,
            };

            test.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerWarning("DXA010").WithSpan(9, 22, 9, 38));

            await test.RunAsync();
        }
    }
}
