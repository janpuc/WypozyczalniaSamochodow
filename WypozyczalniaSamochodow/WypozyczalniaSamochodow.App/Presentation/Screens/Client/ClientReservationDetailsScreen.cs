using WypozyczalniaSamochodow.App.Application.Reservations;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

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
        while (true)
        {
            _ui.Clear();
            _ui.Heading(UiStrings.TitleReservationDetails);
            _ui.WriteLine();

            var t = _ui.CreateDetailsTable();
            _ui.AddReservationRows(t, _reservation);
            t.AddRow(UiStrings.Amount, UiFormats.Money(_reservation.Payment.Amount));
            t.AddRow(UiStrings.PaymentMethod, _reservation.Payment.Describe());
            t.AddRow(UiStrings.PaymentDate, _reservation.Payment.PaidAt.ToString(UiFormats.DateTime));
            _ui.Render(t);
            _ui.WriteLine();

            var canCancel = _reservation.CanCancel;
            if (canCancel)
                _ui.Hint((Keys.Delete, UiStrings.HintCancelReservation), (Keys.Back, UiStrings.HintBack));
            else
                _ui.Hint((Keys.Back, UiStrings.HintBack));

            var key = _ui.ReadKey();
            if (Keys.Back.Matches(key)) return;
            if (Keys.Delete.Matches(key) && canCancel)
            {
                if (_ui.ConfirmCancel(UiStrings.EntityReservation, _reservation.Vehicle.Make))
                    _ui.Guard(() => { _reservations.Cancel(_reservation); _ui.Success(UiStrings.Cancelled); });
            }
        }
    }
}
