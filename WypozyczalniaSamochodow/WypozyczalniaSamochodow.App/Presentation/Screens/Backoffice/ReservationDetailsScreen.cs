using WypozyczalniaSamochodow.App.Application.Reservations;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;


internal sealed class ReservationDetailsScreen : IScreen
{
    private readonly Reservation _reservation;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly ReservationService _reservations;

    public ReservationDetailsScreen(Reservation reservation, IUiRenderer ui, IPrompts prompts,
        ReservationService reservations)
    {
        throw new NotImplementedException();
    }

    public void Run()
    {
        throw new NotImplementedException();
    }
}

