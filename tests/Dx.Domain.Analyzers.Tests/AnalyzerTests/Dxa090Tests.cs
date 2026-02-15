using System.Threading.Tasks;

using Dx.Domain.Analyzers.Tests.Infrastructure;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Dx.Domain.Analyzers.Tests.AnalyzerTests
{
    public sealed class Dxa090Tests
    {
        private const string ConsumerConfig = """
            is_global = true
            build_property.DxLayer = Consumer
            """;

        [Fact]
        public async Task Pragma_Disable_For_Dx_Diagnostic_Reports_Diagnostic()
        {
            var source = """
                public sealed class Service
                {
                #pragma warning disable DXA020
                    public void Test()
                    {
                    }
                #pragma warning restore DXA020
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA090, DiagnosticSeverity.Error)
                .WithSpan(3, 25, 3, 31);

            var test = new CSharpAnalyzerTest<DXA090_SuppressionDetectionAnalyzer, XunitCompatVerifier>
            {
                TestCode = source,
                TestBehaviors = TestBehaviors.SkipSuppressionCheck
            };

            test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", ConsumerConfig));
            test.TestState.ExpectedDiagnostics.Add(expected);

            await test.RunAsync();
        }

        [Fact]
        public async Task SuppressMessage_For_Dx_Diagnostic_Reports_Diagnostic()
        {
            var source = """
                using System.Diagnostics.CodeAnalysis;

                public sealed class Service
                {
                    [SuppressMessage("Usage", "DXA020")]
                    public void Test()
                    {
                    }
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA090, DiagnosticSeverity.Error)
                .WithSpan(5, 31, 5, 39);

            var test = new CSharpAnalyzerTest<DXA090_SuppressionDetectionAnalyzer, XunitCompatVerifier>
            {
                TestCode = source,
                TestBehaviors = TestBehaviors.SkipSuppressionCheck
            };

            test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", ConsumerConfig));
            test.TestState.ExpectedDiagnostics.Add(expected);

            await test.RunAsync();
        }
    }
}
