using WypozyczalniaSamochodow.App.Domain.Fleet;

namespace WypozyczalniaSamochodow.App.Application.Repositories;

internal interface IVehicleRepository
{
    IReadOnlyList<Vehicle> All { get; }
    void Add(Vehicle vehicle);
    void Remove(Vehicle vehicle);
}
