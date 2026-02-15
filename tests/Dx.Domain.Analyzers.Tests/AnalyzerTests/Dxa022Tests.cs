using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Dx.Domain.Analyzers.Tests.AnalyzerTests
{
    public sealed class Dxa022Tests
    {
        private const string ConsumerConfig = """
            is_global = true
            build_property.DxLayer = Consumer
            dx_exception_domain_types = OrderFailureException
            """;

        private const string KernelConfig = """
            is_global = true
            build_property.DxLayer = Kernel
            dx_exception_domain_types = OrderFailureException
            """;

        [Fact]
        public async Task Kernel_Result_Method_Throw_Is_Allowed()
        {
            var source = """
                using System;
                using Dx.Domain;
                using Dx.Domain.Annotations;

                public sealed class OrderFailureException : Exception { }

                public sealed class Service
                {
                    public Result<Unit> DoWork()
                    {
                        throw new OrderFailureException();
                    }
                }
                """;

            await AnalyzerTestHelper<DXA022_DomainControlExceptionAnalyzer>.VerifyAsync(
                source,
                KernelConfig);
        }

        [Fact]
        public async Task Consumer_Result_Method_Throw_Reports_Diagnostic()
        {
            var source = """
                using System;
                using Dx.Domain;
                using Dx.Domain.Annotations;

                public sealed class OrderFailureException : Exception { }

                public sealed class Service
                {
                    public Result<Unit> DoWork()
                    {
                        throw new OrderFailureException();
                    }
                }
                """;

            var expected = new DiagnosticResult(DxRuleIds.DXA022, DiagnosticSeverity.Warning)
                .WithSpan(11, 9, 11, 41);

            await AnalyzerTestHelper<DXA022_DomainControlExceptionAnalyzer>.VerifyAsync(
                source,
                ConsumerConfig,
                expected);
        }
    }
}
