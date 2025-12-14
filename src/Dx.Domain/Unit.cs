namespace Dx.Domain
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a single, unique instance with no value. Used as the type parameter for <see cref="Result{TValue, TError}"/>
    /// when an operation succeeds but returns no data (i.e., a functional equivalent of <c>void</c>).
    /// </summary>
    /// <remarks>
    /// This is a lightweight, stack-allocated, single-ton <see langword="struct"/>.
    /// </remarks>
    public readonly record struct Unit
    {
        /// <summary>
        /// The single, unique instance of the <see cref="Unit"/> struct.
        /// </summary>
        public static Unit Value { get; } = new Unit();

        /// <summary>
        /// Prevents explicit public construction to maintain the singleton nature.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Parameter is not used but is required for explicit constructor.")]
        private Unit(object? value = null) { }
    }
}
