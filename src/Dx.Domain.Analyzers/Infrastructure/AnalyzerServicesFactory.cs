// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="DXA070_GeneratedCodeTaggingAnalyzer.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Analyzers.Infrastructure.Exceptions;
using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Flow;
using Dx.Domain.Analyzers.Infrastructure.Generated;
using Dx.Domain.Analyzers.Infrastructure.Scopes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dx.Domain.Analyzers.Infrastructure;

/// <summary>
/// Factory for composing analyzer service dependencies in a single, testable location.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="AnalyzerServicesFactory"/> encapsulates the composition logic for all analyzer services.
/// Each analyzer instance receives a fully configured <see cref="AnalyzerServices"/> object containing
/// scope resolution, facade detection, semantic classification, and code flow analysis capabilities.
/// </para>
/// <para>
/// Design rationale:
/// - Centralizes dependency graph: single point for adding, removing, or reordering services.
/// - Enables testability: tests can mock or override individual services by re-composing.
/// - Isolates construction concern: analyzers do not know how services are created.
/// - Supports future configuration: environment or build context can influence service setup (e.g., strict mode, experimental features).
/// </para>
/// <para>
/// Service composition order does not impose runtime dependencies—all services are independent
/// implementations of their respective concerns. The factory merely ensures all are available
/// when an analyzer runs.
/// </para>
/// </remarks>
internal static class AnalyzerServicesFactory
{
    /// <summary>
    /// Composes and returns a fully configured analyzer services container.
    /// </summary>
    /// <param name="compilation">
    /// The Roslyn compilation context from which semantic information (symbols, types, metadata)
    /// is derived.
    /// </param>
    /// <param name="config">
    /// The analyzer configuration provider that supplies EditorConfig settings and rule severities.
    /// </param>
    /// <returns>
    /// A <see cref="AnalyzerServices"/> object containing initialized instances of all analyzer
    /// infrastructure components (scope resolver, facade resolver, classifiers, flow engine, etc.).
    /// </returns>
    /// <remarks>
    /// This method instantiates six core service implementations:
    /// <list type="bullet">
    /// <item><description><see cref="ScopeResolver"/> – determines scope boundary (S0–S3) for any symbol.</description></item>
    /// <item><description><see cref="DxFacadeResolver"/> – locates and caches approved Dx facade entry points.</description></item>
    /// <item><description><see cref="SemanticClassifier"/> – queries semantic symbol properties and relationships.</description></item>
    /// <item><description><see cref="ExceptionIntentClassifier"/> – detects exception-based control flow patterns.</description></item>
    /// <item><description><see cref="ResultFlowEngineWrapper"/> – drives Result&lt;T&gt; handling verification (DXA020).</description></item>
    /// <item><description><see cref="GeneratedCodeDetector"/> – identifies auto-generated code to suppress diagnostics.</description></item>
    /// </list>
    /// <para>
    /// Each service is instantiated exactly once per analyzer run. The container is thread-safe
    /// for read access and is discarded after the analysis phase completes.
    /// </para>
    /// </remarks>
    public static AnalyzerServices Create(Compilation compilation, AnalyzerConfigOptionsProvider config)
    {
        return new AnalyzerServices(
            new ScopeResolver(config),
            new DxFacadeResolver(compilation, config),
            new SemanticClassifier(compilation),
            new ExceptionIntentClassifier(compilation, config),
            new ResultFlowEngineWrapper(),
            new GeneratedCodeDetector(config));
    }
}

