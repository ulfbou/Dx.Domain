using System.IO;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.UnitTests
{
    public sealed class GovernanceTargetsTests
    {
        private const string GovernanceTargetsFile = "Dx.DomainAnalyzerGovernance.targets";

        [Fact]
        public void Governance_Targets_File_Is_Present()
        {
            var path = GetGovernancePath();
            Assert.True(File.Exists(path));
        }

        [Fact]
        public void Governance_Targets_Block_Analyzer_Disablement()
        {
            var path = GetGovernancePath();
            var content = File.ReadAllText(path);

            Assert.Contains("RunAnalyzers", content);
            Assert.Contains("EnableNETAnalyzers", content);
            Assert.Contains("dotnet_diagnostic", content);
            Assert.Contains("WarningsAsErrors", content);
        }

        private static string GetGovernancePath()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                var candidate = Path.Combine(current.FullName, "builds", "policy", GovernanceTargetsFile);
                if (File.Exists(candidate))
                    return candidate;

                current = current.Parent;
            }

            return Path.Combine("builds", "policy", GovernanceTargetsFile);
        }
    }
}
