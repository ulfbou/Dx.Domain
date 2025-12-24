// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="IFactFactory.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Factors;

using System.Collections.Concurrent;

using System;

namespace Dx.Domain.Generators.Core
{
    public interface IFactFactory
    {
        IDomainFact Create(string key, object value, Causation causation);
    }

    /// <summary>
    /// Typed registry for creating IDomainFact instances without reflection.
    /// </summary>
    public sealed class FactFactoryRegistry : IFactFactory
    {
        private readonly ConcurrentDictionary<Type, Func<string, object, Causation, IDomainFact>> _map
            = new();

        public void Register<T>(Func<string, T, Causation, Fact<T>> factory)
            where T : notnull
        {
            _map[typeof(T)] = (k, v, c) => factory(k, (T)v, c);
        }

        public IDomainFact Create(string key, object value, Causation causation)
        {
            ArgumentNullException.ThrowIfNull(value);

            var type = value.GetType();
            if (_map.TryGetValue(type, out var factory))
                return factory(key, value, causation);

            // Controlled reflection fallback (cold path only)
            var method = typeof(Dx.Fact)
                .GetMethod(nameof(Dx.Fact.Create))!
                .MakeGenericMethod(type);

            return (IDomainFact)method.Invoke(null, new[] { key, value, causation })!;
        }
    }
}
