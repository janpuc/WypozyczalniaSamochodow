using WypozyczalniaSamochodow.App.Application.Reservations;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Client;

internal sealed class NewReservationFlow : IScreen
{
    private sealed record PaymentMethodOption(string Label, Func<Money, DateTime, Domain.Users.Client, IPrompts, Payment> Create);

    private static readonly PaymentMethodOption[] PaymentMethods =
    {
        new(UiStrings.PayCash, (amount, paidAt, _, _) => new CashPayment(amount, paidAt)),
        new(UiStrings.PayDebitCard, (amount, paidAt, _, prompts) => new DebitCardPayment(amount, paidAt, prompts.PromptText(UiStrings.PromptCardLast4))),
        new(UiStrings.PayBankTransfer, (amount, paidAt, _, prompts) => new BankTransferPayment(amount, paidAt, prompts.PromptText(UiStrings.PromptIban))),
        new(UiStrings.PayBitcoin, (amount, paidAt, _, prompts) => new BitcoinPayment(amount, paidAt, prompts.PromptText(UiStrings.PromptWalletAddress))),
        new(UiStrings.PayPayPal, (amount, paidAt, client, _) => new PayPalPayment(amount, paidAt, client.Email))
    };

    private readonly Domain.Users.Client _client;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly ReservationService _reservations;
    private readonly UserAccountService _users;
    private readonly IClock _clock;

    public NewReservationFlow(Domain.Users.Client client, IUiRenderer ui, IPrompts prompts,
        ReservationService reservations, UserAccountService users, IClock clock)
    {
        _client = client; _ui = ui; _prompts = prompts; _reservations = reservations; _users = users; _clock = clock;
    }

    public void Run()
    {
        var range = PromptDates();
        if (range is null) return;

        int selectedIndex = 0;
        while (true)
        {
            _ui.Clear();
            _ui.Heading(UiStrings.TitleNewReservationVehicle);
            _ui.Hint($"{range.From.ToString(UiFormats.Date)} – {range.To!.Value.ToString(UiFormats.Date)}",
                (Keys.ChangeDates, UiStrings.HintChangeDates));
            _ui.WriteLine();

            var candidates = _reservations.AvailableVehicles(range);
            if (selectedIndex >= candidates.Count) selectedIndex = Math.Max(0, candidates.Count - 1);

            DrawVehicleTable(candidates, selectedIndex);
            _ui.WriteLine();
            if (candidates.Count == 0) _ui.Line(UiStrings.NoVehiclesAvailable, UiRole.Error);
            _ui.Hint((Keys.Select, UiStrings.HintSelect), (Keys.ChangeDates, UiStrings.HintChangeDates), (Keys.Back, UiStrings.HintCancel));

            var key = _ui.ReadKey();
            if (Keys.Back.Matches(key)) return;
            if (Keys.ChangeDates.Matches(key)) { var nd = PromptDates(); if (nd is not null) { range = nd; selectedIndex = 0; } continue; }
            if (Keys.Down.Matches(key) && selectedIndex < candidates.Count - 1) { selectedIndex++; continue; }
            if (Keys.Up.Matches(key) && selectedIndex > 0) { selectedIndex--; continue; }
            if (Keys.Select.Matches(key) && candidates.Count > 0)
            {
                CreateReservation(candidates[selectedIndex], range);
                return;
            }
        }
    }

    private DateRange? PromptDates()
    {
        _ui.Clear();
        _ui.Heading(UiStrings.TitleChooseDates);
        var from = _prompts.PromptDate(UiStrings.From, notBefore: _clock.Today);
        var to = _prompts.PromptDate(UiStrings.To, notBefore: from);
        try { return DateRange.Closed(from, to); }
        catch (DomainException ex) { _ui.Error(ex.Message); return null; }
    }

    private void DrawVehicleTable(IReadOnlyList<Vehicle> vehicles, int selectedIndex)
    {
        var t = new UiTable();
        t.AddColumn(UiStrings.Make, UiRole.Accent).AddColumn(UiStrings.Model, UiRole.Accent).AddColumn(UiStrings.Year, UiRole.Accent)
            .AddColumn(UiStrings.Registration, UiRole.Accent).AddColumn(UiStrings.PricePerDay, UiRole.Accent).AddColumn(UiStrings.Status, UiRole.Accent);
        if (vehicles.Count == 0) { t.AddEmptyRow(); _ui.Render(t); return; }
        for (int i = 0; i < vehicles.Count; i++)
        {
            var v = vehicles[i];
            var prefix = i == selectedIndex ? UiStrings.RowSelected : UiStrings.RowUnselected;
            t.AddRow($"{prefix}{v.Make}", v.Model, v.Year.ToString(), v.Registration.ToString(), UiFormats.Money(v.PricePerDay), _ui.VehicleStatus(v));
        }
        _ui.Render(t);
    }

    private void CreateReservation(Vehicle vehicle, DateRange range)
    {
        _ui.Clear();
        _ui.Heading(string.Format(UiStrings.TitleNewReservationFor, vehicle.Make, vehicle.Model));
        _ui.WriteLine();

        if (_client.DrivingLicence is null)
        {
            _ui.Line(UiStrings.NoLicenceEnterData, UiRole.Warning);
            if (!_ui.Guard(() => _users.RegisterLicence(_client, _prompts.PromptDrivingLicence()))) return;
        }

        var methodChoice = _prompts.PromptChoice(UiStrings.PaymentMethod, PaymentMethods.Select(x => x.Label));
        var method = PaymentMethods.Single(x => x.Label == methodChoice);

        var days = range.Days;
        var totalCost = vehicle.PricePerDay * days;

        Payment payment;
        try { payment = method.Create(totalCost, _clock.Now, _client, _prompts); }
        catch (DomainException ex) { _ui.Error(ex.Message); return; }

        var summary = _ui.CreateDetailsTable();
        summary.AddRow(UiStrings.Vehicle, $"{vehicle.Make} {vehicle.Model}");
        summary.AddRow(UiStrings.From, range.From.ToString(UiFormats.Date));
        summary.AddRow(UiStrings.To, range.To!.Value.ToString(UiFormats.Date));
        summary.AddRow(UiStrings.Days, days.ToString());
        summary.AddRow(UiStrings.Total, UiFormats.Money(totalCost));
        summary.AddRow(UiStrings.Method, payment.Describe());
        _ui.Render(summary);

        if (!_prompts.PromptConfirm(UiStrings.ConfirmReservation))
        { _ui.Error(UiStrings.Cancelled); return; }

        _ui.Guard(() =>
        {
            _reservations.Create(_client, vehicle, range, payment);
            _ui.Success(UiStrings.ReservationCreated);
        });
    }
}
