using AwesomeAssertions;
using NSubstitute;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Screens.Client;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;
using Xunit;
namespace WypozyczalniaSamochodow.Tests.Presentation.Screens.Client;

public sealed class ClientScreensTests
{
    [Fact]
    public void DashboardCanAddLicenceAndEditPersonalData()
    {
        var client = ScreenTestData.CreateClientWithoutLicence();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Add, ConsoleKey.Tab, ConsoleKey.E, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts()
            .EnqueueLicence(new DrivingLicence("ABC123", new DateOnly(2026, 12, 31)))
            .EnqueueText("Jan Nowy", "jan.nowy@example.com");
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();

        new ClientDashboardScreen(client, ui, prompts, reservations, ScreenTestData.Users(new InMemoryClientRepository(), new InMemoryBackofficeRepository(), reservations: reservations), ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations, hasher)).Run();

        client.DrivingLicence.Should().NotBeNull();
        client.FullName.Should().Be("Jan Nowy");
        client.Email.Value.Should().Be("jan.nowy@example.com");
        ui.Successes.Should().Contain(UiStrings.LicenceAdded);
        ui.Successes.Should().Contain(UiStrings.PersonalDataUpdated);
    }

    [Fact]
    public void DashboardCanNavigateReservationsOpenDetailsAndLogout()
    {
        var client = ScreenTestData.CreateClient();
        var firstVehicle = ScreenTestData.CreateVehicle();
        var secondVehicle = ScreenTestData.CreateVehicle("Skoda", "Octavia");
        var reservations = new InMemoryReservationRepository();
        reservations.Add(ScreenTestData.CreateReservation(client, firstVehicle));
        reservations.Add(ScreenTestData.CreateReservation(client, secondVehicle,
            period: DateRange.Closed(new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 12))));
        var vehicles = new InMemoryVehicleRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.DownArrow, ConsoleKey.UpArrow, ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();

        new ClientDashboardScreen(client, ui, prompts, reservations, ScreenTestData.Users(new InMemoryClientRepository(), new InMemoryBackofficeRepository(), reservations: reservations), ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations, hasher)).Run();

        ui.RenderedTables.Should().HaveCountGreaterThan(1);
        ui.Lines.Should().Contain(UiStrings.LoggedOut);
    }

    [Fact]
    public void DashboardCanStartNewReservationFlowFromReservationsTab()
    {
        var client = ScreenTestData.CreateClient();
        var vehicle = ScreenTestData.CreateVehicle();
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(vehicle);
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Add, ConsoleKey.Enter, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts()
            .EnqueueDate(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3))
            .EnqueueChoice(UiStrings.PayCash)
            .EnqueueConfirm(true);

        new ClientDashboardScreen(client, ui, prompts, reservations, ScreenTestData.Users(new InMemoryClientRepository(), new InMemoryBackofficeRepository(), reservations: reservations), ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations, hasher)).Run();

        reservations.All.Should().ContainSingle();
        ui.Successes.Should().Contain(UiStrings.ReservationCreated);
    }

    [Fact]
    public void DashboardIgnoresUnknownReservationTabKeys()
    {
        var client = ScreenTestData.CreateClient();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.F1, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();

        new ClientDashboardScreen(client, ui, prompts, reservations, ScreenTestData.Users(new InMemoryClientRepository(), new InMemoryBackofficeRepository(), reservations: reservations), ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations, hasher)).Run();

        ui.Lines.Should().Contain(UiStrings.LoggedOut);
    }

    [Fact]
    public void DashboardShowsLockedMessagesWhenClientHasActiveReservation()
    {
        var client = ScreenTestData.CreateClient();
        var vehicle = ScreenTestData.CreateVehicle();
        var reservation = ScreenTestData.CreateReservation(client, vehicle);
        reservation.Activate(1000, ScreenTestData.Clock);
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        reservations.Add(reservation);
        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();

        new ClientDashboardScreen(client, ui, prompts, reservations, ScreenTestData.Users(new InMemoryClientRepository(), new InMemoryBackofficeRepository(), reservations: reservations), ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations, hasher)).Run();

        ui.Lines.Should().Contain(UiStrings.LicenceEditLocked);
        ui.Lines.Should().Contain(UiStrings.PersonalDataEditLocked);
    }

    [Fact]
    public void DashboardCanEditExistingLicence()
    {
        var client = ScreenTestData.CreateClient();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.E, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts().EnqueueLicence(new DrivingLicence("XYZ987", new DateOnly(2027, 1, 1)));

        new ClientDashboardScreen(client, ui, prompts, reservations, ScreenTestData.Users(new InMemoryClientRepository(), new InMemoryBackofficeRepository(), reservations: reservations), ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations, hasher)).Run();

        client.DrivingLicence!.Number.Should().Be("XYZ987");
        ui.Successes.Should().Contain(UiStrings.Updated);
    }

    [Fact]
    public void DashboardCanSurfaceLicenceEditError()
    {
        var client = ScreenTestData.CreateClient();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.E, ConsoleKey.Escape);
        var prompts = Substitute.For<IPrompts>();
        prompts.PromptDrivingLicence(Arg.Any<DrivingLicence>()).Returns(_ => throw new DomainException("Błąd prawa jazdy."));

        new ClientDashboardScreen(client, ui, prompts, reservations, ScreenTestData.Users(new InMemoryClientRepository(), new InMemoryBackofficeRepository(), reservations: reservations), ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations, hasher)).Run();

        ui.Errors.Should().Contain("Błąd prawa jazdy.");
    }

    [Fact]
    public void DashboardCanSurfaceLicenceAddError()
    {
        var client = ScreenTestData.CreateClientWithoutLicence();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Add, ConsoleKey.Escape);
        var prompts = Substitute.For<IPrompts>();
        prompts.PromptDrivingLicence(null).Returns(_ => throw new DomainException("Błąd prawa jazdy."));

        new ClientDashboardScreen(client, ui, prompts, reservations, ScreenTestData.Users(new InMemoryClientRepository(), new InMemoryBackofficeRepository(), reservations: reservations), ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations, hasher)).Run();

        ui.Errors.Should().Contain("Błąd prawa jazdy.");
    }

    [Fact]
    public void DashboardCanSurfacePersonalDataEditError()
    {
        var client = ScreenTestData.CreateClient();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.E, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts().EnqueueText("Jan Nowy", "zly-email");

        new ClientDashboardScreen(client, ui, prompts, reservations, ScreenTestData.Users(new InMemoryClientRepository(), new InMemoryBackofficeRepository(), reservations: reservations), ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations, hasher)).Run();

        ui.Errors.Should().Contain("Podaj prawidłowy adres email.");
    }
}

