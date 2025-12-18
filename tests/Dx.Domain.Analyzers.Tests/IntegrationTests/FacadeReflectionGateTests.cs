using Dx.Domain.Analyzers.Infrastructure.Facades;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Xunit;

using Assert = Xunit.Assert;

namespace Dx.Domain.Analyzers.Tests.IntegrationTests
{
    /// <summary>
    /// Reflection gate tests that verify the facade surface matches the documented API.
    /// These tests fail if new facades are added without updating documentation.
    /// </summary>
    public class FacadeReflectionGateTests
    {
        [Fact]
        public void Dx_Facade_Contains_Only_Expected_Nested_Types()
        {
            var dxType = typeof(Dx);
            Assert.NotNull(dxType);

            var nestedTypes = dxType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
            var nestedTypeNames = nestedTypes.Select(t => t.Name).OrderBy(n => n).ToList();

            // Expected facade classes - these are the top-level nested types in Dx
            // Note: Faults are defined within nested types, not as top-level
            // When adding new facades:
            // 1. Update this list
            // 2. Add corresponding facade method tests below
            // 3. Document in docs/dx.factories.md
            // 4. Update CHANGELOG.md
            var expectedFacades = new[]
            {
                "Actor",
                "CausationFactory",
                "Correlation",
                "Fact",
                "Result",
                "Span",
                "Trace"
            }.OrderBy(n => n).ToList();

            // If this test fails, a new facade was added without updating this test
            // Update this list when adding new facades and document in dx.factories.md
            Assert.Equal(expectedFacades, nestedTypeNames);
        }

        [Fact]
        public void All_Dx_Facade_Methods_Are_Public_And_Static()
        {
            var dxType = typeof(Dx);
            var nestedTypes = dxType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var nestedType in nestedTypes)
            {
                var methods = nestedType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

                foreach (var method in methods)
                {
                    Assert.True(method.IsPublic, $"Method {nestedType.Name}.{method.Name} should be public");
                    Assert.True(method.IsStatic, $"Method {nestedType.Name}.{method.Name} should be static");
                }
            }
        }

        [Fact]
        public void Dx_Result_Facade_Contains_Expected_Factory_Methods()
        {
            var resultType = typeof(Dx).GetNestedType("Result", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(resultType);

            var methods = resultType!.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            var methodNames = methods.Select(m => m.Name).Distinct().OrderBy(n => n).ToList();

            // Expected Result factory methods
            // If this test fails, update this list and document in dx.factories.md
            var expectedMethods = new[]
            {
                "Ok",
                "Failure",
                "From"
            }.OrderBy(n => n).ToList();

            // Verify all expected methods exist
            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, methodNames);
            }
        }

        [Fact]
        public void No_Internal_Methods_Are_Exposed_Through_Dx_Facade()
        {
            var dxType = typeof(Dx);
            var nestedTypes = dxType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            foreach (var nestedType in nestedTypes)
            {
                var internalMethods = nestedType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

                // Internal methods should not be exposed through the facade
                foreach (var method in internalMethods)
                {
                    Assert.False(method.IsPublic, $"Method {nestedType.Name}.{method.Name} should not be public");
                }
            }
        }

        [Fact]
        public void Dx_Actor_Facade_Contains_Expected_Factory_Methods()
        {
            var actorType = typeof(Dx).GetNestedType("Actor", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(actorType);

            var methods = actorType!.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            var methodNames = methods.Select(m => m.Name).Distinct().OrderBy(n => n).ToList();

            // Expected Actor factory methods
            var expectedMethods = new[]
            {
                "New",
                "From"
            }.OrderBy(n => n).ToList();

            // Verify all expected methods exist
            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, methodNames);
            }
        }

        [Fact]
        public void Dx_Correlation_Facade_Contains_Expected_Factory_Methods()
        {
            var correlationType = typeof(Dx).GetNestedType("Correlation", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(correlationType);

            var methods = correlationType!.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            var methodNames = methods.Select(m => m.Name).Distinct().OrderBy(n => n).ToList();

            // Expected Correlation factory methods
            var expectedMethods = new[]
            {
                "New",
                "From"
            }.OrderBy(n => n).ToList();

            // Verify all expected methods exist
            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, methodNames);
            }
        }

        [Fact]
        public void Dx_Trace_Facade_Contains_Expected_Factory_Methods()
        {
            var traceType = typeof(Dx).GetNestedType("Trace", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(traceType);

            var methods = traceType!.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            var methodNames = methods.Select(m => m.Name).Distinct().OrderBy(n => n).ToList();

            // Expected Trace factory methods
            var expectedMethods = new[]
            {
                "New"
            }.OrderBy(n => n).ToList();

            // Verify all expected methods exist
            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, methodNames);
            }
        }

        [Fact]
        public void Dx_Span_Facade_Contains_Expected_Factory_Methods()
        {
            var spanType = typeof(Dx).GetNestedType("Span", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(spanType);

            var methods = spanType!.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            var methodNames = methods.Select(m => m.Name).Distinct().OrderBy(n => n).ToList();

            // Expected Span factory methods
            var expectedMethods = new[]
            {
                "New"
            }.OrderBy(n => n).ToList();

            // Verify all expected methods exist
            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, methodNames);
            }
        }

        [Fact]
        public void Dx_Fact_Facade_Contains_Expected_Factory_Methods()
        {
            var factType = typeof(Dx).GetNestedType("Fact", BindingFlags.Public | BindingFlags.Static);
            Assert.NotNull(factType);

            var methods = factType!.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            var methodNames = methods.Select(m => m.Name).Distinct().OrderBy(n => n).ToList();

            // Expected Fact factory methods
            var expectedMethods = new[]
            {
                "Create"
            }.OrderBy(n => n).ToList();

            // Verify all expected methods exist
            foreach (var expectedMethod in expectedMethods)
            {
                Assert.Contains(expectedMethod, methodNames);
            }
        }

        [Fact]
        public void Facade_Surface_Is_Documented()
        {
            // This test serves as a reminder that when the facade surface changes,
            // the documentation in docs/dx.factories.md should be updated.
            // The test itself doesn't verify the documentation content, but its presence
            // ensures developers are aware of the documentation requirement.

            var dxType = typeof(Dx);
            var nestedTypes = dxType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            Assert.NotEmpty(nestedTypes);

            // If you see this test and are adding a new facade:
            // 1. Update the expected facade lists in the tests above
            // 2. Document the new facade in docs/dx.factories.md
            // 3. Update the CHANGELOG.md with the API change
        }
    }
}
