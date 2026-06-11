using AwesomeAssertions;

using WypozyczalniaSamochodow.App;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation;

public sealed class CompositionRootTests
{
    [Fact]
    public void BuildCreatesAppShell()
    {
        var shell = CompositionRoot.Build();

        shell.Should().NotBeNull();
    }
}
