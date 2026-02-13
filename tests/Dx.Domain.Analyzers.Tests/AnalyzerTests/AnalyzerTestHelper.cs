using System.Threading.Tasks;

using Dx.Domain.Errors;

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Tests.AnalyzerTests
{
    internal static class AnalyzerTestHelper<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public static async Task VerifyAsync(
            string source,
            string editorConfig,
            params DiagnosticResult[] expected)
        {
            var test = new CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
            {
                TestCode = source
            };

            test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
            test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(DomainError).Assembly.Location));
            test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(AggregateRootAttribute).Assembly.Location));
            test.ExpectedDiagnostics.AddRange(expected);

            await test.RunAsync();
        }
    }
}
