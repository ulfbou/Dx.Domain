using System;
using Zentient.Metadata;
using Zentient.Abstractions.Metadata.Definitions;

namespace Samples.Basics
{
    public sealed class ConfidentialTag : IMetadataTagDefinition { }
    public sealed class ReviewedByTag : IMetadataTagDefinition { }

    class Program
    {
        static void Main()
        {
            // Build metadata fluently
            var docMetadata = Metadata.Create()
                .SetTag("Author", "Ulf Bourelius")
                .SetTag(typeof(ConfidentialTag).FullName!, true)
                .SetTag(typeof(ReviewedByTag).FullName!, "QA Team")
                .Build();

            // Strongly-typed retrieval
            Console.WriteLine($"Author: {docMetadata.GetValueOrDefault("Author", "Unknown")}");
            Console.WriteLine($"Confidential: {docMetadata.GetValueOrDefault(typeof(ConfidentialTag).FullName!, false)}");
            Console.WriteLine($"Reviewed By: {docMetadata.GetValueOrDefault(typeof(ReviewedByTag).FullName!, "Nobody")}");

            // Enumerate all tags
            Console.WriteLine("\nAll tags:");
            foreach (var tag in docMetadata.Tags)
                Console.WriteLine($"- {tag.Key} = {tag.Value}");
        }
    }
}
