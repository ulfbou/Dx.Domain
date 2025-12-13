// <copyright file="MetadataExtensionsTests.cs" company="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using FluentAssertions;

using System;
using System.Collections.Generic;

using Xunit;

using Zentient.Abstractions.Common.Definitions;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Definitions;
using Zentient.Metadata;

namespace Zentient.Metadata.Tests
{
    // A dummy, compliant category definition for testing purposes.
    public sealed class TestCategoryDefinition : ICategoryDefinition { }

    // A dummy, compliant behavior definition for testing purposes.
    public sealed class TestBehaviorDefinition : IBehaviorDefinition { }

    // A dummy, compliant metadata tag definition for testing purposes.
    public sealed class TestMetadataTagDefinition : IMetadataTagDefinition { }

    // A dummy, compliant identifiable behavior definition for testing purposes.
    public sealed class TestIdentifiableBehaviorDefinition : IBehaviorDefinition, IIdentifiableDefinition
    {
        public const string Id = "f8b7f8f1-c8c3-4f2b-8a3d-9d7e7e7e7e7e";

        string IIdentifiable.Id => Id;
    }

    /// <summary>
    /// Contains unit tests for the <see cref="MetadataExtensions"/> class.
    /// </summary>
    public sealed class MetadataExtensionsTests
    {
        #region Behavioral and Categorical Extensions Tests

        [Fact]
        public void WithBehavior_Adds_Behavior_And_Returns_New_Instance()
        {
            // Arrange
            var originalMetadata = Metadata.Empty;

            // Act
            var newMetadata = originalMetadata.WithBehavior<TestBehaviorDefinition>();

            // Assert
            newMetadata.Should().NotBeSameAs(originalMetadata);
            newMetadata.HasBehavior<TestBehaviorDefinition>().Should().BeTrue();
            originalMetadata.Count.Should().Be(0); // Ensure original is unchanged
        }

        [Fact]
        public void WithCategory_Adds_Category_And_Returns_New_Instance()
        {
            // Arrange
            var originalMetadata = Metadata.Empty;

            // Act
            var newMetadata = originalMetadata.WithCategory<TestCategoryDefinition>();

            // Assert
            newMetadata.Should().NotBeSameAs(originalMetadata);
            newMetadata.HasCategory<TestCategoryDefinition>().Should().BeTrue();
            originalMetadata.Count.Should().Be(0);
        }

        [Fact]
        public void HasBehavior_Returns_True_For_Existing_Behavior()
        {
            // Arrange
            var metadata = Metadata.WithBehavior<TestBehaviorDefinition>();

            // Act & Assert
            metadata.HasBehavior<TestBehaviorDefinition>().Should().BeTrue();
        }

        [Fact]
        public void HasBehavior_Returns_False_For_Non_Existing_Behavior()
        {
            // Arrange
            var metadata = Metadata.Empty;

            // Act & Assert
            metadata.HasBehavior<TestBehaviorDefinition>().Should().BeFalse();
        }

        [Fact]
        public void HasCategory_Returns_True_For_Existing_Category()
        {
            // Arrange
            var metadata = Metadata.WithCategory<TestCategoryDefinition>();

            // Act & Assert
            metadata.HasCategory<TestCategoryDefinition>().Should().BeTrue();
        }

        [Fact]
        public void HasCategory_Returns_False_For_Non_Existing_Category()
        {
            // Arrange
            var metadata = Metadata.Empty;

            // Act & Assert
            metadata.HasCategory<TestCategoryDefinition>().Should().BeFalse();
        }

        #endregion

        #region Tag Extensions Tests

        [Fact]
        public void WithTag_Adds_Tag_And_Returns_New_Instance()
        {
            // Arrange
            var originalMetadata = Metadata.Empty;
            var tagValue = "testValue";

            // Act
            var newMetadata = originalMetadata.WithTag<TestMetadataTagDefinition, string>(tagValue);

            // Assert
            newMetadata.Should().NotBeSameAs(originalMetadata);
            newMetadata.GetTagValue<TestMetadataTagDefinition, string>().Should().Be(tagValue);
            newMetadata.Count.Should().Be(1);
        }

