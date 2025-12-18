using Microsoft.CodeAnalysis.Operations;

namespace Dx.Domain.Analyzers.Infrastructure.Exceptions
{
    /// <summary>
    /// Stub implementation of IExceptionIntentClassifier.
    /// Returns Unknown for all throw operations (fail-open semantics).
    /// Full implementation to be added in a future phase.
    /// </summary>
    public sealed class ExceptionIntentClassifier : IExceptionIntentClassifier
    {
        /// <summary>
        /// Classifies the intent of a throw operation.
        /// </summary>
        /// <param name="throwOperation">The throw operation to classify.</param>
        /// <returns>Always returns <see cref="ExceptionIntent.Unknown"/> (stub implementation).</returns>
        public ExceptionIntent Classify(IThrowOperation throwOperation)
        {
            // Stub implementation - always returns Unknown (fail-open)
            // Full classification logic to be implemented in future phase
            return ExceptionIntent.Unknown;
        }
    }
}
