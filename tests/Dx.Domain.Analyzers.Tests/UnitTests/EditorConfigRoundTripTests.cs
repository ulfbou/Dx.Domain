using System.Threading.Tasks;

using Dx.Domain.Analyzers.Tests.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
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

        [Fact]
        public async Task Kernel_With_Dx_Scope_Map_Produces_No_Diagnostics()
        {
            var test = new CSharpAnalyzerTest<MyAnalyzer, XUnitVerifier>
            {
                TestCode = "namespace Dx.Domain { public class Foo {} }",
            };

            test.TestState.AnalyzerConfigFiles.Add(
                ("/.editorconfig", """
                root = true

                [*.cs]
                dx.scope.map = Dx.Domain=S0
                """)
            );

            await test.RunAsync();
        }

        [Fact]
        public async Task Application_Code_With_Root_Namespaces_Produces_No_Diagnostics()
        {
            var test = new CSharpAnalyzerTest<MyAnalyzer, XUnitVerifier>
            {
                TestCode = "namespace MyApp.Domain { public class Foo {} }",
            };

            test.TestState.AnalyzerConfigFiles.Add(
                ("/.editorconfig", """
                root = true

                [*.cs]
                dx.scope.rootNamespaces = MyApp.Domain;MyApp.Shared
                """)
            );

            await test.RunAsync();
        }

        [Fact]
        public async Task Full_Config_With_S0_And_S3_Produces_No_Diagnostics()
        {
            var test = new CSharpAnalyzerTest<MyAnalyzer, XUnitVerifier>
            {
                TestCode = """
                namespace Dx.Domain 
                { 
                    public class KernelClass {} 
                }

                namespace MyApp.Domain 
                { 
                    public class AppClass {} 
                }
                """,
            };

            test.TestState.AnalyzerConfigFiles.Add(
                ("/.editorconfig", """
                root = true

                [*.cs]
                dx.scope.map = Dx.Domain=S0;MyApp.Domain=S3
                dx.scope.rootNamespaces = MyApp.Domain
                """)
            );

            await test.RunAsync();
        }

        [Fact]
        public async Task Multiple_Scope_Mappings_Produce_No_Diagnostics()
        {
            var test = new CSharpAnalyzerTest<MyAnalyzer, XUnitVerifier>
            {
                TestCode = """
                namespace Dx.Domain { public class KernelClass {} }
                namespace Dx.Shared { public class SharedClass {} }
                namespace MyApp { public class AppClass {} }
                """,
            };

            test.TestState.AnalyzerConfigFiles.Add(
                ("/.editorconfig", """
                root = true

                [*.cs]
                dx.scope.map = Dx.Domain=S0;Dx.Shared=S1;MyApp=S2
                """)
            );

            await test.RunAsync();
        }
    }
}
