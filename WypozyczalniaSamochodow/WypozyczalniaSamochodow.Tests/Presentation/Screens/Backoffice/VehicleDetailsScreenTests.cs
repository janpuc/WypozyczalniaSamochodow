using AwesomeAssertions;

using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Insurance;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation.Screens.Backoffice;

public sealed class VehicleDetailsScreenTests
{
    [Fact]
    public void EmptyTabsRenderPlaceholderRows()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();

        new VehicleDetailsScreen(vehicle, ui, ScreenTestData.Vehicles(vehicles), reservations, ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations)).Run();

        ui.Lines.Should().Contain(UiStrings.NoInsurances);
        ui.Lines.Should().Contain(UiStrings.NoEvents);
        ui.Lines.Should().Contain(UiStrings.NoReservations);
    }

    [Fact]
    public void DetailsTabCanDeleteVehicle()
    {
        var vehicle = ScreenTestData.CreateVehicle();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.D, ConsoleKey.Escape).EnqueueConfirmations(true);
        var prompts = new ScriptedPrompts();
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(vehicle);
        var reservations = new InMemoryReservationRepository();

        new VehicleDetailsScreen(vehicle, ui, ScreenTestData.Vehicles(vehicles), reservations, ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations)).Run();

        vehicles.All.Should().BeEmpty();
        ui.Successes.Should().Contain(UiStrings.Removed);
    }

    [Fact]
    public void InsuranceTabCanAddAndRemoveInsurance()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        vehicle.AddInsurance(new Insurance("PZU", new PolicyNumber("POL-1"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m)));

        var ui = new ScriptedUiRenderer()
            .EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Add, ConsoleKey.D, ConsoleKey.Escape)
            .EnqueueConfirmations(true);
        var prompts = new ScriptedPrompts()
            .EnqueueText("ACME", "POL-2", "OC rozszerzone")
            .EnqueueDate(new DateOnly(2026, 2, 1), new DateOnly(2026, 12, 31))
            .EnqueueDecimal(250m);
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();

        new VehicleDetailsScreen(vehicle, ui, ScreenTestData.Vehicles(vehicles), reservations, ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations)).Run();

        vehicle.Insurances.Should().HaveCount(1);
        ui.Successes.Should().Contain(UiStrings.InsuranceAdded);
        ui.Successes.Should().Contain(UiStrings.Removed);
    }

    [Fact]
    public void EventTabCanAddEvent()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();

        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Add, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts()
            .EnqueueChoice(UiStrings.EventMaintenance)
            .EnqueueDate(new DateOnly(2026, 5, 21))
            .EnqueueText("Planowy")
            .EnqueueDate(new DateOnly(2026, 5, 23));
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();

        new VehicleDetailsScreen(vehicle, ui, ScreenTestData.Vehicles(vehicles), reservations, ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations)).Run();

        vehicle.Schedule.NonReservationEvents.Should().ContainSingle();
        ui.Successes.Should().Contain(UiStrings.EventAdded);
    }

    [Fact]
    public void EventTabCanCreateRepair()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        vehicle.AddEvent(new BrokenDownEvent(DateRange.OpenEnded(new DateOnly(2026, 5, 20)), "Awaria"));

        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.R, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts()
            .EnqueueDate(new DateOnly(2026, 5, 25), new DateOnly(2026, 5, 26))
            .EnqueueText("Naprawa");
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();

        new VehicleDetailsScreen(vehicle, ui, ScreenTestData.Vehicles(vehicles), reservations, ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations)).Run();

        vehicle.Schedule.NonReservationEvents.Should().Contain(e => e is RepairEvent);
        ui.Successes.Should().Contain(UiStrings.RepairCreated);
    }

    [Fact]
    public void ReservationsTabCanOpenReservationDetails()
    {
        var vehicle = ScreenTestData.CreateVehicle();
        var client = ScreenTestData.CreateClient();
        var reservation = ScreenTestData.CreateReservation(client, vehicle);
        var reservations = new InMemoryReservationRepository();
        reservations.Add(reservation);
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(vehicle);
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();

        new VehicleDetailsScreen(vehicle, ui, ScreenTestData.Vehicles(vehicles), reservations, ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations)).Run();

        ui.RenderedTables.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void EventTabCanRejectRemovalOfNonRemovableEvent()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        vehicle.AddEvent(new NonRemovableVehicleEvent(DateRange.Closed(new DateOnly(2026, 5, 21), new DateOnly(2026, 5, 22))));

        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.D, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();

        new VehicleDetailsScreen(vehicle, ui, ScreenTestData.Vehicles(vehicles), reservations, ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations)).Run();

        ui.Errors.Should().Contain("Nie można usunąć zdarzenia typu Rezerwacja z tego widoku.");
    }

    [Fact]
    public void DetailsAndEventTabsAllowCancellingDelete()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        vehicle.AddEvent(new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 5, 21), new DateOnly(2026, 5, 22)), "Serwis"));
        var ui = new ScriptedUiRenderer()
            .EnqueueKeys(ConsoleKey.D, ConsoleKey.Tab, ConsoleKey.Tab, ConsoleKey.D, ConsoleKey.Escape)
            .EnqueueConfirmations(false, false);
        var prompts = new ScriptedPrompts();
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(vehicle);
        var reservations = new InMemoryReservationRepository();

        new VehicleDetailsScreen(vehicle, ui, ScreenTestData.Vehicles(vehicles), reservations, ScreenTestData.Clock, ScreenTestData.Navigator(ui, prompts, vehicles, reservations)).Run();

        vehicles.All.Should().ContainSingle();
        vehicle.Schedule.NonReservationEvents.Should().ContainSingle();
    }

    private sealed class NonRemovableVehicleEvent : VehicleEvent
    {
        public NonRemovableVehicleEvent(DateRange period) : base(period, "Niedozwolone")
        {
        }

        public override string Describe() => "Niedozwolone";

        public override T Accept<T>(IVehicleEventVisitor<T> visitor)
        {
            if (visitor is CanRemoveVisitor)
                return visitor.Visit(new ReservationEvent(Period, Description));

            if (visitor is IsReservationVisitor)
                return visitor.Visit(new BrokenDownEvent(Period, Description));

            return visitor.Visit(new BrokenDownEvent(Period, Description));
        }
    }
}
