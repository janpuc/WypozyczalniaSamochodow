using AwesomeAssertions;

using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Formating;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation;

public sealed class DomainViewFormatterTests
{
    private sealed class UnknownReservationStatus : ReservationStatus
    {
        public override string Label => "Nieznany";
    }

    private static DomainViewFormatter Formatter()
        => new(new TextStyler(), ScreenTestData.Clock);

    [Fact]
    public void VehicleStatusShowsActiveNonBreakdownEvent()
    {
        var vehicle = ScreenTestData.CreateVehicle();
        // Aktywne dziś (2026-05-25) zdarzenie nieserwisowe, które NIE jest awarią z naprawą.
        vehicle.AddEvent(new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 5, 24), new DateOnly(2026, 5, 26)), "Serwis"));

        Formatter().VehicleStatus(vehicle).Should().Contain(UiStrings.EventMaintenance);
    }

    [Fact]
    public void EventLabelColoredReturnsPlainDescriptionWhenInactive()
    {
        var ev = new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 12)), "Serwis");

        Formatter().EventLabelColored(ev, active: false).Should().Be(ev.Describe());
    }

    [Fact]
    public void ReservationStatusReturnsPlainLabelForRolelessStatus()
    {
        var vehicle = ScreenTestData.CreateVehicle();
        var ev = new ReservationEvent(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)));
        var reservation = Reservation.Rehydrate(ScreenTestData.CreateClient(), vehicle, ev,
            new CashPayment(new Money(300m), ScreenTestData.Clock.Now), new UnknownReservationStatus());

        Formatter().ReservationStatus(reservation).Should().Be("Nieznany");
    }

    [Fact]
    public void AddVehicleRowsMarksMissingInsurance()
    {
        var vehicle = ScreenTestData.CreateVehicleWithoutInsurance();
        var table = new UiTable().AddColumns(UiStrings.FieldKey, UiStrings.FieldValue);

        Formatter().AddVehicleRows(table, vehicle);

        table.Rows.Should().Contain(r => r.Length == 2 && r[0] == UiStrings.InsuranceActive && r[1].Contains(UiStrings.No));
    }

    [Fact]
    public void AddReservationRowsLeaveMileageEmptyForPendingReservation()
    {
        var vehicle = ScreenTestData.CreateVehicle();
        var reservation = ScreenTestData.CreateReservation(ScreenTestData.CreateClient(), vehicle);
        var table = new UiTable().AddColumns(UiStrings.FieldKey, UiStrings.FieldValue);

        Formatter().AddReservationRows(table, reservation);

        table.Rows.Should().Contain(r => r.Length == 2 && r[0] == UiStrings.MileageBefore && r[1] == UiStrings.Empty);
        table.Rows.Should().NotContain(r => r.Length == 2 && r[0] == UiStrings.CompletionNote);
    }

    [Fact]
    public void ReservationRowsIncludeCompletionNoteWhenPresent()
    {
        var vehicle = ScreenTestData.CreateVehicle();
        var reservation = ScreenTestData.CreateReservation(ScreenTestData.CreateClient(), vehicle);
        reservation.Activate(1000, ScreenTestData.Clock);
        reservation.Complete(1500, "Drobna rysa na zderzaku");

        var table = new UiTable().AddColumns(UiStrings.FieldKey, UiStrings.FieldValue);
        Formatter().AddReservationRows(table, reservation);

        table.Rows.Should().Contain(r => r.Length == 2 && r[0] == UiStrings.CompletionNote && r[1] == "Drobna rysa na zderzaku");
    }
}
