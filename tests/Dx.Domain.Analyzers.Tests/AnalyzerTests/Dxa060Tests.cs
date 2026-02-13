using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Dx.Domain.Analyzers.Tests.AnalyzerTests
{
    public sealed class Dxa060Tests
    {
        private const string ConsumerConfig = """
            is_global = true
            build_property.DxLayer = Consumer
            """;

        private const string ConsumerAllowConfig = """
            is_global = true
            build_property.DxLayer = Consumer
            dx_forbidden_vocab_allow = MyApp.Repository
            """;

        [Fact]
        public async Task Forbidden_Vocabulary_Reports_Diagnostic()
        {
            var source = """
                using Dx.Domain.Annotations;

                namespace MyApp;

                public sealed class Repository
                {
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA060, DiagnosticSeverity.Error)
                .WithSpan(3, 30, 3, 40);

            await AnalyzerTestHelper<DXA060_ForbiddenVocabularyAnalyzer>.VerifyAsync(
                source,
                ConsumerConfig,
                expected);
        }

        [Fact]
        public async Task Allow_List_Suppresses_Diagnostic()
        {
            var source = """
                using Dx.Domain.Annotations;

                namespace MyApp;

                public sealed class Repository
                {
                }
                """;

            await AnalyzerTestHelper<DXA060_ForbiddenVocabularyAnalyzer>.VerifyAsync(
                source,
                ConsumerAllowConfig);
        }
    }
}
