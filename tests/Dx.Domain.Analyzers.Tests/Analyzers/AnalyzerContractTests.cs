using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Dx.Domain.Analyzers.Analyzers;
using Dx.Domain.Annotations;

using Microsoft.CodeAnalysis.Diagnostics;

using Xunit;

namespace Dx.Domain.Analyzers.Tests;

public sealed class AnalyzerContractTests
{
    private static readonly string[] ExpectedAlphaDiagnosticIds =
    {
        "DXA010",
        "DXA011",
        "DXA020",
        "DXA022",
        "DXA030",
        "DXA040",
        "DXA050",
        "DXA060",
        "DXA065",
        "DXA070",
        "DXA080"
    };

    [Fact]
    public void ShippedAlphaRuleCatalog_DoesNotExpose_DXA090()
    {
        var catalogIds = typeof(DxRuleIds)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.IsLiteral && !field.IsInitOnly)
            .Where(field => field.FieldType == typeof(string))
            .Select(field => field.GetRawConstantValue() as string)
            .Where(value => value is not null && value.StartsWith("DXA", StringComparison.Ordinal))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray()!;

        var analyzerIds = GetShippedAnalyzers()
            .SelectMany(analyzer => analyzer.SupportedDiagnostics)
            .Select(descriptor => descriptor.Id)
            .Where(id => id.StartsWith("DXA", StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(ExpectedAlphaDiagnosticIds, catalogIds);
        Assert.Equal(ExpectedAlphaDiagnosticIds, analyzerIds);
        Assert.DoesNotContain("DXA090", catalogIds);
        Assert.DoesNotContain("DXA090", analyzerIds);
    }

    [Fact]
    public void Governance_Blocks_DxSeverity_Overrides()
    {
        var governanceTargetsPath = Path.Combine(FindRepositoryRoot(), "builds", "policy", "Dx.DomainAnalyzerGovernance.targets");
        var targets = XDocument.Load(governanceTargetsPath);
        var root = targets.Root ?? throw new InvalidOperationException("Governance target file must have a root element.");

        var dxSeverityBlock = root.Descendants("Error")
            .Single(element => string.Equals((string?)element.Attribute("Code"), "DXB003", StringComparison.Ordinal));
        var condition = (string?)dxSeverityBlock.Attribute("Condition") ?? string.Empty;
        var text = (string?)dxSeverityBlock.Attribute("Text") ?? string.Empty;
        var warningsAsErrors = root.Descendants("WarningsAsErrors").Select(element => element.Value).SingleOrDefault() ?? string.Empty;

        Assert.Contains("dotnet_diagnostic.DXA", condition, StringComparison.Ordinal);
        Assert.Contains("dotnet_diagnostic.DXK", condition, StringComparison.Ordinal);
        Assert.Contains("dotnet_diagnostic.DXT", condition, StringComparison.Ordinal);
        Assert.Contains("dotnet_diagnostic.DX100", condition, StringComparison.Ordinal);
        Assert.Contains("dotnet_diagnostic.DX700", condition, StringComparison.Ordinal);
        Assert.Contains(".editorconfig cannot override Dx diagnostics", text, StringComparison.Ordinal);
        Assert.Contains("DXA*", warningsAsErrors, StringComparison.Ordinal);
        Assert.Contains("DXK*", warningsAsErrors, StringComparison.Ordinal);
        Assert.Contains("DXT*", warningsAsErrors, StringComparison.Ordinal);
    }

    [Fact]
    public void FactsPackage_Is_Classified_As_FactsAssembly()
    {
        var projectPath = Path.Combine(FindRepositoryRoot(), "src", "Dx.Domain.Facts", "Dx.Domain.Facts.csproj");
        var project = XDocument.Load(projectPath);
        var root = project.Root ?? throw new InvalidOperationException("Facts project file must have a root element.");

        var dxLayer = root.Descendants("DxLayer").Select(element => element.Value).SingleOrDefault();
        var compilerVisibleDxLayer = root.Descendants("CompilerVisibleProperty")
            .Any(element => string.Equals((string?)element.Attribute("Include"), "DxLayer", StringComparison.Ordinal));
        var embeddedAnalyzer = root.Descendants("None")
            .Any(element =>
                string.Equals((string?)element.Attribute("Pack"), "true", StringComparison.OrdinalIgnoreCase)
                && string.Equals((string?)element.Attribute("PackagePath"), "analyzers/dotnet/cs", StringComparison.Ordinal)
                && string.Equals(
                    (string?)element.Attribute("Include"),
                    @"..\Dx.Domain.Analyzers\bin\$(Configuration)\netstandard2.0\Dx.Domain.Analyzers.dll",
                    StringComparison.Ordinal));

        Assert.Equal("Authority", dxLayer);
        Assert.True(compilerVisibleDxLayer);
        Assert.True(embeddedAnalyzer);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var solutionPath = Path.Combine(directory.FullName, "Dx.Domain.sln");
            if (File.Exists(solutionPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root containing Dx.Domain.sln.");
    }

    private static IEnumerable<DiagnosticAnalyzer> GetShippedAnalyzers()
    {
        yield return new DXA010_ConstructionAuthorityAnalyzer();
        yield return new DXA011_PublicFactoryExposureAnalyzer();
        yield return new DXA020_ResultIgnoredAnalyzer();
        yield return new DXA022_DomainControlExceptionAnalyzer();
        yield return new DXA030_UnapprovedHandlerAnalyzer();
        yield return new DXA040_KernelPublicSurfaceFreezeAnalyzer();
        yield return new DXA050_TemporalHelperUsageAnalyzer();
        yield return new DXA060_ForbiddenVocabularyAnalyzer();
        yield return new DXA065_UnresolvedXmlDocReferenceAnalyzer();
        yield return new DXA070_GeneratedCodeTaggingAnalyzer();
        yield return new DXA080_FacadeInvariantEnforcementAnalyzer();
    }

}
