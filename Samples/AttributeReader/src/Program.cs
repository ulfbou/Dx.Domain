using System;

using Zentient.Abstractions.Common.Metadata;
using Zentient.Abstractions.Metadata;
using Zentient.Abstractions.Metadata.Definitions;
using Zentient.Metadata;
using Zentient.Metadata.Attributes;

namespace Samples.AttributeReader
{
    public static class Program
    {
        public static void Main()
        {
            ExampleUsage();
        }

        private static void ExampleUsage()
        {
            // Use reflection to get the Type of your class.
            Type repositoryType = typeof(OrderRepository);

            // Use the MetadataAttributeReader to get the metadata.
            IMetadata repositoryMetadata = MetadataAttributeReader.GetMetadata(repositoryType);

            // You can now query the metadata using the IMetadata extensions.
            bool isDataStore = repositoryMetadata.HasCategory<DataStoreCategory>();
            string version = repositoryMetadata.GetTagValue<VersionTag, string>();
            string owner = repositoryMetadata.GetTagValue<OwnerTag, string>();

            Console.WriteLine($"Is a data store? {isDataStore}");
            Console.WriteLine($"Version: {version}");
            Console.WriteLine($"Owner: {owner}");
        }
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class DataStoreCategory : Attribute, ICategoryDefinition { }
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class VersionTag : Attribute, IMetadataTagDefinition { }
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class OwnerTag : Attribute, IMetadataTagDefinition { }

    // 2. Apply attributes to a component class.
    [DefinitionCategory(nameof(DataStoreCategory))]
    [DefinitionTag]
    [CategoryDefinition(nameof(DataStoreCategory))]
    [MetadataTag(typeof(VersionTag), "1.0.0")]
    [MetadataTag(typeof(OwnerTag), "DataTeam")]
    internal sealed class OrderRepository { }
}
