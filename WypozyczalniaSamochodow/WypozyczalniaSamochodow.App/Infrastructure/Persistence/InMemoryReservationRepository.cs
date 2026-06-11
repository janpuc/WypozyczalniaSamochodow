using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Infrastructure.Persistence;

internal sealed class InMemoryReservationRepository : InMemoryRepository<Reservation>, IReservationRepository
{
    public IEnumerable<Reservation> OfClient(Client client) =>
        Items.Where(r => r.BelongsTo(client)).ToList();

    public IEnumerable<Reservation> OfVehicle(Vehicle vehicle) =>
        Items.Where(r => r.BelongsTo(vehicle)).ToList();

    public bool HasActiveOf(Client client) =>
        Items.Any(r => r.BelongsTo(client) && r.Status is PendingReservation or ActiveReservation);

    public bool HasActiveOf(Vehicle vehicle) =>
        Items.Any(r => r.BelongsTo(vehicle) && r.Status is PendingReservation or ActiveReservation);
}
