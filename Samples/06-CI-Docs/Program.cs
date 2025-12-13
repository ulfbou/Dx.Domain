using System;
using System.IO;

using Zentient.Metadata.Attributes;

namespace Samples.CiDocs
{
    class Program
    {
        static void Main()
        {
            var scanner = new AttributeMetadataScanner();
            using var writer = new StreamWriter("metadata-report.md");

            foreach (var (member, metadata) in scanner.ScanAll(typeof(Program).Assembly))
            {
                if (metadata.Count == 0)
                    continue;

                writer.WriteLine($"### {member.DeclaringType?.Name ?? "<global>"}.{member.Name}");
                foreach (var tag in metadata.Tags)
                {
                    writer.WriteLine($"- **{tag.Key}**: {tag.Value}");
                }
                writer.WriteLine();
            }

            Console.WriteLine("Metadata report generated: metadata-report.md");
        }
    }
}
