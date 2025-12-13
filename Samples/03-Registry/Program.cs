using System;
using Zentient.Metadata;
using Zentient.Abstractions.Metadata.Definitions;

namespace Samples.Registry
{
    public sealed class FinanceCategory : ICategoryDefinition { }

    class Program
    {
        static void Main()
        {
            // Register a preset once
            MetadataRegistry.RegisterPreset("DefaultFinance", Metadata.WithCategory<FinanceCategory>());

            // Apply preset to multiple objects
            if (MetadataRegistry.TryGetPreset("DefaultFinance", out var preset))
            {
                Console.WriteLine("Preset keys: " + string.Join(", ", preset.Keys));
            }
        }
    }
}
