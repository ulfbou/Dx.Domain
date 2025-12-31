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
using Dx.Domain;
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Core;

using static Dx.DxDomain;

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
            // Use Namespace:Name for unique key identification in the global store
            var compositeKey = $"{key.Namespace}:{key.Name}";

            if (_local.TryGetValue(compositeKey, out var existing))
            {
                // Monotonic Invariant: Once proposed, a fact cannot change within a transaction
                if (!StructuralComparer.StructurallyEqual(existing, value))
                {
                    return Result.Failure<Unit, DomainError>(
                        DxDomain.Faults.InvalidInput($"Conflicting proposal for '{compositeKey}'."));
                }
                return Result.Ok<Unit, DomainError>(Unit.Value);
            }

            _local.Add(compositeKey, value);
            return Result.Ok<Unit, DomainError>(Unit.Value);
        }

        public Result<T, DomainError> GetCommitted<T>(FactKey<T> key)
            where T : notnull
        {
            var compositeKey = $"{key.Namespace}:{key.Name}";

            // Attempts to retrieve a fact already committed to the store
            if (_store.TryGet(compositeKey, out var fact))
            {
                var payload = fact!.GetPayload();
                if (payload is T typedValue)
                {
                    return Result.Ok<T, DomainError>(typedValue);
                }

                return Result.Failure<T, DomainError>(
                    DxDomain.Faults.InvalidInput($"Type mismatch for committed fact '{compositeKey}'. Expected {typeof(T).Name}."));
            }

            return Result.Failure<T, DomainError>(
                DxDomain.Faults.InvalidInput($"Missing required committed fact '{compositeKey}'."));
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
