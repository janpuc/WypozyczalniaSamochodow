using AwesomeAssertions;

using NSubstitute;

using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation.Screens.Backoffice;


public sealed class BackofficeAddScreensTests
{
    [Fact]
    public void AddVehicleScreenAddsVehicle()
    {
        var vehicles = new InMemoryVehicleRepository();
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("Volvo", "XC60", "kr999", "vin999", "Black")
            .EnqueueDecimal(250m)
            .EnqueueInt(2025)
            .EnqueueDate(new DateOnly(2025, 1, 1));

        new AddVehicleScreen(ui, prompts, ScreenTestData.Vehicles(vehicles)).Run();

        vehicles.All.Should().ContainSingle();
        ui.Successes.Should().Contain(UiStrings.VehicleAdded);
    }

    [Fact]
    public void AddClientScreenAddsClientWithoutLicence()
    {
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns("hash");
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("Nowy Klient", "nowy@example.com")
            .EnqueueSecret("secret12")
            .EnqueueConfirm(false);

        new AddClientScreen(ui, prompts, ScreenTestData.Users(clients, backoffice, hasher)).Run();

        clients.All.Should().ContainSingle(c => c.FullName == "Nowy Klient");
        ui.Successes.Should().Contain(UiStrings.ClientAdded);
    }

    [Fact]
    public void AddBackofficeUserScreenAddsUser()
    {
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns("hash");
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("Nowy Admin", "admin2@example.com")
            .EnqueueSecret("secret12");

        new AddBackofficeUserScreen(ui, prompts, ScreenTestData.Users(clients, backoffice, hasher)).Run();

        backoffice.All.Should().ContainSingle(u => u.FullName == "Nowy Admin");
        ui.Successes.Should().Contain(UiStrings.UserAdded);
    }

    [Fact]
    public void AddVehicleScreenShowsValidationError()
    {
        var vehicles = new InMemoryVehicleRepository();
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText(" ", "XC60", "kr999", "vin999", "Black")
            .EnqueueDecimal(250m)
            .EnqueueInt(2025)
            .EnqueueDate(new DateOnly(2025, 1, 1));

        new AddVehicleScreen(ui, prompts, ScreenTestData.Vehicles(vehicles)).Run();

        vehicles.All.Should().BeEmpty();
        ui.Errors.Should().Contain("Marka nie może być pusta.");
    }

    [Fact]
    public void AddClientScreenShowsValidationError()
    {
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns("hash");
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("Nowy Klient", "zly-email")
            .EnqueueSecret("secret12")
            .EnqueueConfirm(false);

        new AddClientScreen(ui, prompts, ScreenTestData.Users(clients, backoffice, hasher)).Run();

        clients.All.Should().BeEmpty();
        ui.Errors.Should().Contain("Podaj prawidłowy adres email.");
    }

    [Fact]
    public void AddBackofficeUserScreenShowsValidationError()
    {
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns("hash");
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("Nowy Admin", "zly-email")
            .EnqueueSecret("secret12");

        new AddBackofficeUserScreen(ui, prompts, ScreenTestData.Users(clients, backoffice, hasher)).Run();

        backoffice.All.Should().BeEmpty();
        ui.Errors.Should().Contain("Podaj prawidłowy adres email.");
    }

