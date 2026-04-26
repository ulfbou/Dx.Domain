using Dx.Domain.Analyzers.Analyzers;
using Dx.Domain.Analyzers.Infrastructure.Facades;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

using System.Threading.Tasks;

using Xunit;

using System.Linq;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.IntegrationTests
{
    public class ConstructionPathTests
    {
        [Fact]
        public async Task Type_With_Internal_Ctor_And_Facade_No_Diagnostics()
        {
            const string code = """
                namespace Dx.Domain.Annotations
                {
                    public class DxFacadeAttribute : System.Attribute {}
                }

                namespace TestApp.Domain
                {
                    public class DomainType
                    {
                        internal DomainType() {}
                    }

                    [Dx.Domain.Annotations.DxFacade]
                    public static class DomainFactory
                    {
                        public static DomainType Create() => new DomainType();
                    }
                }
                """;
            var analyzerAsm = typeof(DxFacadeResolver).Assembly.Location;

            var test010 = new CSharpAnalyzerTest<DXA010_ConstructionAuthorityAnalyzer, DefaultVerifier>
            { TestCode = code };
            test010.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(analyzerAsm));
            await test010.RunAsync();

            var test011 = new CSharpAnalyzerTest<DXA011_PublicFactoryExposureAnalyzer, DefaultVerifier>
            { TestCode = code };
            test011.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(analyzerAsm));
            await test011.RunAsync();
        }

        [Fact]
        public async Task Type_With_No_Path_Triggers_DXA011_Only()
        {
            const string code = """
                namespace TestApp.Domain
                {
                    public class OrphanType
                    {
                        internal OrphanType() {}
                    }
                }
                """;
            var analyzerAsm = typeof(DxFacadeResolver).Assembly.Location;

            var test011 = new CSharpAnalyzerTest<DXA011_PublicFactoryExposureAnalyzer, DefaultVerifier>
            { TestCode = code };
            test011.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(analyzerAsm));
            test011.TestState.AnalyzerConfigFiles.Add(
                ("/.editorconfig", """
                    is_global = true
                    dx.scope.map = TestProject=S1
                    """));

            test011.ExpectedDiagnostics.Add(
                DiagnosticResult.CompilerWarning("DXA011").WithSpan(3, 18, 3, 28));

            await test011.RunAsync();

            var test010 = new CSharpAnalyzerTest<DXA010_ConstructionAuthorityAnalyzer, DefaultVerifier>
            { TestCode = code };
            test010.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(analyzerAsm));

            await test010.RunAsync();
        }
    }
}
