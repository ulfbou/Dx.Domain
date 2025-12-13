// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
//
// Tests for Zentient.Metadata.MetadataBuilder via the public Metadata API.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using Zentient.Metadata;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Readers;

namespace Zentient.Metadata.Tests
{
    [Trait("Category", "Unit")]
    [Collection("MetadataBuilder Collection")]
    public class MetadataBuilderTests
    {
        [Fact(DisplayName = "SetTag sets and overwrites tags")]
        public void SetTag_ShouldSetAndOverwriteTags()
        {
            var builder = Metadata.Create();
            builder.SetTag("key1", "value1");
            builder.SetTag("key2", 42);
            builder.SetTag("key1", "newValue");

            var metadata = builder.Build();
            metadata.Count.Should().Be(2);
            metadata.GetValueOrDefault<string>("key1").Should().Be("newValue");
            metadata.GetValueOrDefault<int>("key2").Should().Be(42);
        }

        [Fact(DisplayName = "AddTags adds multiple tags and overwrites existing")]
        public void AddTags_ShouldAddAndOverwriteTags()
        {
            var builder = Metadata.Create();
            builder.SetTag("a", 1);
            var tags = new[]
            {
                new KeyValuePair<string, object?>("a", 2),
                new KeyValuePair<string, object?>("b", 3)
            };
            builder.AddTags(tags);

            var metadata = builder.Build();
            metadata.Count.Should().Be(2);
            metadata.GetValueOrDefault<int>("a").Should().Be(2);
            metadata.GetValueOrDefault<int>("b").Should().Be(3);
        }

        [Fact(DisplayName = "RemoveTag removes tag if exists")]
        public void RemoveTag_ShouldRemoveTagIfExists()
        {
            var builder = Metadata.Create();
            builder.SetTag("x", 1).SetTag("y", 2);
            builder.RemoveTag("x");

            var metadata = builder.Build();
            metadata.Count.Should().Be(1);
            metadata.ContainsKey("x").Should().BeFalse();
            metadata.ContainsKey("y").Should().BeTrue();
        }

        [Fact(DisplayName = "RemoveTags removes multiple tags efficiently")]
        public void RemoveTags_ShouldRemoveMultipleTags()
        {
            var builder = Metadata.Create();
            builder.SetTag("a", 1).SetTag("b", 2).SetTag("c", 3);
            builder.RemoveTags(new[] { "a", "c" });

            var metadata = builder.Build();
            metadata.Count.Should().Be(1);
            metadata.ContainsKey("b").Should().BeTrue();
        }

        [Fact(DisplayName = "RemoveTags with predicate removes correct tags")]
        public void RemoveTags_WithPredicate_RemovesCorrectTags()
        {
            var builder = Metadata.Create();
            builder.SetTag("foo", 1).SetTag("bar", 2).SetTag("baz", 3);
            builder.RemoveTags((key, value) => key.StartsWith("b"));

            var metadata = builder.Build();
            metadata.Count.Should().Be(1);
            metadata.ContainsKey("foo").Should().BeTrue();
        }

        [Fact(DisplayName = "UpdateTags updates values based on predicate")]
        public void UpdateTags_ShouldUpdateValues()
        {
            var builder = Metadata.Create();
            builder.SetTag("a", 1).SetTag("b", 2);
            builder.UpdateTags((k, v) => k == "a", (k, v) => ((int)v!) + 10);

            var metadata = builder.Build();
            metadata.GetValueOrDefault<int>("a").Should().Be(11);
            metadata.GetValueOrDefault<int>("b").Should().Be(2);
        }

        [Fact(DisplayName = "Merge merges tags from another IMetadataReader")]
        public void Merge_ShouldMergeTagsFromOtherReader()
        {
            var builder = Metadata.Create();
            builder.SetTag("x", 1);

            var otherTags = new Dictionary<string, object?> { { "y", 2 }, { "x", 3 } };
            IMetadataReader otherReader = Metadata.Create().AddTags(otherTags).Build();

            builder.Merge(otherReader);

            var metadata = builder.Build();
            metadata.Count.Should().Be(2);
            metadata.GetValueOrDefault<int>("x").Should().Be(3);
            metadata.GetValueOrDefault<int>("y").Should().Be(2);
        }

        [Fact(DisplayName = "Build returns immutable metadata")]
        public void Build_ShouldReturnImmutableMetadata()
        {
            var builder = Metadata.Create();
            builder.SetTag("a", 1);
            var metadata = builder.Build();

            Action act = () => ((IDictionary<string, object?>)metadata.Tags).Add("b", 2);
            act.Should().Throw<NotSupportedException>();
        }

        [Fact(DisplayName = "Metadata WithTag returns new instance with added tag")]
        public void Metadata_WithTag_ReturnsNewInstanceWithAddedTag()
        {
            var builder = Metadata.Create();
            builder.SetTag("a", 1);
            var metadata = builder.Build();
            var newMetadata = metadata.WithTag("b", 2);

            newMetadata.Count.Should().Be(2);
            newMetadata.GetValueOrDefault<int>("b").Should().Be(2);
            metadata.Count.Should().Be(1);
        }

        [Fact(DisplayName = "Metadata WithoutTag returns new instance without specified tag")]
        public void Metadata_WithoutTag_ReturnsNewInstanceWithoutTag()
        {
            var builder = Metadata.Create();
            builder.SetTag("a", 1).SetTag("b", 2);
            var metadata = builder.Build();
            var newMetadata = metadata.WithoutTag("a");

            newMetadata.Count.Should().Be(1);
            newMetadata.ContainsKey("a").Should().BeFalse();
            metadata.Count.Should().Be(2);
        }

        [Fact(DisplayName = "Metadata WithTags merges multiple tags")]
        public void Metadata_WithTags_MergesMultipleTags()
        {
            var builder = Metadata.Create();
            builder.SetTag("a", 1);
            var metadata = builder.Build();
            var newMetadata = metadata.WithTags(new[]
            {
                new KeyValuePair<string, object?>("b", 2),
                new KeyValuePair<string, object?>("a", 3)
            });

            newMetadata.Count.Should().Be(2);
            newMetadata.GetValueOrDefault<int>("a").Should().Be(3);
            newMetadata.GetValueOrDefault<int>("b").Should().Be(2);
        }

        [Fact(DisplayName = "Metadata WithoutTags removes multiple tags")]
        public void Metadata_WithoutTags_RemovesMultipleTags()
        {
            var builder = Metadata.Create();
            builder.SetTag("a", 1).SetTag("b", 2).SetTag("c", 3);
            var metadata = builder.Build();
            var newMetadata = metadata.WithoutTags(new[] { "a", "c" });

            newMetadata.Count.Should().Be(1);
            newMetadata.ContainsKey("b").Should().BeTrue();
        }

        [Fact(DisplayName = "Metadata Merge merges tags from IMetadataReader")]
        public void Metadata_Merge_MergesTagsFromIMetadataReader()
        {
            var builder = Metadata.Create();
            builder.SetTag("x", 1);
            var metadata = builder.Build();

            var otherTags = new Dictionary<string, object?> { { "y", 2 }, { "x", 3 } };
            IMetadataReader otherReader = Metadata.Create().AddTags(otherTags).Build();
            var merged = metadata.Merge(otherReader);

            merged.Count.Should().Be(2);
            merged.GetValueOrDefault<int>("x").Should().Be(3);
            merged.GetValueOrDefault<int>("y").Should().Be(2);
        }
    }
}
