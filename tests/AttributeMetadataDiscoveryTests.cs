// Copyright © 2025 Zentient Framework Team. All rights reserved.
// Tests for attribute-based metadata discovery and conversion, as per Zentient_Metadata_Metadata-Attribute-Specification.md

using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;
using Zentient.Metadata.Attributes;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Definitions;

namespace Zentient.Metadata.Tests
{
    // Dummy tag types for testing
    public class VersionTag : IMetadataTagDefinition { }
    public class CacheableTag : IMetadataTagDefinition { }

    [BehaviorDefinition]
    public class AuditableBehavior : IBehaviorDefinition { }

    [CategoryDefinition]
    public class ServiceCategory : ICategoryDefinition { }

    [Zentient.Abstractions.Common.Metadata.DefinitionCategory("service")]
    public interface IServiceDefinition { }

    [MetadataTag(typeof(VersionTag), "1.2")]
    [MetadataTag(typeof(CacheableTag), true)]
    public class MyService { }

    [Zentient.Abstractions.Common.Metadata.DefinitionTag("auditable", "cacheable")]
    public interface ILegacyServiceDefinition { }

    public class AttributeMetadataDiscoveryTests
    {
        [Fact]
        public void BehaviorDefinitionAttribute_Produces_Behavior_Metadata()
        {
            var metadata = MetadataAttributeReader.GetMetadata(typeof(AuditableBehavior));
            metadata.HasBehavior<AuditableBehavior>().Should().BeTrue();
        }

        [Fact]
        public void CategoryDefinitionAttribute_Produces_Category_Metadata()
        {
            var metadata = MetadataAttributeReader.GetMetadata(typeof(ServiceCategory));
            metadata.HasCategory<ServiceCategory>().Should().BeTrue();
        }

        [Fact]
        public void MetadataTagAttribute_Produces_Tag_Metadata()
        {
            var metadata = MetadataAttributeReader.GetMetadata(typeof(MyService));
            metadata.HasTag<VersionTag>().Should().BeTrue();
            metadata.GetTagValue<VersionTag, string>().Should().Be("1.2");
            metadata.HasTag<CacheableTag>().Should().BeTrue();
            metadata.GetTagValue<CacheableTag, bool>().Should().BeTrue();
        }

        [Fact]
        public void DefinitionCategoryAttribute_Produces_Category_Metadata()
        {
            var metadata = MetadataAttributeReader.GetMetadata(typeof(IServiceDefinition));
            metadata.GetValueOrDefault<string>("category").Should().Be("service");
        }

        [Fact]
        public void DefinitionTagAttribute_Produces_Tags_Metadata()
        {
            var metadata = MetadataAttributeReader.GetMetadata(typeof(ILegacyServiceDefinition));
            var tags = metadata.GetValueOrDefault<string[]>("tags");
            tags.Should().NotBeNull();
            tags.Should().Contain(new[] { "auditable", "cacheable" });
        }

        [Fact]
        public void AttributeMetadataReader_Supports_MemberInfo_Metadata()
        {
            var prop = typeof(MemberTagTestClass).GetProperty(nameof(MemberTagTestClass.Flag));
            var metadata = MetadataAttributeReader.GetMetadata(prop!);
            metadata.HasTag<CacheableTag>().Should().BeTrue();
            metadata.GetTagValue<CacheableTag, bool>().Should().BeTrue();
        }

        [Fact]
        public void AttributeMetadataReader_Resolves_Conflicts_LastDeclaredWins()
        {
            var metadata = AttributeMetadataConverter.Convert(new Attribute[]
            {
                new MetadataTagAttribute(typeof(VersionTag), "1.0"),
                new MetadataTagAttribute(typeof(VersionTag), "2.0")
            });
            metadata.GetTagValue<VersionTag, string>().Should().Be("2.0");
        }

        [Fact]
        public void Custom_MetadataAttribute_Can_Be_Registered_And_Used()
        {
            AttributeHandlerRegistry.Register<PriorityAttribute>((attr, ctx) => ctx.Builder.SetTag("priority", attr.Level));
            var metadata = MetadataAttributeReader.GetMetadata(typeof(CustomPriorityClass));
            metadata.GetValueOrDefault<int>("priority").Should().Be(5);
        }

        // Test types for member and custom attribute scenarios
        public class MemberTagTestClass
        {
            [MetadataTag(typeof(CacheableTag), true)]
            public bool Flag { get; set; }
        }

        public class PriorityAttribute : MetadataAttribute
        {
            public int Level { get; }
            public PriorityAttribute(int level) => Level = level;
        }

        [Priority(5)]
        public class CustomPriorityClass { }
    }
}
