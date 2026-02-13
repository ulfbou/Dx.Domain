using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Dx.Domain.Analyzers.Tests.AnalyzerTests
{
    public sealed class Dxa010Tests
    {
        private const string ConsumerConfig = """
            is_global = true
            build_property.DxLayer = Consumer
            """;

        private const string KernelConfig = """
            is_global = true
            build_property.DxLayer = Kernel
            """;

        [Fact]
        public async Task Kernel_Allows_Direct_Construction()
        {
            var source = """
                using Dx.Domain;
                using Dx.Domain.Annotations;

                [AggregateRoot]
                public sealed class Order
                {
                    public Order() { }
                }

                public sealed class Consumer
                {
                    public void Create()
                    {
                        var order = new Order();
                    }
                }
                """;

            await AnalyzerTestHelper<DXA010_ConstructionAuthorityAnalyzer>.VerifyAsync(
                source,
                KernelConfig);
        }

        [Fact]
        public async Task Consumer_Direct_Construction_Reports_Diagnostic()
        {
            var source = """
                using Dx.Domain;
                using Dx.Domain.Annotations;

                [AggregateRoot]
                public sealed class Order
                {
                    public Order() { }
                }

                public sealed class Consumer
                {
                    public void Create()
                    {
                        var order = new Order();
                    }
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA010, DiagnosticSeverity.Warning)
                .WithSpan(14, 21, 14, 32);

            await AnalyzerTestHelper<DXA010_ConstructionAuthorityAnalyzer>.VerifyAsync(
                source,
                ConsumerConfig,
                expected);
        }
    }
}
