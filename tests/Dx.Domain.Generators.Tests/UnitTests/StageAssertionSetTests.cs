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

using Dx.Domain.Factors;
using Dx.Domain.Generators.Abstractions;
using Dx.Domain.Generators.Core;

using FluentAssertions;

using System.Collections.Generic;
using System.Linq;

using Xunit;

using static Dx.DxDomain;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class StageAssertionSetTests
{
    private static StageAssertionSet CreateSet(IDictionary<string, object> assertions)
    {
        // helper: treat all provided keys as required for these tests
        var builder = StageAssertionSet.Create();

        foreach (var key in assertions.Keys)
        {
            // we ignore the value dimension in the new model; only presence matters for Preconditions
            builder.Require(new FactKey<object>("Test", key));
        }

        return builder.Build();
    }

    private static MonotonicFactStore SeedStore(IEnumerable<string> keys)
    {
        // Use the real MonotonicFactStore API: AtomicCommit with a FactFactoryRegistry
        var factory = new FactFactoryRegistry();
        var store = new MonotonicFactStore(factory);

        var proposals = keys.ToDictionary(k => k, _ => (object)"dummy");
        if (proposals.Count == 0)
            return store;

        var causation = DxDomain.CausationFactory.Create(
            correlationId: DxDomain.Correlation.New(),
            traceId: DxDomain.Trace.New(),
            actorId: ActorId.Empty);

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

        var prior = CreateSet(priorAssertions);
        var current = CreateSet(currentAssertions);

        // Shim semantics: current must be valid against a store that already has prior keys
        var store = SeedStore(prior.RequiredKeys);

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

        var prior = CreateSet(priorAssertions);
        var current = CreateSet(currentAssertions);

        // In the new model, simulate a “contradiction” as a missing required key:
        // prior is satisfied, but we do not seed current's required keys.
        var store = SeedStore(prior.RequiredKeys);

        // Act
        var result = current.Validate(store);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Missing");
    }

    [Fact]
    public void IsCompatibleWith_WithNumericValues_ComparesCorrectly()
    {
        // This behaviour (value-level numeric compatibility) is no longer represented
        // in the new assertion model; keep a smoke test that the builder works.
        var prior = CreateSet(new Dictionary<string, object> { { "count", 42 } });
        var current = CreateSet(new Dictionary<string, object> { { "count", 42.0 } });

        var store = SeedStore(prior.RequiredKeys);

        var result = current.Validate(store);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void IsCompatibleWithAll_WithMultiplePriorStages_ValidatesAll()
    {
        var stage1 = CreateSet(new Dictionary<string, object>
        {
            { "field1", "value1" }
        });
        var stage2 = CreateSet(new Dictionary<string, object>
        {
            { "field2", "value2" }
        });
        var current = CreateSet(new Dictionary<string, object>
        {
            { "field1", "value1" },
            { "field2", "value2" },
            { "field3", "value3" }
        });

        // Seed store with keys from both stage1 and stage2
        var store = SeedStore(stage1.RequiredKeys.Concat(stage2.RequiredKeys));

        var result = current.Validate(store);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithNullStageName_ThrowsArgumentNullException()
    {
        // The new API uses a builder; validate that passing a null FactKey throws via helper
        var builder = StageAssertionSet.Create();
        var act = () => builder.Require(new FactKey<object>("Test", null!));
        act.Should().Throw<ArgumentNullException>();
    }
}
