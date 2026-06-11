using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Client;


internal sealed class ClientDashboardScreen : IScreen
{
    private enum ClientTab
    {
        Reservations,
        DrivingLicence,
        PersonalData
    }

    private readonly Domain.Users.Client _client;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly IReservationRepository _reservations;
    private readonly UserAccountService _users;
    private readonly IClock _clock;
    private readonly INavigator _navigator;

    public ClientDashboardScreen(Domain.Users.Client client, IUiRenderer ui, IPrompts prompts,
        IReservationRepository reservations, UserAccountService users, IClock clock, INavigator navigator)
    {
        _client = client; _ui = ui; _prompts = prompts;
        _reservations = reservations; _users = users; _clock = clock; _navigator = navigator;
    }

    public void Run()
    {
        var tab = ClientTab.Reservations;
        int selectedIndex = 0;
        var tabNames = new[] { UiStrings.TabReservations, UiStrings.DrivingLicence, UiStrings.TabPersonalData };

        while (true)
        {
            _ui.Clear();
            _ui.DrawTabs(tabNames, (int)tab);
            _ui.WriteLine();

            var myReservations = _reservations.OfClient(_client).ToList();
            bool hasActive = _reservations.HasActiveOf(_client);

            if (tab == ClientTab.Reservations)
            {
                selectedIndex = TabNavigation.ClampSelection(selectedIndex, myReservations.Count);
                DrawReservationList(myReservations, selectedIndex);
                _ui.WriteLine();
                if (myReservations.Count == 0)
                    _ui.Line(UiStrings.NoReservationsCreate, UiRole.Muted);
                _ui.Hint((Keys.Select, UiStrings.HintDetails), (Keys.Add, UiStrings.HintNewReservation),
                    (Keys.NextTab, UiStrings.DrivingLicence), (Keys.Back, UiStrings.HintLogout));
            }
            else if (tab == ClientTab.DrivingLicence)
            {
                DrawLicence();
                _ui.WriteLine();
                if (hasActive)
                    _ui.Line(UiStrings.LicenceEditLocked, UiRole.Warning);
                if (_client.DrivingLicence is null && !hasActive)
                    _ui.Hint((Keys.Add, UiStrings.HintAddLicence), (Keys.NextTab, UiStrings.HintNext), (Keys.Back, UiStrings.HintLogout));
                else if (!hasActive)
                    _ui.Hint((Keys.Edit, UiStrings.HintEdit), (Keys.NextTab, UiStrings.HintNext), (Keys.Back, UiStrings.HintLogout));
                else
                    _ui.Hint((Keys.NextTab, UiStrings.HintNext), (Keys.Back, UiStrings.HintLogout));
            }
            else
            {
                DrawPersonalData();
                _ui.WriteLine();
                if (hasActive)
                    _ui.Line(UiStrings.PersonalDataEditLocked, UiRole.Warning);
                if (!hasActive)
                    _ui.Hint((Keys.Edit, UiStrings.HintEdit), (Keys.NextTab, UiStrings.HintNext), (Keys.Back, UiStrings.HintLogout));
                else
                    _ui.Hint((Keys.NextTab, UiStrings.HintNext), (Keys.Back, UiStrings.HintLogout));
            }

            var key = _ui.ReadKey();
            if (Keys.Back.Matches(key)) { _ui.Line(UiStrings.LoggedOut, UiRole.Success); _ui.WaitForKey(); return; }

            if (Keys.NextTab.Matches(key))
            {
                tab = (ClientTab)TabNavigation.Cycle((int)tab, tabNames.Length, key);
                selectedIndex = 0;
                continue;
            }

            if (tab == ClientTab.Reservations)
            {
                if (Keys.Add.Matches(key))
                { OpenNewReservation(); continue; }
                if (Keys.Down.Matches(key) && selectedIndex < myReservations.Count - 1) { selectedIndex++; continue; }
                if (Keys.Up.Matches(key) && selectedIndex > 0) { selectedIndex--; continue; }
                if (Keys.Select.Matches(key) && myReservations.Count > 0)
                { OpenReservationDetails(myReservations[selectedIndex]); continue; }
            }
            else if (tab == ClientTab.DrivingLicence && !hasActive)
            {
                if (Keys.Edit.Matches(key) && _client.DrivingLicence is not null)
                    _ui.Guard(() => { _users.RegisterLicence(_client, _prompts.PromptDrivingLicence(_client.DrivingLicence)); _ui.Success(UiStrings.Updated); });
                if (Keys.Add.Matches(key) && _client.DrivingLicence is null)
                    _ui.Guard(() => { _users.RegisterLicence(_client, _prompts.PromptDrivingLicence()); _ui.Success(UiStrings.LicenceAdded); });
            }
            else if (tab == ClientTab.PersonalData && !hasActive && Keys.Edit.Matches(key))
            {
                _ui.Guard(() =>
                {
                    _users.UpdateProfile(_client,
                        _prompts.PromptFullName(_client.FullName),
                        _prompts.PromptEmail(_client.Email.Value));
                    _ui.Success(UiStrings.PersonalDataUpdated);
                });
            }
        }
    }
    private void OpenNewReservation() => _navigator.OpenNewReservation(_client);

    private void OpenReservationDetails(Reservation reservation) =>
        _navigator.OpenClientReservationDetails(reservation);

    private void DrawReservationList(List<Reservation> reservations, int selectedIndex)
    {
        var t = SelectableList.Build(reservations, selectedIndex,
            new[] { UiStrings.Vehicle, UiStrings.From, UiStrings.To, UiStrings.Status },
            r => new[] { $"{r.Vehicle.Make} {r.Vehicle.Model}", r.Event.FromDate.ToString(UiFormats.Date), _ui.FormatDate(r.Event.ToDate), _ui.ReservationStatus(r) });
        _ui.Render(t);
    }

    private void DrawLicence()
    {
        var t = _ui.CreateDetailsTable();
        if (_client.DrivingLicence is not null)
        {
            t.AddRow(UiStrings.LicenceNumber, _client.DrivingLicence.Number);
            t.AddRow(UiStrings.LicenceExpiry, _client.DrivingLicence.ExpiryDate.ToString(UiFormats.Date));
            t.AddRow(UiStrings.LicenceValid, _client.DrivingLicence.IsValidOn(_clock.Today)
                ? _ui.Colorize(UiStrings.Yes, UiRole.Success) : _ui.Colorize(UiStrings.No, UiRole.Error));
        }
        else
            t.AddRow(UiStrings.DrivingLicence, _ui.Colorize(UiStrings.None, UiRole.Muted));
        _ui.Render(t);
    }

    private void DrawPersonalData()
    {
        var t = _ui.CreateDetailsTable();
        t.AddRow(UiStrings.FullName, _client.FullName);
        t.AddRow(UiStrings.Email, _client.Email.Value);
        _ui.Render(t);
    }
}
