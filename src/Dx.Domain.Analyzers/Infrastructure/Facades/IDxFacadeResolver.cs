using Microsoft.CodeAnalysis;

namespace Dx.Domain.Analyzers.Infrastructure.Facades
{
    public interface IDxFacadeResolver
    {
        IReadOnlyCollection<IMethodSymbol> FacadeFactories { get; }

        bool IsDxFacadeFactory(IMethodSymbol method);
        IMethodSymbol? FindFacadeFactoryForType(ITypeSymbol type);
    }
    public sealed class DxFacadeResolver : IDxFacadeResolver
    {
        private readonly HashSet<IMethodSymbol> _methods =
            new(SymbolEqualityComparer.Default);

        public DxFacadeResolver(Compilation compilation)
        {
            var dx = compilation.GetTypeByMetadataName("Dx");
            if (dx == null)
                return;

            foreach (var nested in dx.GetTypeMembers())
            {
                if (nested.DeclaredAccessibility != Accessibility.Public)
                    continue;

                foreach (var method in nested.GetMembers().OfType<IMethodSymbol>())
                {
                    if (method.DeclaredAccessibility == Accessibility.Public &&
                        method.IsStatic)
                    {
                        _methods.Add(method);
                    }
                }
            }
        }

        public IReadOnlyCollection<IMethodSymbol> FacadeFactories => _methods;

        public bool IsDxFacadeFactory(IMethodSymbol method) =>
            _methods.Contains(method);

        public IMethodSymbol? FindFacadeFactoryForType(ITypeSymbol type) =>
            _methods.FirstOrDefault(m =>
                    SymbolEqualityComparer.Default.Equals(m.ReturnType, type));
    }
}
