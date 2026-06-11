using WypozyczalniaSamochodow.App.Application.Reservations;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

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
        _reservation = reservation; _ui = ui; _prompts = prompts; _reservations = reservations;
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
            _ui.Render(t);
            _ui.WriteLine();

            var actions = new List<HintItem>();
            if (_reservation.CanActivate) actions.Add((Keys.Activate, UiStrings.HintActivate));
            if (_reservation.CanSwapVehicle) actions.Add((Keys.SwapVehicle, UiStrings.HintSwapVehicle));
            if (_reservation.CanCancel) actions.Add((Keys.Delete, UiStrings.HintCancel));
            if (_reservation.CanComplete) actions.Add((Keys.Complete, UiStrings.HintComplete));
            actions.Add((Keys.Back, UiStrings.HintBack));
            _ui.Hint(actions.ToArray());

            var key = _ui.ReadKey();
            if (Keys.Back.Matches(key)) return;

            if (Keys.Activate.Matches(key) && _reservation.CanActivate)
            {
                _ui.Guard(() =>
                {
                    var mileage = _prompts.PromptInt(UiStrings.PromptMileageStart);
                    _reservations.Activate(_reservation, mileage);
                    _ui.Success(UiStrings.ReservationActivated);
                });
            }
            if (Keys.Complete.Matches(key) && _reservation.CanComplete)
            {
                _ui.Guard(() =>
                {
                    var mileage = _prompts.PromptInt(UiStrings.PromptMileageEnd);
                    var note = _prompts.PromptText(UiStrings.PromptDescriptionOptional, allowEmpty: true);
                    _reservations.Complete(_reservation, mileage, note.NullIfBlank());
                    _ui.Success(UiStrings.ReservationCompleted);
                });
            }
            if (Keys.Delete.Matches(key) && _reservation.CanCancel)
            {
                if (_ui.ConfirmCancel(UiStrings.EntityReservation, _reservation.Vehicle.Make))
                    _ui.Guard(() => { _reservations.Cancel(_reservation); _ui.Success(UiStrings.Cancelled); });
            }
            if (Keys.SwapVehicle.Matches(key) && _reservation.CanSwapVehicle)
                SwapVehicle();
        }
    }

    private void SwapVehicle()
    {
        var candidates = _reservations.AvailableVehicles(_reservation.Event.Period, _reservation.Vehicle);
        if (candidates.Count == 0) { _ui.Error(UiStrings.NoVehiclesAvailable); return; }

        int idx = 0;
        while (true)
        {
            _ui.Clear();
            _ui.Heading(UiStrings.TitleSwapVehicle);
            var t = SelectableList.Build(candidates, idx,
                new[] { UiStrings.Make, UiStrings.Model, UiStrings.Registration, UiStrings.PricePerDay },
                v => new[] { v.Make, v.Model, v.Registration.ToString(), UiFormats.Money(v.PricePerDay) });
            _ui.Render(t);
            _ui.Hint((Keys.Select, UiStrings.HintSelect), (Keys.Back, UiStrings.HintCancel));

            var key = _ui.ReadKey();
            if (Keys.Back.Matches(key)) return;
            if (Keys.Down.Matches(key) && idx < candidates.Count - 1) { idx++; continue; }
            if (Keys.Up.Matches(key) && idx > 0) { idx--; continue; }
            if (Keys.Select.Matches(key))
            {
                _ui.Guard(() => { _reservations.SwapVehicle(_reservation, candidates[idx]); _ui.Success(UiStrings.VehicleSwapped); });
                return;
            }
        }
    }
}