        [Fact]
        public void WithTag_Updates_Existing_Tag_And_Returns_New_Instance()
        {
            // Arrange
            var originalMetadata = Metadata.WithTag<TestMetadataTagDefinition, string>("oldValue");

            // Act
            var newMetadata = originalMetadata.WithTag<TestMetadataTagDefinition, string>("newValue");

            // Assert
            newMetadata.Should().NotBeSameAs(originalMetadata);
            newMetadata.GetTagValue<TestMetadataTagDefinition, string>().Should().Be("newValue");
            newMetadata.Count.Should().Be(1);
        }

        [Fact]
        public void WithTagIfNotExists_DoesNot_Update_Existing_Tag()
        {
            // Arrange
            var originalMetadata = Metadata.WithTag<TestMetadataTagDefinition, string>("initialValue");

            // Act
            var newMetadata = originalMetadata.WithTagIfNotExists<TestMetadataTagDefinition, string>("ignoredValue");

            // Assert
            newMetadata.Should().BeSameAs(originalMetadata);
            newMetadata.GetTagValue<TestMetadataTagDefinition, string>().Should().Be("initialValue");
        }

        [Fact]
        public void WithTagIfNotExists_Adds_New_Tag_When_Not_Exists()
        {
            // Arrange
            var originalMetadata = Metadata.Empty;

            // Act
            var newMetadata = originalMetadata.WithTagIfNotExists<TestMetadataTagDefinition, string>("newValue");

            // Assert
            newMetadata.Should().NotBeSameAs(originalMetadata);
            newMetadata.GetTagValue<TestMetadataTagDefinition, string>().Should().Be("newValue");
        }

        [Fact]
        public void GetTagValue_Returns_Correct_Value_For_Existing_Tag()
        {
            // Arrange
            var metadata = Metadata.WithTag<TestMetadataTagDefinition, int>(42);

            // Act & Assert
            metadata.GetTagValue<TestMetadataTagDefinition, int>().Should().Be(42);
        }

        [Fact]
        public void GetTagValue_Returns_Default_Value_For_Non_Existing_Tag()
        {
            // Arrange
            var metadata = Metadata.Empty;

            // Act & Assert
            metadata.GetTagValue<TestMetadataTagDefinition, int>().Should().Be(0);
        }

        [Fact]
        public void TryGetTagValue_Returns_True_And_Correct_Value_For_Existing_Tag()
        {
            // Arrange
            var metadata = Metadata.WithTag<TestMetadataTagDefinition, string>("found");

            // Act
            bool result = metadata.TryGetTagValue<TestMetadataTagDefinition, string>(out var value);

            // Assert
            result.Should().BeTrue();
            value.Should().Be("found");
        }

        [Fact]
        public void TryGetTagValue_Returns_False_And_Default_Value_For_Non_Existing_Tag()
        {
            // Arrange
            var metadata = Metadata.Empty;

            // Act
            bool result = metadata.TryGetTagValue<TestMetadataTagDefinition, string>(out var value);

            // Assert
            result.Should().BeFalse();
            value.Should().BeNull();
        }

        [Fact]
        public void HasTag_Returns_True_For_Existing_Tag()
        {
            // Arrange
            var metadata = Metadata.WithTag<TestMetadataTagDefinition, string>("value");

            // Act & Assert
            metadata.HasTag<TestMetadataTagDefinition>().Should().BeTrue();
        }

        [Fact]
        public void HasTag_Returns_False_For_Non_Existing_Tag()
        {
            // Arrange
            var metadata = Metadata.Empty;

            // Act & Assert
            metadata.HasTag<TestMetadataTagDefinition>().Should().BeFalse();
        }

        #endregion

        #region Merging and Traversal Tests

