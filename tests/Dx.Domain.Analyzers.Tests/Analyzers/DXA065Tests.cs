// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA065Tests.cs" company="Dx.Domain Team">
// Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
// This software is licensed under the MIT License.
// See the project's root <c>LICENSE</c> file for details.
// Contributions are welcome, subject to the terms of the project's license.
// See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;

using Dx.Domain.Analyzers.Analyzers;
using Dx.Domain.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

using Xunit;

namespace Dx.Domain.Analyzers.Tests;

public class DXA065Tests
{
    [Fact]
    public async Task PlainTypeName_InSummary_TriggersDiagnostic()
    {
        const string code = @"
using Dx.Domain.Annotations;
[assembly: DxLayer(""Kernel"")]
namespace Dx.Domain.Primitives;
/// <summary>Creates a new UserId from a Guid.</summary>
public readonly struct UserId
{
    /// <summary>Returns a Result indicating success.</summary>
    public static UserId New() => default;
}";

        var test = new CSharpAnalyzerTest<DXA065_UnresolvedXmlDocReferenceAnalyzer, DefaultVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
        };
        test.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(DxLayerAttribute).Assembly.Location));

        // FIXED: now expecting the exact location the analyzer reports
        test.ExpectedDiagnostics.Add(
            DiagnosticResult.CompilerWarning("DXA065")
               .WithSpan(5, 28, 5, 34)
               .WithArguments("UserId"));
        test.ExpectedDiagnostics.Add(
            DiagnosticResult.CompilerWarning("DXA065")
               .WithSpan(8, 28, 8, 34)
               .WithArguments("Result"));

        await test.RunAsync();
    }

    [Fact]
    public async Task SeeCref_Used_NoDiagnostic()
    {
        const string code = @"
using Dx.Domain.Annotations;
[assembly: DxLayer(""Kernel"")]
namespace Dx.Domain.Primitives;
/// <summary>Creates a new <see cref=""UserId""/>.</summary>
public readonly struct UserId { }";

        var test = new CSharpAnalyzerTest<DXA065_UnresolvedXmlDocReferenceAnalyzer, DefaultVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
        };
        test.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(DxLayerAttribute).Assembly.Location));

        await test.RunAsync();
    }

    [Fact]
    public async Task PlainTypeName_InCodeTag_Ignored()
    {
        const string code = @"
using Dx.Domain.Annotations;
[assembly: DxLayer(""Kernel"")]
namespace Dx.Domain.Primitives;
/// <summary>Use <c>UserId</c> for identifiers.</summary>
public readonly struct UserId { }";

        var test = new CSharpAnalyzerTest<DXA065_UnresolvedXmlDocReferenceAnalyzer, DefaultVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
        };
        test.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(DxLayerAttribute).Assembly.Location));

        await test.RunAsync();
    }

    [Fact]
    public async Task NonPublicApi_Ignored()
    {
        const string code = @"
using Dx.Domain.Annotations;
[assembly: DxLayer(""Kernel"")]
namespace Dx.Domain.Primitives;
/// <summary>Internal helper using Result.</summary>
internal class Helper { }";

        var test = new CSharpAnalyzerTest<DXA065_UnresolvedXmlDocReferenceAnalyzer, DefaultVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
        };
        test.TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(DxLayerAttribute).Assembly.Location));

        await test.RunAsync();
    }

    [Fact]
    public async Task NonKernelAssembly_Ignored()
    {
        const string code = @"
namespace MyApp.Domain;
/// <summary>Uses UserId from domain.</summary>
public class Service { }";

        var test = new CSharpAnalyzerTest<DXA065_UnresolvedXmlDocReferenceAnalyzer, DefaultVerifier>
        {
            TestCode = code,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
        };

        await test.RunAsync();
    }
}
