// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageTransaction.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Dx;
using Dx.Domain; // Required for Unit and DomainError
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Core;

using static Dx.Dx;

namespace Dx.Domain.Generators.Orchestration
{
    /// <summary>
    /// Manages a set of proposed facts during a generator stage execution.
    /// Implements IDisposable to support 'using' blocks in the Orchestrator.
    /// </summary>
    internal sealed class StageTransaction : IFactTransaction, IDisposable
    {
        private readonly MonotonicFactStore _store;
        private readonly Dictionary<string, object> _local = new(StringComparer.Ordinal);
        private bool _disposed;

        public StageTransaction(MonotonicFactStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public Result<Unit, DomainError> Propose<T>(FactKey<T> key, T value)
            where T : notnull
        {
            if (_local.TryGetValue(key.Name, out var existing))
            {
                // Ensure the new proposal matches the existing one (monotonicity)
                if (!StructuralComparer.StructurallyEqual(existing, value))
                {
                    return Result.Failure<Unit, DomainError>(
                        Dx.Faults.InvalidInput($"Conflicting proposal for '{key.Name}'.")); // Aligned with Faults.cs
                }
                return Result.Ok<Unit, DomainError>(Unit.Value); // Aligned with Unit.cs
            }

            _local.Add(key.Name, value);
            return Result.Ok<Unit, DomainError>(Unit.Value);
        }

        public Result<T, DomainError> GetCommitted<T>(FactKey<T> key)
            where T : notnull
        {
            // Attempts to retrieve a fact already committed to the store
            if (_store.TryGet(key.Name, out var fact))
            {
                // Note: MonotonicFactStore should return a type that allows payload extraction
                return Result.Ok<T, DomainError>((T)fact!.GetPayload());
            }

            return Result.Failure<T, DomainError>(
                Dx.Faults.InvalidInput($"Missing required committed fact '{key.Name}'."));
        }

        /// <summary>
        /// Provides a read-only view of the currently pending facts in this transaction.
        /// </summary>
        public IReadOnlyDictionary<string, object> AsReadOnly() => Snapshot();

        /// <summary>
        /// Captures the current state of the transaction for the commit process.
        /// </summary>
        internal IReadOnlyDictionary<string, object> Snapshot()
            => new ReadOnlyDictionary<string, object>(
                new Dictionary<string, object>(_local, StringComparer.Ordinal));

        public void Dispose()
        {
            if (_disposed)
                return;
            _local.Clear();
            _disposed = true;
        }
    }
}
