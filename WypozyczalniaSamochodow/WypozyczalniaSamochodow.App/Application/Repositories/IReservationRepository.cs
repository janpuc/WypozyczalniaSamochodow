using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Application.Repositories;

internal interface IReservationRepository
{
    IReadOnlyList<Reservation> All { get; }
    IEnumerable<Reservation> OfClient(Client client);
    IEnumerable<Reservation> OfVehicle(Vehicle vehicle);
    bool HasActiveOf(Client client);
    bool HasActiveOf(Vehicle vehicle);
    void Add(Reservation reservation);
    void Remove(Reservation reservation);
}
