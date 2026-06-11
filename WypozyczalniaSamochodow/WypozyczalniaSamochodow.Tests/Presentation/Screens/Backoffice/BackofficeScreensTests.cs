using AwesomeAssertions;

using NSubstitute;

using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation.Screens.Backoffice;


public sealed class BackofficeScreensTests
{
    [Fact]
    public void EmptyDashboardTabsRenderPlaceholderRows()
    {
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();

        new BackofficeDashboardScreen(ui, prompts, clients, backoffice, vehicles, reservations, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher)).Run();

        ui.Lines.Should().Contain(UiStrings.LoggedOut);
    }

    [Fact]
    public void DashboardCanOpenReservationDetailsAndLogout()
    {
        var client = ScreenTestData.CreateClient();
        var vehicle = ScreenTestData.CreateVehicle();
        var reservation = ScreenTestData.CreateReservation(client, vehicle);

        var clients = new InMemoryClientRepository();
        clients.Add(client);
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(vehicle);
        var reservations = new InMemoryReservationRepository();
        reservations.Add(reservation);

        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer()
            .EnqueueKeys(ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();

        new BackofficeDashboardScreen(ui, prompts, clients, backoffice, vehicles, reservations, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher)).Run();

        ui.Lines.Should().Contain(UiStrings.LoggedOut);
    }

    [Fact]
    public void DashboardCanOpenVehicleClientAndAdminDetails()
    {
        var client = ScreenTestData.CreateClient();
        var admin = ScreenTestData.CreateBackoffice();
        var vehicle = ScreenTestData.CreateVehicle();

        var clients = new InMemoryClientRepository();
        clients.Add(client);
        var backoffice = new InMemoryBackofficeRepository();
        backoffice.Add(admin);
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(vehicle);
        var reservations = new InMemoryReservationRepository();

        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns("hash");
        var ui = new ScriptedUiRenderer()
            .EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Enter, ConsoleKey.Escape,
                ConsoleKey.Tab, ConsoleKey.Enter, ConsoleKey.Escape,
                ConsoleKey.Tab, ConsoleKey.Enter, ConsoleKey.Escape,
                ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();

        new BackofficeDashboardScreen(ui, prompts, clients, backoffice, vehicles, reservations, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher)).Run();

        ui.RenderedTables.Should().HaveCountGreaterThan(3);
        ui.Lines.Should().Contain(UiStrings.LoggedOut);
    }

    [Fact]
    public void DashboardCanOpenAddScreensFromAllNonReservationTabs()
    {
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns("hash");
        var ui = new ScriptedUiRenderer()
            .EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Add,
                ConsoleKey.Tab, ConsoleKey.Add,
                ConsoleKey.Tab, ConsoleKey.Add,
                ConsoleKey.Escape);
        var prompts = new ScriptedPrompts()
            .EnqueueText("Volvo", "XC60", "kr999", "vin999", "Black")
            .EnqueueDecimal(250m)
            .EnqueueInt(2025)
            .EnqueueDate(new DateOnly(2025, 1, 1))
            .EnqueueText("Nowy Klient", "nowy@example.com")
            .EnqueueSecret("secret12")
            .EnqueueConfirm(false)
            .EnqueueText("Nowy Admin", "admin2@example.com")
            .EnqueueSecret("secret12");

        new BackofficeDashboardScreen(ui, prompts, clients, backoffice, vehicles, reservations, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher)).Run();

        vehicles.All.Should().ContainSingle(v => v.Make == "Volvo");
        clients.All.Should().ContainSingle(c => c.FullName == "Nowy Klient");
        backoffice.All.Should().ContainSingle(u => u.FullName == "Nowy Admin");
    }

    [Fact]
    public void DashboardIgnoresUnknownKeys()
    {
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.F1, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();

        new BackofficeDashboardScreen(ui, prompts, clients, backoffice, vehicles, reservations, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher)).Run();

        ui.Lines.Should().Contain(UiStrings.LoggedOut);
    }
}

