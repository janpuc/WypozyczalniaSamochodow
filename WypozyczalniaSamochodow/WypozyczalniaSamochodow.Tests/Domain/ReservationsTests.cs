using AwesomeAssertions;
using NSubstitute;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.Tests.TestSupport;
using Xunit;

namespace WypozyczalniaSamochodow.Tests.Domain;

public sealed class ReservationsTests
{
    private static Client CreateClient(bool withValidLicence = true)
    {
        var hasher = Substitute.For<WypozyczalniaSamochodow.App.Domain.Users.IPasswordHasher>();
        hasher.Hash("secret").Returns("hash");
        return new Client("Jan Kowalski", new Email("jan@example.com"), Password.FromPlain("secret", hasher),
            withValidLicence ? new DrivingLicence("ABC", new DateOnly(2026, 12, 31)) : null);
    }

    private static Vehicle CreateVehicle()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", new RegistrationNumber("kr123"), new Vin("vin123"), "Silver", new Money(100m), 2022, new DateOnly(2024, 1, 1));
        vehicle.AddInsurance(new WypozyczalniaSamochodow.App.Domain.Insurance.Insurance("PZU", new WypozyczalniaSamochodow.App.Domain.Insurance.PolicyNumber("POL-1"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m)));
        return vehicle;
    }

    private static FixedClock Clock => new(new DateOnly(2026, 5, 25));

    [Fact]
    public void ReservationConstructorValidatesAndInitializesStatus()
    {
        var client = CreateClient();
        var vehicle = CreateVehicle();

        var reservation = new Reservation(client, vehicle, DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);

        reservation.Client.Should().BeSameAs(client);
        reservation.Vehicle.Should().BeSameAs(vehicle);
        reservation.Status.Should().BeOfType<PendingReservation>();
        reservation.BelongsTo(client).Should().BeTrue();
        reservation.BelongsTo(vehicle).Should().BeTrue();
    }

    [Fact]
    public void ReservationConstructorRejectsInvalidInput()
    {
        var client = CreateClient();
        var vehicle = CreateVehicle();

        Action act1 = () => new Reservation(client, vehicle, DateRange.OpenEnded(new DateOnly(2026, 6, 1)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);
        Action act2 = () => new Reservation(client, vehicle, DateRange.Closed(new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 3)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);
        var noLicence = CreateClient(withValidLicence: false);
        Action act3 = () => new Reservation(noLicence, vehicle, DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);

        act1.Should().Throw<DomainException>().WithMessage("Rezerwacja musi mieć datę zakończenia.");
        act2.Should().Throw<DomainException>().WithMessage("Data rozpoczęcia nie może być w przeszłości.");
        act3.Should().Throw<DomainException>();
    }

    [Fact]
    public void ReservationSupportsStateTransitions()
    {
        var client = CreateClient();
        var vehicle = CreateVehicle();
        var reservation = new Reservation(client, vehicle, DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);

        reservation.Activate(1000, Clock);
        reservation.Status.Should().BeOfType<ActiveReservation>();
        reservation.Status.MileageBefore.Should().Be(1000);

        reservation.Complete(1050, "ok");
        reservation.Status.Should().BeOfType<CompletedReservation>();
        reservation.Status.MileageAfter.Should().Be(1050);
        reservation.Status.CompletionNote.Should().Be("ok");

        var cancelled = new Reservation(client, vehicle, DateRange.Closed(new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 12)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);
        cancelled.Cancel();
        cancelled.Status.Should().BeOfType<CancelledReservation>();
        cancelled.Status.Label.Should().Be("Anulowana");
    }

    [Fact]
    public void CancellingReservationReleasesVehicleSchedule()
    {
        var client = CreateClient();
        var vehicle = CreateVehicle();
        var period = DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3));
        var reservation = new Reservation(client, vehicle, period, new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);

        vehicle.IsAvailableFor(period, Clock).Should().BeFalse();

        reservation.Cancel();

        vehicle.IsAvailableFor(period, Clock).Should().BeTrue();
    }

    [Fact]
    public void ActivateRejectsClientWhoseLicenceExpiredBetweenBookingAndPickup()
    {
        var hasher = Substitute.For<WypozyczalniaSamochodow.App.Domain.Users.IPasswordHasher>();
        hasher.Hash("secret").Returns("hash");
        var client = new Client("Jan Kowalski", new Email("jan@example.com"), Password.FromPlain("secret", hasher),
            new DrivingLicence("ABC", new DateOnly(2026, 5, 30)));
        var vehicle = CreateVehicle();
        var reservation = new Reservation(client, vehicle, DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)),
            new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);

        var act = () => reservation.Activate(1000, new FixedClock(new DateOnly(2026, 6, 1)));

        act.Should().Throw<DomainException>().WithMessage("Prawo jazdy klienta jest nieważne.");
        reservation.Status.Should().BeOfType<PendingReservation>();
    }

    [Fact]
    public void ReservationStateObjectsValidateTheirRules()
    {
        var pending = new PendingReservation();
        pending.Label.Should().Be("Oczekująca");
        pending.MileageBefore.Should().BeNull();
        pending.Activate(0).Should().BeOfType<ActiveReservation>();
        pending.Cancel().Should().BeOfType<CancelledReservation>();

        Action act4 = () => pending.Activate(-1);
        act4.Should().Throw<DomainException>().WithMessage("Przebieg nie może być ujemny.");

        var active = new ActiveReservation(1000);
        active.Label.Should().Be("Aktywna");
        active.Complete(1000, null).Should().BeOfType<CompletedReservation>();
        Action actNegativeMileage = () => active.Complete(-1, null);
        Action act5 = () => active.Complete(999, null);
        actNegativeMileage.Should().Throw<DomainException>().WithMessage("Przebieg nie może być ujemny.");
        act5.Should().Throw<DomainException>().WithMessage("Przebieg końcowy nie może być mniejszy niż początkowy.");

        var completed = new CompletedReservation(100, 200, "done");
        completed.Label.Should().Be("Zakończona");
        completed.MileageBefore.Should().Be(100);
        completed.MileageAfter.Should().Be(200);
        completed.CompletionNote.Should().Be("done");
        Action act6 = () => completed.Activate(1);
        Action act7 = () => completed.Cancel();
        Action act8 = () => completed.Complete(1, null);
        act6.Should().Throw<DomainException>();
        act7.Should().Throw<DomainException>();
        act8.Should().Throw<DomainException>();
    }

    [Fact]
    public void ReservationCanRescheduleAndSwapVehicle()
    {
        var client = CreateClient();
        var vehicle = CreateVehicle();
        var reservation = new Reservation(client, vehicle, DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);

        reservation.Reschedule(DateRange.Closed(new DateOnly(2026, 6, 4), new DateOnly(2026, 6, 6)));
        reservation.Event.FromDate.Should().Be(new DateOnly(2026, 6, 4));

        var newVehicle = CreateVehicle();
        reservation.SwapVehicle(newVehicle, Clock);
        reservation.Vehicle.Should().BeSameAs(newVehicle);

        reservation.Activate(100, Clock);
        Action act9 = () => reservation.SwapVehicle(CreateVehicle(), Clock);
        act9.Should().Throw<DomainException>().WithMessage("Pojazd można wymienić tylko dla rezerwacji oczekującej.");

        var pending = new Reservation(client, vehicle, DateRange.Closed(new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 12)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);
        var unavailable = CreateVehicle();
        unavailable.AddEvent(new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 12))));
        Action act10 = () => pending.SwapVehicle(unavailable, Clock);
        act10.Should().Throw<DomainException>().WithMessage("Nowy pojazd nie jest dostępny w wybranym terminie.");
    }

    [Fact]
    public void ReservationRejectsOpenEndedReschedule()
    {
        var reservation = new Reservation(CreateClient(), CreateVehicle(),
            DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)),
            new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);

        var act = () => reservation.Reschedule(DateRange.OpenEnded(new DateOnly(2026, 6, 4)));

        act.Should().Throw<DomainException>().WithMessage("Rezerwacja musi mieć datę zakończenia.");
    }

    [Fact]
    public void RescheduleRejectsPeriodConflictingWithAnotherEvent()
    {
        var vehicle = CreateVehicle();
        vehicle.AddEvent(new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 6, 20), new DateOnly(2026, 6, 22))));
        var reservation = new Reservation(CreateClient(), vehicle,
            DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)),
            new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), Clock);

        var act = () => reservation.Reschedule(DateRange.Closed(new DateOnly(2026, 6, 21), new DateOnly(2026, 6, 23)));

        act.Should().Throw<DomainException>().WithMessage("Nowy termin nakłada się*");
        reservation.Event.FromDate.Should().Be(new DateOnly(2026, 6, 1));
    }
}
