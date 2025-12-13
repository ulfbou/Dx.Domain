using System;
using Zentient.Abstractions.Metadata.Definitions;

namespace Zentient.Metadata.Attributes
{
    /// <summary>
    /// Declares a type as a metadata behavior definition. Use on classes or interfaces implementing <see cref="IBehaviorDefinition"/>.
    /// </summary>
    /// <remarks>
    /// This attribute is a marker for behavior definitions in the Zentient ecosystem.
    /// </remarks>
    /// <example>
    /// [BehaviorDefinition]
    /// public class AuditableBehavior : IBehaviorDefinition { }
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class BehaviorDefinitionAttribute : MetadataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorDefinitionAttribute"/> class.
        /// </summary>
        /// <param name="order">The order in which this attribute should be processed.</param>
        /// <param name="preset">An optional preset name associated with the attribute.</param>
        public BehaviorDefinitionAttribute(int order = 0, string? preset = null) : base(order, preset) { }
    }
}
