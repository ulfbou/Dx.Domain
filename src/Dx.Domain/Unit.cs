// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="Unit.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

namespace Dx.Domain
{
    using System.Diagnostics;

    /// <summary>
    /// Represents a single, unique instance with no value. Used as the type parameter for <see cref="Result{TValue, TError}"/>
    /// when an operation succeeds but returns no data (i.e., a functional equivalent of <c>void</c>).
    /// </summary>
    /// <remarks>
    /// This is a lightweight, stack-allocated record struct.
    /// </remarks>
    [DebuggerDisplay("Unit")]
    public readonly record struct Unit
    {
        /// <summary>
        /// Gets the single, unique instance of the <see cref="Unit"/> struct.
        /// </summary>
        public static Unit Value
        {
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
    }
}