    [Fact]
    public void AddInsuranceScreenShowsValidationError()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText(" ", "POL-1", "OC")
            .EnqueueDate(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31))
            .EnqueueDecimal(100m);

        new AddInsuranceScreen(vehicle, ui, prompts, ScreenTestData.Vehicles()).Run();

        vehicle.Insurances.Should().BeEmpty();
        ui.Errors.Should().Contain("Firma ubezpieczeniowa nie może być pusta.");
    }

    [Fact]
    public void AddVehicleEventScreenSupportsBrokenDownAndSuspendedEvents()
    {
        var brokenVehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        var brokenUi = new ScriptedUiRenderer();
        var brokenPrompts = new ScriptedPrompts()
            .EnqueueChoice(UiStrings.EventBrokenDown)
            .EnqueueDate(new DateOnly(2026, 5, 26))
            .EnqueueText("Awaria");

        new AddVehicleEventScreen(brokenVehicle, brokenUi, brokenPrompts, ScreenTestData.Vehicles(), ScreenTestData.Clock).Run();

        brokenVehicle.Schedule.NonReservationEvents.Should().ContainSingle(e => e is BrokenDownEvent);
        brokenUi.Successes.Should().Contain(UiStrings.EventAdded);

        var suspendedVehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        var suspendedUi = new ScriptedUiRenderer();
        var suspendedPrompts = new ScriptedPrompts()
            .EnqueueChoice(UiStrings.EventSuspended)
            .EnqueueDate(new DateOnly(2026, 5, 27))
            .EnqueueText("Wstrzymany");

        new AddVehicleEventScreen(suspendedVehicle, suspendedUi, suspendedPrompts, ScreenTestData.Vehicles(), ScreenTestData.Clock).Run();

        suspendedVehicle.Schedule.NonReservationEvents.Should().ContainSingle(e => e is SuspendedEvent);
        suspendedUi.Successes.Should().Contain(UiStrings.EventAdded);
    }

    [Fact]
    public void AddVehicleEventScreenSupportsInspectionAndDetailingEvents()
    {
        var inspectionVehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        var inspectionUi = new ScriptedUiRenderer();
        var inspectionPrompts = new ScriptedPrompts()
            .EnqueueChoice(UiStrings.EventInspection)
            .EnqueueDate(new DateOnly(2026, 5, 26))
            .EnqueueText("Przeglad")
            .EnqueueDate(new DateOnly(2026, 5, 27));

        new AddVehicleEventScreen(inspectionVehicle, inspectionUi, inspectionPrompts, ScreenTestData.Vehicles(), ScreenTestData.Clock).Run();

        inspectionVehicle.Schedule.NonReservationEvents.Should().ContainSingle(e => e is InspectionEvent);

        var detailingVehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        var detailingUi = new ScriptedUiRenderer();
        var detailingPrompts = new ScriptedPrompts()
            .EnqueueChoice(UiStrings.EventDetailing)
            .EnqueueDate(new DateOnly(2026, 5, 28))
            .EnqueueText("Detailing")
            .EnqueueDate(new DateOnly(2026, 5, 29));

        new AddVehicleEventScreen(detailingVehicle, detailingUi, detailingPrompts, ScreenTestData.Vehicles(), ScreenTestData.Clock).Run();

        detailingVehicle.Schedule.NonReservationEvents.Should().ContainSingle(e => e is DetailingEvent);
    }

    [Fact]
    public void AddVehicleEventScreenShowsDomainErrors()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        vehicle.AddEvent(new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 5, 26), new DateOnly(2026, 5, 27))));
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueChoice(UiStrings.EventMaintenance)
            .EnqueueDate(new DateOnly(2026, 5, 26))
            .EnqueueText("Planowy")
            .EnqueueDate(new DateOnly(2026, 5, 27));

        new AddVehicleEventScreen(vehicle, ui, prompts, ScreenTestData.Vehicles(), ScreenTestData.Clock).Run();

        ui.Errors.Should().Contain("Nowe zdarzenie nakłada się na istniejące zdarzenie w harmonogramie.");
    }

    [Fact]
    public void AddVehicleEventScreenRejectsUnknownEventType()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueChoice("Nieznane")
            .EnqueueDate(new DateOnly(2026, 5, 26))
            .EnqueueText("Opis")
            .EnqueueDate(new DateOnly(2026, 5, 27));

        var act = () => new AddVehicleEventScreen(vehicle, ui, prompts, ScreenTestData.Vehicles(), ScreenTestData.Clock).Run();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateRepairScreenShowsValidationError()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        var cause = new BrokenDownEvent(DateRange.OpenEnded(new DateOnly(2026, 5, 20)), "Awaria");
        vehicle.AddEvent(cause);
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueDate(new DateOnly(2026, 5, 26), new DateOnly(2026, 5, 25))
            .EnqueueText("Naprawa");

        new CreateRepairScreen(vehicle, cause, ui, prompts, ScreenTestData.Vehicles(), ScreenTestData.Clock).Run();

        ui.Errors.Should().Contain("Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
    }
}

