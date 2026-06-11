using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Application.Reservations;

internal sealed class ReservationService
{
    private readonly IReservationRepository _reservations;
    private readonly IVehicleRepository _vehicles;
    private readonly IClock _clock;

    public ReservationService(IReservationRepository reservations, IVehicleRepository vehicles, IClock clock)
    {
        _reservations = reservations;
        _vehicles = vehicles;
        _clock = clock;
    }

    public Reservation Create(Client client, Vehicle vehicle, DateRange period, Payment payment)
    {
        var reservation = new Reservation(client, vehicle, period, payment, _clock);
        _reservations.Add(reservation);
        return reservation;
    }

    public void Activate(Reservation reservation, int mileageBefore) => reservation.Activate(mileageBefore, _clock);

    public void Complete(Reservation reservation, int mileageAfter, string? note) =>
        reservation.Complete(mileageAfter, note);

    public void Cancel(Reservation reservation) => reservation.Cancel();

    public void SwapVehicle(Reservation reservation, Vehicle newVehicle) =>
        reservation.SwapVehicle(newVehicle, _clock);

    public IReadOnlyList<Vehicle> AvailableVehicles(DateRange period, Vehicle? excluding = null) =>
        _vehicles.All
            .Where(v => !ReferenceEquals(v, excluding) && v.IsAvailableFor(period, _clock))
            .ToList();
}
