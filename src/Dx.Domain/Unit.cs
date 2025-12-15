namespace Dx.Domain
{
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
        public static Unit Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_value == null)
                {
                    _value = new Unit();
                }

                return (Unit)_value.Value;
            }
        }
        private static Unit? _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Unit"/> struct.
        /// </summary>
        public Unit() { }
    }
}
