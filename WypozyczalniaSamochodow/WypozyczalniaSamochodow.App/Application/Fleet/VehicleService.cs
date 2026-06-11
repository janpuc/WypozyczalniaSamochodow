using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Insurance;

namespace WypozyczalniaSamochodow.App.Application.Fleet;

internal sealed class VehicleService
{
    private readonly IVehicleRepository _vehicles;

    public VehicleService(IVehicleRepository vehicles) => throw new NotImplementedException();

    public void Add(Vehicle vehicle) => throw new NotImplementedException();

    public void Remove(Vehicle vehicle) => throw new NotImplementedException();

    public void AddInsurance(Vehicle vehicle, Insurance insurance) => throw new NotImplementedException();

    public void RemoveInsurance(Vehicle vehicle, Insurance insurance) => throw new NotImplementedException();

    public void ScheduleEvent(Vehicle vehicle, VehicleEvent vehicleEvent) => throw new NotImplementedException();

    public void RemoveEvent(Vehicle vehicle, VehicleEvent vehicleEvent) => throw new NotImplementedException();
}

