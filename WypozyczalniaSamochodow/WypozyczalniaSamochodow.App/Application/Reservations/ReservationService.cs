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
        throw new NotImplementedException();
    }

    public void Activate(Reservation reservation, int mileageBefore) => throw new NotImplementedException();

    public void Complete(Reservation reservation, int mileageAfter, string? note) => throw new NotImplementedException();

    public void Cancel(Reservation reservation) => throw new NotImplementedException();

    public void SwapVehicle(Reservation reservation, Vehicle newVehicle) => throw new NotImplementedException();

    public IReadOnlyList<Vehicle> AvailableVehicles(DateRange period, Vehicle? excluding = null) => throw new NotImplementedException();
}

