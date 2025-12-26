// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="InputFingerprintTests.cs" company="Dx.Domain Team">
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

namespace Dx.Domain.Generators.Tests.UnitTests;

public class InputFingerprintTests
{
    [Fact]
    public void Compute_WithIdenticalInputs_ProducesSameFingerprint()
    {
        // Arrange
        var intent = "intent";
        var manifest = "manifest";
        var policies = "policies";
        var version = "1.0.0";

        // Act
        var fingerprint1 = InputFingerprint.Compute(intent, manifest, policies, version);
        var fingerprint2 = InputFingerprint.Compute(intent, manifest, policies, version);

        // Assert
        fingerprint1.Should().Be(fingerprint2);
        fingerprint1.Value.Should().Be(fingerprint2.Value);
    }

    [Fact]
    public void Compute_WithDifferentInputs_ProducesDifferentFingerprints()
    {
        // Arrange
        var intent1 = "intent1";
        var intent2 = "intent2";
        var manifest = "manifest";
        var policies = "policies";
        var version = "1.0.0";

        // Act
        var fingerprint1 = InputFingerprint.Compute(intent1, manifest, policies, version);
        var fingerprint2 = InputFingerprint.Compute(intent2, manifest, policies, version);

        // Assert
        fingerprint1.Should().NotBe(fingerprint2);
    }

    [Fact]
    public void Compute_ProducesValidSha256Hash()
    {
        // Arrange
        var intent = "intent";
        var manifest = "manifest";
        var policies = "policies";
        var version = "1.0.0";

        // Act
        var fingerprint = InputFingerprint.Compute(intent, manifest, policies, version);

        // Assert
        fingerprint.Value.Should().HaveLength(64); // SHA256 = 64 hex characters
        fingerprint.Value.Should().MatchRegex("^[a-f0-9]{64}$");
    }

    [Fact]
    public void Compute_WithNullIntent_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => InputFingerprint.Compute(null!, "manifest", "policies", "1.0.0");
        act.Should().Throw<ArgumentNullException>().WithParameterName("canonicalizedIntent");
    }

    [Fact]
    public void FromHash_WithValidHash_CreatesFingerprint()
    {
        // Arrange
        var hash = "a".PadRight(64, 'b');

        // Act
        var fingerprint = InputFingerprint.FromHash(hash);

        // Assert
        fingerprint.Value.Should().Be(hash);
    }

    [Fact]
    public void FromHash_WithNullHash_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => InputFingerprint.FromHash(null!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_WithSameHash_ReturnsTrue()
    {
        // Arrange
        var hash = "a".PadRight(64, 'b');
        var fingerprint1 = InputFingerprint.FromHash(hash);
        var fingerprint2 = InputFingerprint.FromHash(hash);

        // Act & Assert
        fingerprint1.Equals(fingerprint2).Should().BeTrue();
        (fingerprint1 == fingerprint2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameHash_ReturnsSameHashCode()
    {
        // Arrange
        var hash = "a".PadRight(64, 'b');
        var fingerprint1 = InputFingerprint.FromHash(hash);
        var fingerprint2 = InputFingerprint.FromHash(hash);

        // Act & Assert
        fingerprint1.GetHashCode().Should().Be(fingerprint2.GetHashCode());
    }
}
