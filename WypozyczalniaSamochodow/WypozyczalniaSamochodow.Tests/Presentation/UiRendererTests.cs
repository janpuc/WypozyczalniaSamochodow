using AwesomeAssertions;

using NSubstitute;
using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Insurance;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Presentation;
using WypozyczalniaSamochodow.App.Presentation.Formating;
using WypozyczalniaSamochodow.Tests.TestSupport;
using Xunit;
namespace WypozyczalniaSamochodow.Tests.Presentation;

public sealed class UiRendererTests
{
    private static Vehicle CreateVehicle()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", new RegistrationNumber("kr123"), new Vin("vin123"), "Silver", new Money(100m), 2022, new DateOnly(2024, 1, 1));
        vehicle.AddInsurance(new Insurance("PZU", new PolicyNumber("POL-1"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m)));
        return vehicle;
    }

    private static Reservation CreateReservation(Client client, Vehicle vehicle)
        => new(client, vehicle, DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), new FixedClock(new DateOnly(2026, 5, 25)));

    [Fact]
    public void RendererFormatsStatusesAndTables()
    {
        using var scope = new TestConsoleScope();
        var reservations = Substitute.For<IReservationRepository>();
        var ui = new UiRenderer(new TextStyler(), new DomainViewFormatter(new TextStyler(), new FixedClock(new DateOnly(2026, 5, 25))));
        var vehicle = CreateVehicle();
        var client = new Client("Jan", new Email("jan@example.com"), Password.FromHash("hash"), new DrivingLicence("ABC", new DateOnly(2026, 12, 31)));
        var reservation = CreateReservation(client, vehicle);
        reservation.Activate(100, ScreenTestData.Clock);
        var maintenance = new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 12)), "Serwis");
        vehicle.AddEvent(maintenance);

        ui.FormatDate(null).Should().Be("—");
        ui.EventLabel(maintenance).Should().Be("Serwis");
        ui.VehicleStatus(vehicle).Should().Contain("Dostępny");
        ui.ReservationStatus(reservation).Should().Contain("Aktywna");
        ui.PaymentLabel(reservation.Payment).Should().Be("Gotówka");

        var table = ui.CreateDetailsTable();
        ui.AddVehicleRows(table, vehicle);
        ui.AddReservationRows(table, reservation);
        ui.Render(table);

        scope.Console.Output.Should().Contain("Toyota").And.Contain("Jan");
    }

    [Fact]
    public void RendererWaitsForKeyAndRendersMessages()
    {
        using var scope = new TestConsoleScope();
        var reservations = Substitute.For<IReservationRepository>();
        var ui = new UiRenderer(new TextStyler(), new DomainViewFormatter(new TextStyler(), new FixedClock(new DateOnly(2026, 5, 25))));

        scope.Console.Input.PushKey(ConsoleKey.Enter);
        ui.WaitForKey();

        scope.Console.Input.PushKey(ConsoleKey.Enter);
        ui.Success("OK");

        scope.Console.Input.PushKey(ConsoleKey.Enter);
        ui.Error("Błąd");

        scope.Console.Output.Should().Contain("OK").And.Contain("Błąd");
    }

    [Fact]
    public void RendererFormatsEventsAndTabs()
    {
        using var scope = new TestConsoleScope();
        var reservations = Substitute.For<IReservationRepository>();
        var ui = new UiRenderer(new TextStyler(), new DomainViewFormatter(new TextStyler(), new FixedClock(new DateOnly(2026, 5, 25))));
        var reservation = new Reservation(new Client("Jan", new Email("jan@example.com"), Password.FromHash("hash"), new DrivingLicence("ABC", new DateOnly(2026, 12, 31))), CreateVehicle(), DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), new FixedClock(new DateOnly(2026, 5, 25)));
        reservation.Activate(100, ScreenTestData.Clock);
        var broken = new BrokenDownEvent(DateRange.OpenEnded(new DateOnly(2026, 5, 20)), "Awaria");
        var repair = broken.RegisterRepair(DateRange.Closed(new DateOnly(2026, 5, 25), new DateOnly(2026, 5, 26)), "Naprawa");
        var maintenance = new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 6, 14), new DateOnly(2026, 6, 15)), "Serwis");
        var inspection = new InspectionEvent(DateRange.Closed(new DateOnly(2026, 6, 16), new DateOnly(2026, 6, 17)), "Przegląd");
        var detailing = new DetailingEvent(DateRange.Closed(new DateOnly(2026, 6, 18), new DateOnly(2026, 6, 19)), "Detailing");
        var suspended = new SuspendedEvent(new DateOnly(2026, 6, 20), "Wstrzymany");
        var reservationEvent = new ReservationEvent(DateRange.Closed(new DateOnly(2026, 6, 21), new DateOnly(2026, 6, 22)), "Rezerwacja");

        ui.DrawTabs(new[] { "A", "B" }, 1);
        ui.EventLabelColored(reservationEvent, true).Should().Contain("blue");
        ui.EventLabelColored(maintenance, true).Should().Contain("yellow");
        ui.EventLabelColored(broken, true).Should().Contain("red");
        ui.EventLabelColored(repair, true).Should().Contain("red");
        ui.EventLabelColored(inspection, true).Should().Contain("yellow");
        ui.EventLabelColored(detailing, true).Should().Contain("aqua");
        ui.EventLabelColored(suspended, true).Should().Contain("red");

        var vehicle = CreateVehicle();
        vehicle.AddEvent(broken);
        vehicle.AddEvent(repair);
        ui.VehicleStatus(vehicle).Should().Contain("Naprawa");

        ui.ReservationStatus(reservation).Should().Contain("Aktywna");
        ui.ReservationStatus(new Reservation(new Client("Jan", new Email("jan2@example.com"), Password.FromHash("hash"), new DrivingLicence("ABC", new DateOnly(2026, 12, 31))), CreateVehicle(), DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), new FixedClock(new DateOnly(2026, 5, 25)))).Should().Contain("Oczekująca");
    }
}
