// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="InvariantViolationException.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System.Diagnostics;

namespace Dx.Domain
{
    /// <summary>
    /// Represents an exception that is thrown when an invariant condition is violated.
    /// </summary>
    /// <remarks>This exception is typically used to signal that a program invariant has been broken,
    /// indicating a serious error in program logic. The associated diagnostic information provides details about the
    /// specific invariant that was violated.</remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class InvariantViolationException : Exception
    {
        /// <summary>Gets the diagnostic information describing the violated invariant.</summary>
        public InvariantError Diagnostic { get; }

        /// <inheritdoc />
        public override string Message => Diagnostic.EffectiveMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvariantViolationException"/> class.
        /// </summary>
        /// <param name="diagnostic">The diagnostic information for the violated invariant.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private InvariantViolationException(InvariantError diagnostic)
            : base(diagnostic.EffectiveMessage)
        {
            Diagnostic = diagnostic;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal InvariantViolationException Create(InvariantError diagnostic)
            => new InvariantViolationException(diagnostic);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"InvariantViolationException: {Message}";
    }
}
