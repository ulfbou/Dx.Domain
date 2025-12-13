using System;
using System.Collections.Concurrent;
using Zentient.Abstractions.Metadata.Builders;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Registry for custom attribute handlers for metadata conversion.
    /// </summary>
    public static class AttributeHandlerRegistry
    {
        private static readonly ConcurrentDictionary<Type, Delegate> _handlers = new();

        /// <summary>
        /// Registers a handler for a custom attribute type.
        /// </summary>
        /// <typeparam name="TAttr">The attribute type.</typeparam>
        /// <param name="handler">The handler delegate.</param>
        public static void Register<TAttr>(Action<TAttr, AttributeHandlerContext> handler) where TAttr : Attribute
        {
            _handlers[typeof(TAttr)] = handler;
        }

        /// <summary>
        /// Invokes a registered handler for the given attribute, if present.
        /// </summary>
        public static bool TryHandle(Attribute attr, AttributeHandlerContext ctx)
        {
            if (attr == null)
            {
                return false;
            }

            if (_handlers.TryGetValue(attr.GetType(), out var del))
            {
                del.DynamicInvoke(attr, ctx);
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Context passed to attribute handler delegates.
    /// </summary>
    public sealed class AttributeHandlerContext
    {
        public IMetadataBuilder Builder { get; }
        public AttributeHandlerContext(IMetadataBuilder builder) => Builder = builder;
    }
}
