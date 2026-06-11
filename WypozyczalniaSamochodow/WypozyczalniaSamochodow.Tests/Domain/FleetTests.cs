using AwesomeAssertions;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Insurance;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Domain;

public sealed class FleetTests
{
    private static Vehicle CreateVehicle()
    {
        var price = new Money(100m);
        return new Vehicle("Toyota", "Corolla", new RegistrationNumber("kr123"), new Vin("vin123"), "Silver", price, 2022, new DateOnly(2024, 1, 1));
    }

    [Fact]
    public void ValueObjectsValidateAndNormalize()
    {
        new Vin("vin123").Value.Should().Be("VIN123");
        new RegistrationNumber("kr 123").Value.Should().Be("KR 123");
        new PolicyNumber("ABC-123").Value.Should().Be("ABC-123");
    }

    [Fact]
    public void ValueObjectsRejectEmptyValues()
    {
        Action act1 = () => new Vin(" ");
        Action act2 = () => new RegistrationNumber("");
        Action act3 = () => new PolicyNumber(" ");

        act1.Should().Throw<DomainException>();
        act2.Should().Throw<DomainException>();
        act3.Should().Throw<DomainException>();
    }

    [Fact]
    public void InsuranceValidatesAndChecksDate()
    {
        var insurance = new Insurance("PZU", new PolicyNumber("POL-1"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m));

        insurance.IssueDate.Should().Be(new DateOnly(2026, 1, 1));
        insurance.ExpiryDate.Should().Be(new DateOnly(2026, 12, 31));
        insurance.IsValidOn(new DateOnly(2025, 12, 31)).Should().BeFalse();
        insurance.IsValidOn(new DateOnly(2026, 12, 31)).Should().BeTrue();
        insurance.IsValidOn(new DateOnly(2027, 1, 1)).Should().BeFalse();
        insurance.Covers(DateRange.Closed(new DateOnly(2026, 2, 1), new DateOnly(2026, 2, 3))).Should().BeTrue();
        insurance.Covers(DateRange.Closed(new DateOnly(2026, 12, 30), new DateOnly(2027, 1, 2))).Should().BeFalse();
    }

    [Fact]
    public void InsuranceRejectsInvalidInput()
    {
        Action act1 = () => new Insurance(" ", new PolicyNumber("POL-1"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m));
        Action act2 = () => new Insurance("PZU", new PolicyNumber("POL-1"), " ", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m));
        Action act3 = () => new Insurance("PZU", new PolicyNumber("POL-1"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 1), new Money(100m));

