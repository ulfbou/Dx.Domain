using System;
using Zentient.Metadata.Attributes;
using Zentient.Abstractions.Metadata.Definitions;

namespace Samples.Attributes
{
    public sealed class ConfidentialTag : IMetadataTagDefinition { }

    [CategoryDefinition("Finance")]
    [MetadataTagAttribute(typeof(ConfidentialTag), true)]
    public class Invoice { }

    public sealed class Program
    {
        public static void Main()
        {
            var metadata = MetadataAttributeReader.GetMetadata(typeof(Invoice));

            var category = metadata.GetValueOrDefault(typeof(Invoice).FullName!, "Unknown");
            var confidential = metadata.GetValueOrDefault(typeof(ConfidentialTag).FullName!, false);

            Console.WriteLine($"Category: {category}");
            Console.WriteLine($"Confidential: {confidential}");
        }
    }
}
