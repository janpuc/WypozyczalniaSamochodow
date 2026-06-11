using AwesomeAssertions;

using NSubstitute;

using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation;

public sealed class ProgramTests
{
    [Fact]
    public void MainRunsShellCreatedByFactory()
    {
        var ui = new ScriptedUiRenderer().EnqueueMenu(UiStrings.MenuExit);

        Program.Run(() => CreateShell(ui));

        ui.Lines.Should().Contain(UiStrings.Goodbye);
    }

    private static AppShell CreateShell(ScriptedUiRenderer ui)
    {
        var clock = ScreenTestData.Clock;
        var hasher = Substitute.For<IPasswordHasher>();
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var auth = new AuthService(clients, backoffice, hasher);
        var prompts = new ScriptedPrompts();
        return new AppShell(auth, ui, prompts, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher));
    }
}
