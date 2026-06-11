using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;

namespace WypozyczalniaSamochodow.App.Infrastructure.Persistence;

internal sealed class InMemoryVehicleRepository : InMemoryRepository<Vehicle>, IVehicleRepository
{
}
