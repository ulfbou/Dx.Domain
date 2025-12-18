using System.Threading.Tasks;

using Dx.Domain.Analyzers.Tests.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.IntegrationTests
{
    public class EditorConfigRoundTripTests
    {
        [Fact]
        public async Task Kernel_With_Minimal_Config_Produces_No_Diagnostics()
        {
            var test = new CSharpAnalyzerTest<MyAnalyzer, XUnitVerifier>
            {
                TestCode = "namespace Dx.Domain { public class Foo {} }",
            };

            await test.RunAsync();
        }
    }
}
