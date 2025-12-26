// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="StageDeclarationTests.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>
// <license>
//     This software is licensed under the MIT License.
//     See the project's root <c>LICENSE</c> file for details.
//     Contributions are welcome, subject to the terms of the project's license.
//     See the repository root <c>CONTRIBUTING.md</c> file for details.
// </license>
// ----------------------------------------------------------------------------------

using Dx.Domain.Generators.Core;
using Dx.Domain.Generators.Pipeline;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class StageDeclarationTests
{
    [Fact]
    public void ComputeCacheKey_WithSameInputs_ProducesSameKey()
    {
        // Arrange
        var declaration = new StageDeclaration(
            "TestStage",
            new[] { "input1" },
            new[] { "output1" },
            "1.0.0",
            cacheable: true);

        var fingerprint = InputFingerprint.Compute("intent", "manifest", "policies", "1.0.0");
        var policyVersions = "v1";

        // Act
        var key1 = declaration.ComputeCacheKey(fingerprint, policyVersions);
        var key2 = declaration.ComputeCacheKey(fingerprint, policyVersions);

        // Assert
        key1.Should().Be(key2);
    }

    [Fact]
    public void ComputeCacheKey_WithDifferentInputs_ProducesDifferentKeys()
    {
        // Arrange
        var declaration = new StageDeclaration(
            "TestStage",
            new[] { "input1" },
            new[] { "output1" },
            "1.0.0",
            cacheable: true);

        var fingerprint1 = InputFingerprint.Compute("intent1", "manifest", "policies", "1.0.0");
        var fingerprint2 = InputFingerprint.Compute("intent2", "manifest", "policies", "1.0.0");
        var policyVersions = "v1";

        // Act
        var key1 = declaration.ComputeCacheKey(fingerprint1, policyVersions);
        var key2 = declaration.ComputeCacheKey(fingerprint2, policyVersions);

        // Assert
        key1.Should().NotBe(key2);
    }

    [Fact]
    public void ComputeCacheKey_ProducesValidSha256Hash()
    {
        // Arrange
        var declaration = new StageDeclaration(
            "TestStage",
            new[] { "input1" },
            new[] { "output1" },
            "1.0.0",
            cacheable: true);

        var fingerprint = InputFingerprint.Compute("intent", "manifest", "policies", "1.0.0");
        var policyVersions = "v1";

        // Act
        var key = declaration.ComputeCacheKey(fingerprint, policyVersions);

        // Assert
        key.Should().HaveLength(64); // SHA256 = 64 hex characters
        key.Should().MatchRegex("^[a-f0-9]{64}$");
    }

    [Fact]
    public void Constructor_WithValidInputs_InitializesCorrectly()
    {
        // Arrange & Act
        var declaration = new StageDeclaration(
            "TestStage",
            new[] { "input1", "input2" },
            new[] { "output1" },
            "1.0.0",
            cacheable: true,
            new[] { "dependency1" },
            new[] { "capability1" });

        // Assert
        declaration.StageName.Should().Be("TestStage");
        declaration.InputKeys.Should().HaveCount(2);
        declaration.OutputKeys.Should().HaveCount(1);
        declaration.StageVersion.Should().Be("1.0.0");
        declaration.Cacheable.Should().BeTrue();
        declaration.DeclaredDependencies.Should().HaveCount(1);
        declaration.Capabilities.Should().HaveCount(1);
    }

    [Fact]
    public void Constructor_WithNullStageName_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new StageDeclaration(
            null!,
            new[] { "input1" },
            new[] { "output1" },
            "1.0.0",
            cacheable: true);

        act.Should().Throw<ArgumentNullException>();
    }
}
