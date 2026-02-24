using System.Threading.Tasks;

using Dx.Domain.Analyzers.Tests.Infrastructure;
using Dx.Domain.Errors;

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Tests.AnalyzerTests
{
    internal static class AnalyzerTestHelper<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        private const string KernelStubs = """
            namespace Dx.Domain
            {
                public readonly struct DomainError { }

                public readonly struct Unit
                {
                    public static Unit Value => default;
                }

                public sealed class Result
                {
                    public static Result Success() => new();
                    public static Result<TValue> Success<TValue>(TValue value) where TValue : notnull => default;
                    public static Result<TValue> Failure<TValue>(DomainError error) where TValue : notnull => default;
                }

                public readonly struct Result<TValue> where TValue : notnull { }
                public readonly struct Result<TValue, TError> where TValue : notnull where TError : notnull { }
            }

            namespace Dx.Domain.Errors
            {
                public readonly struct DomainError { }
            }
            """;

        public static async Task VerifyAsync(
            string source,
            string editorConfig,
            params DiagnosticResult[] expected)
            => await VerifyAsync(source, editorConfig, includeKernelReferences: false, expected);

        public static async Task VerifyAsync(
            string source,
            string editorConfig,
            bool includeKernelReferences,
            params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<TAnalyzer, XunitCompatVerifier>
            {
                TestCode = source
            };

            var normalizedConfig = EnsureTestProjectFlag(editorConfig);
            test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", normalizedConfig));
            test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(AggregateRootAttribute).Assembly.Location));

            if (!includeKernelReferences)
                test.TestState.Sources.Add(("KernelStubs.cs", KernelStubs));

            if (includeKernelReferences)
                test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(DomainError).Assembly.Location));
            test.ExpectedDiagnostics.AddRange(expected);

            await test.RunAsync();
        }

        private static string EnsureTestProjectFlag(string editorConfig)
        {
            if (editorConfig.Contains("build_property.IsTestProject", StringComparison.OrdinalIgnoreCase))
                return editorConfig;

            return $"{editorConfig}\n            build_property.IsTestProject = false\n            ";
        }
    }
}
