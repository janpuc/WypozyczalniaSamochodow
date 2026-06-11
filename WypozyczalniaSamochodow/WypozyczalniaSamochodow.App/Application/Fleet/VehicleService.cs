using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Insurance;

namespace WypozyczalniaSamochodow.App.Application.Fleet;

internal sealed class VehicleService
{
    private readonly IVehicleRepository _vehicles;

    public VehicleService(IVehicleRepository vehicles) => _vehicles = vehicles;

    public void Add(Vehicle vehicle) => _vehicles.Add(vehicle);

    public void Remove(Vehicle vehicle) => _vehicles.Remove(vehicle);

    public void AddInsurance(Vehicle vehicle, Insurance insurance) => vehicle.AddInsurance(insurance);

    public void RemoveInsurance(Vehicle vehicle, Insurance insurance) => vehicle.RemoveInsurance(insurance);

    public void ScheduleEvent(Vehicle vehicle, VehicleEvent vehicleEvent) => vehicle.AddEvent(vehicleEvent);

    public void RemoveEvent(Vehicle vehicle, VehicleEvent vehicleEvent) => vehicle.RemoveEvent(vehicleEvent);
}