        act1.Should().Throw<DomainException>().WithMessage("Firma ubezpieczeniowa nie może być pusta.");
        act2.Should().Throw<DomainException>().WithMessage("Nazwa polisy nie może być pusta.");
        act3.Should().Throw<DomainException>().WithMessage("Data wygaśnięcia musi być po dacie wystawienia.");
    }

    [Fact]
    public void ScheduleDetectsConflictsAndQueriesActiveEvents()
    {
        var schedule = new Schedule();
        var reservation = new ReservationEvent(DateRange.Closed(new DateOnly(2026, 5, 10), new DateOnly(2026, 5, 12)));
        var maintenance = new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 5, 14), new DateOnly(2026, 5, 15)));

        schedule.Add(reservation);
        schedule.Add(maintenance);

        schedule.WouldConflict(new ReservationEvent(DateRange.Closed(new DateOnly(2026, 5, 11), new DateOnly(2026, 5, 13)))).Should().BeTrue();
        schedule.ConflictingNonReservationEvents(DateRange.Closed(new DateOnly(2026, 5, 14), new DateOnly(2026, 5, 15))).Should().ContainSingle();
        schedule.ActiveNonReservationOn(new DateOnly(2026, 5, 14)).Should().BeSameAs(maintenance);
        schedule.NonReservationEvents.Should().ContainSingle();

        schedule.Remove(maintenance);
        schedule.NonReservationEvents.Should().BeEmpty();
    }

    [Fact]
    public void ScheduleRejectsConflictingAdds()
    {
        var schedule = new Schedule();
        schedule.Add(new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 5, 10), new DateOnly(2026, 5, 12))));

        var act = () => schedule.Add(new InspectionEvent(DateRange.Closed(new DateOnly(2026, 5, 11), new DateOnly(2026, 5, 13))));

        act.Should().Throw<DomainException>().WithMessage("Nowe zdarzenie nakłada się na istniejące zdarzenie w harmonogramie.");
    }

    [Fact]
    public void VehicleSupportsMutationsAndAvailabilityRules()
    {
        var clock = new FixedClock(new DateOnly(2026, 5, 25));
        var vehicle = CreateVehicle();
        vehicle.AddInsurance(new Insurance("PZU", new PolicyNumber("POL-2"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m)));

        vehicle.Rename("Honda", "Civic");
        vehicle.ChangeRegistration(new RegistrationNumber("kr999"));
        vehicle.ChangeVin(new Vin("newvin"));
        vehicle.Repaint("Black");
        vehicle.Reprice(new Money(150m));
        vehicle.SetYear(2023);
        vehicle.SetPurchaseDate(new DateOnly(2024, 2, 2));

        vehicle.Make.Should().Be("Honda");
        vehicle.Model.Should().Be("Civic");
        vehicle.Registration.Value.Should().Be("KR999");
        vehicle.Vin.Value.Should().Be("NEWVIN");
        vehicle.Color.Should().Be("Black");
        vehicle.PricePerDay.Value.Should().Be(150m);
        vehicle.Year.Should().Be(2023);
        vehicle.PurchaseDate.Should().Be(new DateOnly(2024, 2, 2));
        vehicle.HasActiveInsuranceOn(new DateOnly(2026, 5, 25)).Should().BeTrue();
        vehicle.IsAvailableFor(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), clock).Should().BeTrue();

        var eventRange = DateRange.Closed(new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 12));
        var eventReservation = new ReservationEvent(eventRange);
        vehicle.AddEvent(eventReservation);
        vehicle.IsAvailableFor(eventRange, clock).Should().BeFalse();

        var act = () => vehicle.RemoveEvent(eventReservation);
        act.Should().Throw<DomainException>().WithMessage("Nie można usunąć zdarzenia typu Rezerwacja z tego widoku.");
    }

    [Fact]
    public void VehicleCanRemoveNonReservationEvent()
    {
        var vehicle = CreateVehicle();
        var maintenance = new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 12)));
        vehicle.AddEvent(maintenance);

        vehicle.RemoveEvent(maintenance);

        vehicle.Schedule.NonReservationEvents.Should().BeEmpty();
    }

    [Fact]
    public void VehicleRequiresInsuranceForWholeReservationPeriod()
    {
        var clock = new FixedClock(new DateOnly(2026, 5, 25));
        var vehicle = CreateVehicle();
        vehicle.AddInsurance(new Insurance("PZU", new PolicyNumber("POL-2"), "OC", new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 2), new Money(100m)));

        vehicle.IsAvailableFor(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), clock).Should().BeFalse();

        vehicle.AddInsurance(new Insurance("PZU", new PolicyNumber("POL-3"), "OC", new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), new Money(100m)));

        vehicle.IsAvailableFor(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), clock).Should().BeTrue();
    }

    [Fact]
    public void RemovingInsuranceThatWouldLeaveAReservationUninsuredIsRejected()
    {
        var clock = new FixedClock(new DateOnly(2026, 5, 25));
        var vehicle = CreateVehicle();
        var policy = new Insurance("PZU", new PolicyNumber("POL-9"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m));
        vehicle.AddInsurance(policy);
        vehicle.Reserve(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), clock);

        Action act = () => vehicle.RemoveInsurance(policy);

        act.Should().Throw<DomainException>().WithMessage("Nie można usunąć polisy — rezerwacja straciłaby pokrycie ubezpieczeniem.");
        vehicle.Insurances.Should().Contain(policy);
    }

    [Fact]
    public void RemovingRedundantInsuranceIsAllowedWhenAnotherStillCoversTheReservation()
    {
        var clock = new FixedClock(new DateOnly(2026, 5, 25));
        var vehicle = CreateVehicle();
        var primary = new Insurance("PZU", new PolicyNumber("POL-A"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m));
        var backup = new Insurance("Warta", new PolicyNumber("POL-B"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(120m));
        vehicle.AddInsurance(primary);
        vehicle.AddInsurance(backup);
        vehicle.Reserve(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), clock);

        vehicle.RemoveInsurance(backup);

        vehicle.Insurances.Should().ContainSingle().Which.Should().BeSameAs(primary);
    }

    [Fact]
    public void ReserveRejectsVehicleWithoutInsuranceCoverForThePeriod()
    {
        var clock = new FixedClock(new DateOnly(2026, 5, 25));
        var vehicle = CreateVehicle(); // brak jakiejkolwiek polisy

        var act = () => vehicle.Reserve(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), clock);

        act.Should().Throw<DomainException>().WithMessage("Pojazd nie jest dostępny w wybranym terminie.");
    }

    [Fact]
    public void RescheduleReservationRejectsTermNotCoveredByInsurance()
    {
        var clock = new FixedClock(new DateOnly(2026, 5, 25));
        var vehicle = CreateVehicle();
        vehicle.AddInsurance(new Insurance("PZU", new PolicyNumber("POL-7"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m)));
        var reservation = vehicle.Reserve(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), clock);

        var act = () => vehicle.RescheduleReservation(reservation, DateRange.Closed(new DateOnly(2027, 1, 1), new DateOnly(2027, 1, 3)));

        act.Should().Throw<DomainException>().WithMessage("Brak ubezpieczenia pokrywającego nowy termin rezerwacji.");
    }

    [Fact]
    public void ConflictPolicyTreatsSelfAndLinkedBreakdownRepairPairAsNonConflicting()
    {
        var broken = new BrokenDownEvent(DateRange.OpenEnded(new DateOnly(2026, 6, 4)), "Awaria");
        var repair = broken.RegisterRepair(DateRange.Closed(new DateOnly(2026, 6, 5), new DateOnly(2026, 6, 7)), "Naprawa");
        var maintenance = new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 2)), "Serwis");
        var farBroken = new BrokenDownEvent(DateRange.Closed(new DateOnly(2026, 8, 1), new DateOnly(2026, 8, 2)), "Awaria 2");

        broken.ConflictsWith(broken).Should().BeFalse();
        repair.ConflictsWith(repair).Should().BeFalse();
        maintenance.ConflictsWith(maintenance).Should().BeFalse();

        repair.ConflictsWith(broken).Should().BeFalse();
        broken.ConflictsWith(repair).Should().BeFalse();

        maintenance.ConflictsWith(farBroken).Should().BeFalse();

        var otherBroken = new BrokenDownEvent(DateRange.OpenEnded(new DateOnly(2026, 6, 4)), "Awaria 3");
        var otherRepair = otherBroken.RegisterRepair(DateRange.Closed(new DateOnly(2026, 6, 5), new DateOnly(2026, 6, 7)), "Naprawa 3");
        broken.ConflictsWith(otherRepair).Should().BeTrue();
    }

    [Fact]
    public void RepairEventRequiresEndDateAndCause()
    {
        var broken = new BrokenDownEvent(DateRange.OpenEnded(new DateOnly(2026, 6, 4)), "Awaria");

        Action openEnded = () => new RepairEvent(DateRange.OpenEnded(new DateOnly(2026, 6, 5)), broken, "Naprawa");
        Action nullCause = () => new RepairEvent(DateRange.Closed(new DateOnly(2026, 6, 5), new DateOnly(2026, 6, 7)), null!, "Naprawa");

        openEnded.Should().Throw<DomainException>().WithMessage("Naprawa musi mieć datę zakończenia.");
        nullCause.Should().Throw<DomainException>().WithMessage("Naprawa musi być powiązana z awarią.");
    }

    [Fact]
    public void VehicleConstructorRejectsInvalidFields()
    {
        var reg = new RegistrationNumber("kr123");
        var vin = new Vin("vin123");
        var date = new DateOnly(2024, 1, 1);

        Action emptyModel = () => new Vehicle("Toyota", " ", reg, vin, "Silver", new Money(100m), 2022, date);
        Action emptyColor = () => new Vehicle("Toyota", "Corolla", reg, vin, " ", new Money(100m), 2022, date);
        Action zeroPrice = () => new Vehicle("Toyota", "Corolla", reg, vin, "Silver", new Money(0m), 2022, date);
        Action invalidYear = () => new Vehicle("Toyota", "Corolla", reg, vin, "Silver", new Money(100m), 1899, date);

        emptyModel.Should().Throw<DomainException>().WithMessage("Model nie może być pusty.");
        emptyColor.Should().Throw<DomainException>().WithMessage("Kolor nie może być pusty.");
        zeroPrice.Should().Throw<DomainException>().WithMessage("Cena za dzień musi być większa od zera.");
        invalidYear.Should().Throw<DomainException>().WithMessage("Rok produkcji jest nieprawidłowy.");
    }

    [Fact]
    public void VehicleWithActiveBlockingEventTodayIsNotAvailable()
    {
        var clock = new FixedClock(new DateOnly(2026, 5, 25));
        var vehicle = CreateVehicle();
        vehicle.AddInsurance(new Insurance("PZU", new PolicyNumber("POL-5"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m)));
        vehicle.AddEvent(new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 5, 24), new DateOnly(2026, 5, 26)), "Serwis"));

        vehicle.IsAvailableFor(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), clock).Should().BeFalse();
    }

    [Fact]
    public void BrokenDownRepairAndOtherEventsBehaveAsExpected()
    {
        var broken = new BrokenDownEvent(DateRange.OpenEnded(new DateOnly(2026, 5, 20)), "Awaria");
        var repair = broken.RegisterRepair(DateRange.Closed(new DateOnly(2026, 5, 21), new DateOnly(2026, 5, 23)), "Naprawa");

        broken.LinkedRepair.Should().BeSameAs(repair);
        broken.Period.To.Should().Be(new DateOnly(2026, 5, 21));
        repair.Cause.Should().BeSameAs(broken);
        repair.Describe().Should().Be("Naprawa");
        broken.Describe().Should().Be("Niesprawny");

        Action act4 = () => broken.RegisterRepair(DateRange.OpenEnded(new DateOnly(2026, 5, 21)));
        Action actBeforeFailure = () => broken.RegisterRepair(DateRange.Closed(new DateOnly(2026, 5, 19), new DateOnly(2026, 5, 23)));
        Action act5 = () => new ReservationEvent(DateRange.OpenEnded(new DateOnly(2026, 5, 21)));
        Action act6 = () => new MaintenanceEvent(DateRange.OpenEnded(new DateOnly(2026, 5, 21)));
        Action act7 = () => new InspectionEvent(DateRange.OpenEnded(new DateOnly(2026, 5, 21)));
        Action act8 = () => new DetailingEvent(DateRange.OpenEnded(new DateOnly(2026, 5, 21)));

        act4.Should().Throw<DomainException>();
        actBeforeFailure.Should().Throw<DomainException>().WithMessage("Naprawa nie może zaczynać się przed awarią.");
        act5.Should().Throw<DomainException>();
        act6.Should().Throw<DomainException>();
        act7.Should().Throw<DomainException>();
        act8.Should().Throw<DomainException>();
    }
}
