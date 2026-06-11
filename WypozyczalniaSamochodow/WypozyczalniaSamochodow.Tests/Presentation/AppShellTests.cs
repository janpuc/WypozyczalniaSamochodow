using AwesomeAssertions;

using NSubstitute;

using WypozyczalniaSamochodow.App;
using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation;

public sealed class AppShellTests
{
    [Fact]
    public void RunExitsFromMainMenu()
    {
        var ui = new ScriptedUiRenderer().EnqueueMenu(UiStrings.MenuExit);
        var shell = CreateShell(ui, new ScriptedPrompts());

        shell.Run();

        ui.Lines.Should().Contain(UiStrings.Goodbye);
    }

    [Fact]
    public void RunCanOpenRegisterAndLoginFlows()
    {
        var ui = new ScriptedUiRenderer().EnqueueMenu(UiStrings.MenuRegister, UiStrings.MenuLogin, UiStrings.MenuExit).EnqueueKeys(ConsoleKey.Escape);
        var prompts = new ScriptedPrompts()
            .EnqueueText("Jan Kowalski", "jan@example.com", "jan@example.com")
            .EnqueueSecret("secret12", "secret12")
            .EnqueueConfirm(false)
            .EnqueueText("jan@example.com")
            .EnqueueSecret("secret12");

        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash("secret12").Returns("hash");
        hasher.Verify("secret12", "hash").Returns(true);
        var auth = new AuthService(clients, backoffice, hasher);
        var shell = new AppShell(auth, ui, prompts, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher));

        backoffice.Add(ScreenTestData.CreateBackoffice());

        shell.Run();

        clients.All.Should().HaveCount(1);
        ui.Lines.Should().Contain(UiStrings.RegisterSuccess);
    }

    private static AppShell CreateShell(ScriptedUiRenderer ui, ScriptedPrompts prompts)
    {
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        var auth = new AuthService(clients, backoffice, hasher);
        return new AppShell(auth, ui, prompts, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher));
    }
}
