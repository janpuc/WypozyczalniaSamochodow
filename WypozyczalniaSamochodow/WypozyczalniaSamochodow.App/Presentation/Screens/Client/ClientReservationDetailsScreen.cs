using WypozyczalniaSamochodow.App.Application.Reservations;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Client;

internal sealed class ClientReservationDetailsScreen : IScreen
{
    private readonly Reservation _reservation;
    private readonly IUiRenderer _ui;
    private readonly ReservationService _reservations;

    public ClientReservationDetailsScreen(Reservation reservation, IUiRenderer ui, ReservationService reservations)
    {
        _reservation = reservation; _ui = ui; _reservations = reservations;
    }

    public void Run()
    {
        throw new NotImplementedException();
    }
}

