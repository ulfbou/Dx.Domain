using System.IO;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.UnitTests
{
    public sealed class GovernanceTargetsTests
    {
        [Fact]
        public void Governance_Targets_File_Is_Present()
        {
            var path = Path.Combine("..", "..", "..", "..", "builds", "policy", "Dx.DomainAnalyzerGovernance.targets");
            Assert.True(File.Exists(path));
        }

        [Fact]
        public void Governance_Targets_Block_Analyzer_Disablement()
        {
            var path = Path.Combine("..", "..", "..", "..", "builds", "policy", "Dx.DomainAnalyzerGovernance.targets");
            var content = File.ReadAllText(path);

            Assert.Contains("RunAnalyzers", content);
            Assert.Contains("EnableNETAnalyzers", content);
            Assert.Contains("dotnet_diagnostic", content);
            Assert.Contains("WarningsAsErrors", content);
        }
    }
}
