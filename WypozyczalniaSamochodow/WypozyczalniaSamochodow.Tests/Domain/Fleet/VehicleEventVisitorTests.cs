using AwesomeAssertions;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Shared;
using Xunit;

namespace WypozyczalniaSamochodow.Tests.Domain.Fleet;

public sealed class VehicleEventVisitorTests
{
    [Fact]
    public void VisitorsMapEventTypesAndPolicies()
    {
        var reservation = new ReservationEvent(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), "Rezerwacja");
        var broken = new BrokenDownEvent(DateRange.OpenEnded(new DateOnly(2026, 6, 4)), "Awaria");
        var repair = new RepairEvent(DateRange.Closed(new DateOnly(2026, 6, 5), new DateOnly(2026, 6, 7)), broken, "Naprawa");
        var maintenance = new MaintenanceEvent(DateRange.Closed(new DateOnly(2026, 6, 8), new DateOnly(2026, 6, 9)), "Serwis");
        var inspection = new InspectionEvent(DateRange.Closed(new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 11)), "Przegląd");
        var detailing = new DetailingEvent(DateRange.Closed(new DateOnly(2026, 6, 12), new DateOnly(2026, 6, 13)), "Detailing");
        var suspended = new SuspendedEvent(new DateOnly(2026, 7, 2), "Wstrzymany");

        var canRemove = new CanRemoveVisitor();
        canRemove.Visit(reservation).Should().BeFalse();
        canRemove.Visit(broken).Should().BeTrue();
        canRemove.Visit(repair).Should().BeTrue();
        canRemove.Visit(maintenance).Should().BeTrue();
        canRemove.Visit(inspection).Should().BeTrue();
        canRemove.Visit(detailing).Should().BeTrue();
        canRemove.Visit(suspended).Should().BeTrue();

        var isReservation = new IsReservationVisitor();
        isReservation.Visit(reservation).Should().BeTrue();
        isReservation.Visit(broken).Should().BeFalse();
        isReservation.Visit(repair).Should().BeFalse();
        isReservation.Visit(maintenance).Should().BeFalse();
        isReservation.Visit(inspection).Should().BeFalse();
        isReservation.Visit(detailing).Should().BeFalse();
        isReservation.Visit(suspended).Should().BeFalse();

        var subject = new RepairEvent(DateRange.Closed(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 3)), broken, "Naprawa 2");
        var policy = new ConflictPolicyVisitor(subject);
        policy.Visit(reservation).Should().BeFalse();
        policy.Visit(new ReservationEvent(DateRange.Closed(new DateOnly(2026, 7, 2), new DateOnly(2026, 7, 4)))).Should().BeTrue();
        policy.Visit(broken).Should().BeFalse();
        policy.Visit(new BrokenDownEvent(DateRange.Closed(new DateOnly(2026, 7, 2), new DateOnly(2026, 7, 4)), "Awaria 2")).Should().BeTrue();
        policy.Visit(repair).Should().BeFalse();
        policy.Visit(new RepairEvent(DateRange.Closed(new DateOnly(2026, 7, 2), new DateOnly(2026, 7, 4)), broken, "Naprawa 3")).Should().BeTrue();
        policy.Visit(maintenance).Should().BeFalse();
        policy.Visit(inspection).Should().BeFalse();
        policy.Visit(detailing).Should().BeFalse();
        policy.Visit(suspended).Should().BeTrue();
    }

    [Fact]
    public void EventsDescribeExposeEffectiveToAndAllowInternalMutation()
    {
        var reservation = new ReservationEvent(DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)));
        var inspection = new InspectionEvent(DateRange.Closed(new DateOnly(2026, 6, 4), new DateOnly(2026, 6, 5)));
        var detailing = new DetailingEvent(DateRange.Closed(new DateOnly(2026, 6, 6), new DateOnly(2026, 6, 7)));
        var suspended = new SuspendedEvent(new DateOnly(2026, 6, 8), "Opis");

        reservation.Describe().Should().Be("Rezerwacja");
        inspection.Describe().Should().Be("Przegląd pojazdu");
        detailing.Describe().Should().Be("Detailing pojazdu");
        suspended.Describe().Should().Be("Wstrzymany");
        suspended.EffectiveTo.Should().Be(DateOnly.MaxValue);

        suspended.UpdateDescription("Nowy opis");
        suspended.Description.Should().Be("Nowy opis");
    }

}
