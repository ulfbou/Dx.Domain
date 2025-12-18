using Dx.Domain.Analyzers.Infrastructure.Facades;
using Dx.Domain.Analyzers.Infrastructure.Scopes;
using Dx.Domain.Analyzers.Infrastructure.Semantics;

using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    // Support for C# 9 record types when targeting netstandard2.0
    internal static class IsExternalInit { }
}

namespace Dx.Domain.Analyzers.Infrastructure
{
    // Placeholder interfaces used by analyzers; defined to allow the project to compile.
    public interface IExceptionIntentClassifier { }
    public sealed class ResultFlowEngineWrapper { }
    public interface IGeneratedCodeDetector { }

    public sealed record AnalyzerServices(
        IScopeResolver Scope,
        IDxFacadeResolver Dx,
        ISemanticClassifier Semantic,
        IExceptionIntentClassifier Exceptions,
        ResultFlowEngineWrapper Flow,
        IGeneratedCodeDetector Generated);
}
