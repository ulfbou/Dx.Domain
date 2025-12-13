using System;
using Zentient.Metadata;
using Zentient.Abstractions.Metadata.Definitions;

namespace Samples.Merging
{
    public sealed class ConfidentialTag : IMetadataTagDefinition { }

    class Program
    {
        static void Main()
        {
            var attrMetadata = Metadata.WithTag<ConfidentialTag, bool>(true);
            var runtimeMetadata = Metadata.Create().SetTag("ReviewedBy", "QA").Build();

            var merged = attrMetadata.WithMergedTags(runtimeMetadata);

            Console.WriteLine("Merged tags:");
            foreach (var tag in merged.Tags)
                Console.WriteLine($"{tag.Key} = {tag.Value}");
        }
    }
}
