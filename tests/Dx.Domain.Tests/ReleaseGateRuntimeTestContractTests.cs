using Xunit;

namespace Dx.Domain.Tests;

public sealed class ReleaseGateRuntimeTestContractTests
{
    [Fact]
    public void Runtime_test_project_exposes_a_discoverable_test()
    {
        Assert.True(true);
    }
}
