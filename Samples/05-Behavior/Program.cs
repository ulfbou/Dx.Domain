using System;
using Zentient.Metadata;
using Zentient.Abstractions.Metadata.Definitions;
using Zentient.Metadata.Attributes;

namespace Samples.Behavior
{
    public sealed class ExportableBehavior : IBehaviorDefinition { }

    [BehaviorDefinition] // Marker only
    public class Report { }

    sealed class Program
    {
        static void Main()
        {
            // Map BehaviorDefinitionAttribute to ExportableBehavior
            AttributeHandlerRegistry.Register<BehaviorDefinitionAttribute>((attr, ctx) =>
            {
                ctx.Builder.SetTag(typeof(ExportableBehavior).FullName!, new ExportableBehavior());
            });

            var scanner = new AttributeMetadataScanner();
            foreach (var (member, metadata) in scanner.ScanAll(typeof(Program).Assembly))
            {
                if (metadata.HasBehavior<ExportableBehavior>())
                {
                    Console.WriteLine($"Exporting: {member.Name}");
                }
            }
        }
    }
}
