using System.Collections.Generic;

using Dx.Domain.Analyzers.Roles;
using Dx.Domain.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Moq;

using Xunit;

namespace Dx.Domain.Analyzers.Tests.UnitTests
{
    public class RoleResolverTests
    {
        [Fact]
        public void DxResolvedRole_Overrides_AssemblyAttribute()
        {
            var compilation = CreateCompilationWithRoleAttribute(DxAssemblyRole.Domain);
            var config = CreateConfig(new Dictionary<string, string>
            {
                ["build_property.DxResolvedRole"] = "Host"
            });

            var role = RoleResolver.Resolve(compilation, config);

            Assert.Equal(DxAssemblyRole.Host, role);
        }

        [Fact]
        public void RoleResolver_Falls_Back_To_Attribute_When_No_Option()
        {
            var compilation = CreateCompilationWithRoleAttribute(DxAssemblyRole.Application);
            var config = CreateConfig(new Dictionary<string, string>());

            var role = RoleResolver.Resolve(compilation, config);

            Assert.Equal(DxAssemblyRole.Application, role);
        }

        [Fact]
        public void RoleResolver_Returns_Null_When_No_Signal()
        {
            var compilation = CreateCompilation();
            var config = CreateConfig(new Dictionary<string, string>());

            var role = RoleResolver.Resolve(compilation, config);

            Assert.Null(role);
        }

        private static AnalyzerConfigOptionsProvider CreateConfig(Dictionary<string, string> values)
        {
            var mockConfig = new Mock<AnalyzerConfigOptionsProvider>();
            var mockOptions = new Mock<AnalyzerConfigOptions>();

            mockOptions.Setup(o => o.TryGetValue(It.IsAny<string>(), out It.Ref<string?>.IsAny))
                .Returns((string key, out string value) =>
                {
                    if (values.TryGetValue(key, out var configured))
                    {
                        value = configured;
                        return true;
                    }

                    value = string.Empty;
                    return false;
                });

            mockConfig.Setup(c => c.GlobalOptions).Returns(mockOptions.Object);
            return mockConfig.Object;
        }

        private static Compilation CreateCompilation()
        {
            var code = "namespace Test { public class Foo {} }";
            return CreateCompilation(code);
        }

        private static Compilation CreateCompilationWithRoleAttribute(DxAssemblyRole role)
        {
            var code = $@"""
using System;

public enum DxAssemblyRole
{{
    Domain = 1,
    Application = 2,
    Host = 4
}}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class DxAssemblyRoleAttribute : Attribute
{{
    public DxAssemblyRoleAttribute(DxAssemblyRole role) {{ }}
}}

[assembly: DxAssemblyRoleAttribute(DxAssemblyRole.{role})]

namespace Test {{ public class Foo {{ }} }}
""";

            return CreateCompilation(code);
        }

        private static Compilation CreateCompilation(string code)
        {
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location)
            };

            return CSharpCompilation.Create(
                "TestAssembly",
                new[] { CSharpSyntaxTree.ParseText(code) },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }
    }
}
