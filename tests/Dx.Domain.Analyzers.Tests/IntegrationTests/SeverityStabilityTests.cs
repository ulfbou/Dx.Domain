using Dx.Domain.Analyzers.Analyzers;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.IntegrationTests
{
    /// <summary>
    /// AC10: Diagnostic severity stability
    /// </summary>
    public class SeverityStabilityTests
    {
        [Fact]
        public void DXA010_Is_Warning()
        {
            var analyzer = new DXA010_ConstructionAuthorityAnalyzer();
            var descriptor = analyzer.SupportedDiagnostics[0];
            
            Assert.Equal("DXA010", descriptor.Id);
            Assert.Equal(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning, descriptor.DefaultSeverity);
            Assert.True(descriptor.IsEnabledByDefault);
        }

        [Fact]
        public void DXA020_Is_Error()
        {
            var analyzer = new DXA020_ResultIgnoredAnalyzer();
            var descriptor = analyzer.SupportedDiagnostics[0];
            
            Assert.Equal("DXA020", descriptor.Id);
            Assert.Equal(Microsoft.CodeAnalysis.DiagnosticSeverity.Error, descriptor.DefaultSeverity);
            Assert.True(descriptor.IsEnabledByDefault);
        }
    }
}
