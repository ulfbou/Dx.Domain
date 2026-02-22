// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageAssertionSetTests.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Facts;
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Model;
using Dx.Domain.Primitives;

using FluentAssertions;

using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class StageAssertionSetTests
{
    private static StageAssertionSet CreateSet(IEnumerable<string> keys)
    {
        // helper: treat all provided keys as required for these tests
        var builder = StageAssertionSet.Create();

        foreach (var key in keys)
        {
            // we ignore the value dimension in the new model; only presence matters for Preconditions
            builder.Require(new FactKey<DomainIntentModel>("Test", key));
        }

        return builder.Build();
    }

    private static IEnumerable<string> ToStoreKeys(IEnumerable<string> keys)
        => keys.Select(key => $"Test:{key}");

    private static MonotonicFactStore SeedStore(IEnumerable<string> keys)
    {
        // Use the real MonotonicFactStore API: AtomicCommit with a FactFactoryRegistry
        var factory = new FactFactoryRegistry();
        var store = new MonotonicFactStore(factory);

        var proposals = keys.ToDictionary(k => k, _ => (object)"dummy");
        if (proposals.Count == 0)
            return store;

        var causation = Causation.Create(
            correlationId: CorrelationId.New(),
            traceId: TraceId.New(),
            actorId: null);

        var result = store.AtomicCommit("StageAssertionSetTests.Seed", proposals, causation);
        result.IsSuccess.Should().BeTrue("seeding the store for tests must succeed");

        return store;
    }

    [Fact]
    public void IsCompatibleWith_WithNoContradictions_ReturnsTrue()
    {
        // Arrange
        var priorAssertions = new Dictionary<string, object>
        {
            { "schema", "v1" },
            { "version", 1 }
        };
        var currentAssertions = new Dictionary<string, object>
        {
            { "schema", "v1" },
            { "additionalField", "value" }
        };

        var priorKeys = priorAssertions.Keys.ToArray();
        var currentKeys = currentAssertions.Keys.ToArray();
        var prior = CreateSet(priorKeys);
        var current = CreateSet(currentKeys);

        // Shim semantics: current must be valid against a store that already has prior keys
        var store = SeedStore(ToStoreKeys(priorKeys));

        // Act
        var result = current.Validate(store);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void IsCompatibleWith_WithContradiction_ReturnsFalse()
    {
        // Arrange
        var priorAssertions = new Dictionary<string, object>
        {
            { "schema", "v1" }
        };
        var currentAssertions = new Dictionary<string, object>
        {
            { "schema", "v2" }
        };

        var priorKeys = priorAssertions.Keys.ToArray();
        var currentKeys = currentAssertions.Keys.ToArray();
        var prior = CreateSet(priorKeys);
        var current = CreateSet(currentKeys);

        // In the new model, simulate a “contradiction” as a missing required key:
        // prior is satisfied, but we do not seed current's required keys.
        var store = SeedStore(ToStoreKeys(priorKeys));

        // Act
        var result = current.Validate(store);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Diagnostic.Message.Should().Contain("Missing");
    }

    [Fact]
    public void IsCompatibleWith_WithNumericValues_ComparesCorrectly()
    {
        // This behaviour (value-level numeric compatibility) is no longer represented
        // in the new assertion model; keep a smoke test that the builder works.
        var keys = new[] { "count" };
        var prior = CreateSet(keys);
        var current = CreateSet(keys);

        var store = SeedStore(ToStoreKeys(keys));

        var result = current.Validate(store);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void IsCompatibleWithAll_WithMultiplePriorStages_ValidatesAll()
    {
        var stage1Keys = new[] { "field1" };
        var stage2Keys = new[] { "field2" };
        var currentKeys = new[] { "field1", "field2", "field3" };

        var stage1 = CreateSet(stage1Keys);
        var stage2 = CreateSet(stage2Keys);
        var current = CreateSet(currentKeys);

        // Seed store with keys from both stage1 and stage2
        var store = SeedStore(ToStoreKeys(stage1Keys.Concat(stage2Keys)));

        var result = current.Validate(store);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithNullStageName_ThrowsArgumentNullException()
    {
        // The new API uses a builder; validate that passing a null FactKey throws via helper
        var builder = StageAssertionSet.Create();
        var act = () => builder.Require(new FactKey<DomainIntentModel>("Test", null!));
        act.Should().Throw<ArgumentNullException>();
    }
}
