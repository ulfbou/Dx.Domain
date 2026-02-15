using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Dx.Domain.Analyzers.Tests.AnalyzerTests
{
    public sealed class Dxa020Tests
    {
        private const string ConsumerConfig = """
            is_global = true
            build_property.DxLayer = Consumer
            """;

        [Fact]
        public async Task Discard_Assignment_Reports_Diagnostic()
        {
            var source = """
                using Dx.Domain;
                using Dx.Domain.Annotations;

                public sealed class Service
                {
                    public Result<Unit> DoWork() => default;

                    public void Test()
                    {
                        _ = DoWork();
                    }
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA020, DiagnosticSeverity.Error)
                .WithSpan(10, 9, 10, 21);

            await AnalyzerTestHelper<DXA020_ResultIgnoredAnalyzer>.VerifyAsync(
                source,
                ConsumerConfig,
                expected);
        }

        [Fact]
        public async Task Discard_Local_Assignment_Reports_Diagnostic()
        {
            var source = """
                using Dx.Domain;
                using Dx.Domain.Annotations;

                public sealed class Service
                {
                    public Result<Unit> DoWork() => default;

                    public void Test()
                    {
                        var _ = DoWork();
                    }
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA020, DiagnosticSeverity.Error)
                .WithSpan(10, 9, 10, 25);

            await AnalyzerTestHelper<DXA020_ResultIgnoredAnalyzer>.VerifyAsync(
                source,
                ConsumerConfig,
                expected);
        }

        [Fact]
        public async Task Expression_Statement_Reports_Diagnostic()
        {
            var source = """
                using Dx.Domain;
                using Dx.Domain.Annotations;

                public sealed class Service
                {
                    public Result<Unit> DoWork() => default;

                    public void Test()
                    {
                        DoWork();
                    }
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA020, DiagnosticSeverity.Error)
                .WithSpan(10, 9, 10, 17);

            await AnalyzerTestHelper<DXA020_ResultIgnoredAnalyzer>.VerifyAsync(
                source,
                ConsumerConfig,
                expected);
        }

        [Fact]
        public async Task Local_Function_Ignore_Reports_Diagnostic()
        {
            var source = """
                using Dx.Domain;
                using Dx.Domain.Annotations;

                public sealed class Service
                {
                    public Result<Unit> DoWork() => default;

                    public void Test()
                    {
                        Result Local() => DoWork();

                        Local();
                    }
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA020, DiagnosticSeverity.Error)
                .WithSpan(12, 9, 12, 15);

            await AnalyzerTestHelper<DXA020_ResultIgnoredAnalyzer>.VerifyAsync(
                source,
                ConsumerConfig,
                expected);
        }

        [Fact]
        public async Task Expression_Bodied_Method_Reports_Diagnostic()
        {
            var source = """
                using Dx.Domain;
                using Dx.Domain.Annotations;

                public sealed class Service
                {
                    public Result<Unit> DoWork() => default;

                    public void Test() => DoWork();
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA020, DiagnosticSeverity.Error)
                .WithSpan(8, 27, 8, 35);

            await AnalyzerTestHelper<DXA020_ResultIgnoredAnalyzer>.VerifyAsync(
                source,
                ConsumerConfig,
                expected);
        }
    }
}
