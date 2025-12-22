// <authors>Ulf Bourelius (Original Author)</authors>
// <copyright file="AutomationPolicyTests.cs" company="Dx.Domain Team">
//     Copyright (c) 2025 Dx.Domain Team. All rights reserved.
// </copyright>

using Dx.Domain.Generators.Governance;
using Dx.Domain.Generators.Diagnostics;

namespace Dx.Domain.Generators.Tests.UnitTests;

public class AutomationPolicyTests
{
    [Fact]
    public void CanAutoApply_WithNoneLevel_ReturnsFalse()
    {
        // Arrange
        var policy = new AutomationPolicy(
            AutoFixLevel.None,
            ImpactLevel.Safe);

        // Act
        var result = policy.CanAutoApply(ImpactLevel.Safe, isCiContext: true);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanAutoApply_WithSuggestLevel_ReturnsFalse()
    {
        // Arrange
        var policy = new AutomationPolicy(
            AutoFixLevel.Suggest,
            ImpactLevel.Safe);

        // Act
        var result = policy.CanAutoApply(ImpactLevel.Safe, isCiContext: true);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanAutoApply_WithSafeLevel_OnlySafeFixes_ReturnsTrue()
    {
        // Arrange
        var policy = new AutomationPolicy(
            AutoFixLevel.Safe,
            ImpactLevel.Safe);

        // Act
        var resultSafe = policy.CanAutoApply(ImpactLevel.Safe, isCiContext: true);
        var resultBehavioral = policy.CanAutoApply(ImpactLevel.Behavioral, isCiContext: true);

        // Assert
        resultSafe.Should().BeTrue();
        resultBehavioral.Should().BeFalse();
    }

    [Fact]
    public void CanAutoApply_WithApplyLevel_AllowsUpToMaxImpact()
    {
        // Arrange
        var policy = new AutomationPolicy(
            AutoFixLevel.Apply,
            ImpactLevel.Behavioral);

        // Act
        var resultSafe = policy.CanAutoApply(ImpactLevel.Safe, isCiContext: true);
        var resultBehavioral = policy.CanAutoApply(ImpactLevel.Behavioral, isCiContext: true);
        var resultBreaking = policy.CanAutoApply(ImpactLevel.Breaking, isCiContext: true);

        // Assert
        resultSafe.Should().BeTrue();
        resultBehavioral.Should().BeTrue();
        resultBreaking.Should().BeFalse();
    }

    [Fact]
    public void CanAutoApply_WithWrongContext_ReturnsFalse()
    {
        // Arrange
        var ciPolicy = new AutomationPolicy(
            AutoFixLevel.Apply,
            ImpactLevel.Safe,
            appliesInCi: true,
            appliesInIde: false);

        // Act
        var resultInCi = ciPolicy.CanAutoApply(ImpactLevel.Safe, isCiContext: true);
        var resultInIde = ciPolicy.CanAutoApply(ImpactLevel.Safe, isCiContext: false);

        // Assert
        resultInCi.Should().BeTrue();
        resultInIde.Should().BeFalse();
    }

    [Fact]
    public void DefaultCiPolicy_IsStrictSafeOnly()
    {
        // Arrange
        var policy = AutomationPolicy.DefaultCiPolicy;

        // Assert
        policy.AutoFixLevel.Should().Be(AutoFixLevel.Safe);
        policy.MaxAutoApplyImpact.Should().Be(ImpactLevel.Safe);
        policy.AppliesInCi.Should().BeTrue();
        policy.AppliesInIde.Should().BeFalse();
    }

    [Fact]
    public void DefaultIdePolicy_IsSuggestOnly()
    {
        // Arrange
        var policy = AutomationPolicy.DefaultIdePolicy;

        // Assert
        policy.AutoFixLevel.Should().Be(AutoFixLevel.Suggest);
        policy.AppliesInCi.Should().BeFalse();
        policy.AppliesInIde.Should().BeTrue();
    }
}
