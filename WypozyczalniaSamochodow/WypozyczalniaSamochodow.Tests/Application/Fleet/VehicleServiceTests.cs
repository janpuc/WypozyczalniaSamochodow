using AwesomeAssertions;

using NSubstitute;

using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Insurance;
using WypozyczalniaSamochodow.App.Domain.Shared;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Application;

public sealed class VehicleServiceTests
{
    private static Vehicle CreateVehicle()
    {
        var price = new Money(100m);
        return new Vehicle("Toyota", "Corolla", new RegistrationNumber("kr123"), new Vin("vin123"), "Silver", price, 2022, new DateOnly(2024, 1, 1));
    }

    private static Insurance CreateInsurance(
        DateOnly? issueDate = null,
        DateOnly? expiryDate = null,
        string number = "POL-1") =>
        new("PZU", new PolicyNumber(number), "OC",
            issueDate ?? new DateOnly(2026, 1, 1),
            expiryDate ?? new DateOnly(2026, 12, 31),
            new Money(100m));

    [Fact]
    public void AddDelegatesToRepository()
    {
        var vehicles = Substitute.For<IVehicleRepository>();
        var sut = new VehicleService(vehicles);
        var vehicle = CreateVehicle();

        sut.Add(vehicle);

        vehicles.Received(1).Add(vehicle);
    }

    [Fact]
    public void RemoveDelegatesToRepository()
    {
        var vehicles = Substitute.For<IVehicleRepository>();
        var sut = new VehicleService(vehicles);
        var vehicle = CreateVehicle();

        sut.Remove(vehicle);

        vehicles.Received(1).Remove(vehicle);
    }

    [Fact]
    public void AddInsuranceAttachesInsuranceToVehicle()
    {
        var sut = new VehicleService(Substitute.For<IVehicleRepository>());
        var vehicle = CreateVehicle();
        var insurance = CreateInsurance();

        sut.AddInsurance(vehicle, insurance);

        vehicle.Insurances.Should().ContainSingle().Which.Should().BeSameAs(insurance);
    }

    [Fact]
    public void RemoveInsuranceDetachesInsuranceFromVehicle()
    {
        var sut = new VehicleService(Substitute.For<IVehicleRepository>());
        var vehicle = CreateVehicle();
        var insurance = CreateInsurance();
        sut.AddInsurance(vehicle, insurance);

        sut.RemoveInsurance(vehicle, insurance);

        vehicle.Insurances.Should().BeEmpty();
    }

    [Fact]
    public void RemoveInsuranceThrowsWhenItWouldLeaveReservationUninsured()
    {
        var sut = new VehicleService(Substitute.For<IVehicleRepository>());
        var vehicle = CreateVehicle();
        var insurance = CreateInsurance();
        sut.AddInsurance(vehicle, insurance);
        sut.ScheduleEvent(vehicle, new ReservationEvent(
            DateRange.Closed(new DateOnly(2026, 2, 1), new DateOnly(2026, 2, 10))));

        Action act = () => sut.RemoveInsurance(vehicle, insurance);

        act.Should().Throw<DomainException>();
        vehicle.Insurances.Should().Contain(insurance);
    }

    [Fact]
    public void ScheduleEventAddsEventToSchedule()
    {
        var sut = new VehicleService(Substitute.For<IVehicleRepository>());
        var vehicle = CreateVehicle();
        var maintenance = new MaintenanceEvent(
            DateRange.Closed(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 5)));

        sut.ScheduleEvent(vehicle, maintenance);

        vehicle.Schedule.Events.Should().ContainSingle().Which.Should().BeSameAs(maintenance);
    }

    [Fact]
    public void ScheduleEventThrowsWhenEventOverlapsExisting()
    {
        var sut = new VehicleService(Substitute.For<IVehicleRepository>());
        var vehicle = CreateVehicle();
        sut.ScheduleEvent(vehicle, new MaintenanceEvent(
            DateRange.Closed(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 10))));

        Action act = () => sut.ScheduleEvent(vehicle, new MaintenanceEvent(
            DateRange.Closed(new DateOnly(2026, 3, 5), new DateOnly(2026, 3, 15))));

        act.Should().Throw<DomainException>();
        vehicle.Schedule.Events.Should().ContainSingle();
    }

    [Fact]
    public void RemoveEventRemovesNonReservationEvent()
    {
        var sut = new VehicleService(Substitute.For<IVehicleRepository>());
        var vehicle = CreateVehicle();
        var maintenance = new MaintenanceEvent(
            DateRange.Closed(new DateOnly(2026, 3, 1), new DateOnly(2026, 3, 5)));
        sut.ScheduleEvent(vehicle, maintenance);

        sut.RemoveEvent(vehicle, maintenance);

        vehicle.Schedule.Events.Should().BeEmpty();
    }

    [Fact]
    public void RemoveEventThrowsForReservationEvent()
    {
        var sut = new VehicleService(Substitute.For<IVehicleRepository>());
        var vehicle = CreateVehicle();
        var reservation = new ReservationEvent(
            DateRange.Closed(new DateOnly(2026, 2, 1), new DateOnly(2026, 2, 10)));
        sut.ScheduleEvent(vehicle, reservation);

        Action act = () => sut.RemoveEvent(vehicle, reservation);

        act.Should().Throw<DomainException>();
        vehicle.Schedule.Events.Should().Contain(reservation);
    }
}
