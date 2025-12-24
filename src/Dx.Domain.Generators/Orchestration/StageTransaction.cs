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

using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Core;

using static Dx.Dx;

using System.Collections.ObjectModel;

namespace Dx.Domain.Generators.Orchestration
{
    internal sealed class StageTransaction : IFactTransaction
    {
        private readonly MonotonicFactStore _store;
        private readonly Dictionary<string, object> _local = new(StringComparer.Ordinal);

        public StageTransaction(MonotonicFactStore store)
        {
            _store = store;
        }

        public Result<Unit, DomainError> Propose<T>(FactKey<T> key, T value)
            where T : notnull
        {
            if (_local.TryGetValue(key.Name, out var existing))
            {
                if (!StructuralComparer.StructurallyEqual(existing, value))
                    return Result.Failure<Unit, DomainError>(
                        Faults.InvalidInput($"Conflicting proposal for '{key.Name}'."));
                return Result.Ok<Unit, DomainError>(Unit.Value);
            }

            _local.Add(key.Name, value);
            return Result.Ok(Unit.Value);
        }

        public Result<T, DomainError> GetCommitted<T>(FactKey<T> key)
            where T : notnull
        {
            if (_store.TryGet(key.Name, out var fact))
                return Result.Ok<T, DomainError>((T)fact.GetPayload());

            return Result.Failure<T, DomainError>(
                Dx.Faults.InvalidInput($"Missing fact '{key.Name}'."));
        }

        internal IReadOnlyDictionary<string, object> Snapshot()
            => new ReadOnlyDictionary<string, object>(
                new Dictionary<string, object>(_local, StringComparer.Ordinal));
    }
}