        [Fact]
        public void WithMergedTags_Merges_Correctly_With_New_Tags()
        {
            // Arrange
            var meta1 = Metadata.WithTag<TestMetadataTagDefinition, string>("value1");
            var meta2 = Metadata.Create().SetTag("key2", "value2").Build();

            // Act
            var merged = meta1.WithMergedTags(meta2);

            // Assert
            merged.Should().NotBeSameAs(meta1);
            merged.Count.Should().Be(2);
            merged.GetTagValue<TestMetadataTagDefinition, string>().Should().Be("value1");
            merged.GetValueOrDefault<string>("key2").Should().Be("value2");
        }

        [Fact]
        public void WithMergedTags_Overwrites_Existing_Tags()
        {
            // Arrange
            var meta1 = Metadata.WithTag<TestMetadataTagDefinition, string>("oldValue");
            var meta2 = Metadata.WithTag<TestMetadataTagDefinition, string>("newValue");

            // Act
            var merged = meta1.WithMergedTags(meta2);

            // Assert
            merged.Should().NotBeSameAs(meta1);
            merged.Count.Should().Be(1);
            merged.GetTagValue<TestMetadataTagDefinition, string>().Should().Be("newValue");
        }

        [Fact]
        public void GetBehavior_Returns_Correct_Nested_Metadata_Instance()
        {
            // Arrange
            var nestedMetadata = Metadata.WithTag<TestMetadataTagDefinition, int>(123);
            var metadata = Metadata.Empty.WithTag(typeof(TestBehaviorDefinition).FullName!, nestedMetadata);

            // Act
            var retrievedBehavior = metadata.GetBehavior<TestBehaviorDefinition>();

            // Assert
            retrievedBehavior.Should().BeSameAs(nestedMetadata);
        }

        [Fact]
        public void GetBehavior_Returns_Null_For_Non_Existing_Behavior()
        {
            // Arrange
            var metadata = Metadata.Empty;

            // Act
            var retrievedBehavior = metadata.GetBehavior<TestBehaviorDefinition>();

            // Assert
            retrievedBehavior.Should().BeNull();
        }

        #endregion

        #region Exception Handling Tests

        [Fact]
        public void WithBehavior_Throws_ArgumentNullException_For_Null_Instance()
        {
            IMetadata nullMetadata = null!;
            Action act = () => nullMetadata.WithBehavior<TestBehaviorDefinition>();
            act.Should().Throw<ArgumentNullException>().WithParameterName("metadata");
        }

        [Fact]
        public void HasBehavior_Throws_ArgumentNullException_For_Null_Instance()
        {
            IMetadata nullMetadata = null!;
            Action act = () => nullMetadata.HasBehavior<TestBehaviorDefinition>();
            act.Should().Throw<ArgumentNullException>().WithParameterName("metadata");
        }

        [Fact]
        public void WithTag_Throws_ArgumentNullException_For_Null_Instance()
        {
            IMetadata nullMetadata = null!;
            Action act = () => nullMetadata.WithTag<TestMetadataTagDefinition, string>("value");
            act.Should().Throw<ArgumentNullException>().WithParameterName("metadata");
        }

        [Fact]
        public void GetTagValue_Throws_ArgumentNullException_For_Null_Instance()
        {
            IMetadata nullMetadata = null!;
            Action act = () => nullMetadata.GetTagValue<TestMetadataTagDefinition, string>();
            act.Should().Throw<ArgumentNullException>().WithParameterName("metadata");
        }

        [Fact]
        public void TryGetTagValue_Throws_ArgumentNullException_For_Null_Instance()
        {
            IMetadata nullMetadata = null!;
            Action act = () => nullMetadata.TryGetTagValue<TestMetadataTagDefinition, string>(out _);
            act.Should().Throw<ArgumentNullException>().WithParameterName("metadata");
        }

        [Fact]
        public void HasTag_Throws_ArgumentNullException_For_Null_Instance()
        {
            IMetadata nullMetadata = null!;
            Action act = () => nullMetadata.HasTag<TestMetadataTagDefinition>();
            act.Should().Throw<ArgumentNullException>().WithParameterName("metadata");
        }

        #endregion
    }
}
