using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class BackofficeDashboardScreen : IScreen
{
    private sealed class Tab
    {
        public Tab(string name, Func<int> count, Func<int, UiTable> build, Action<int> openDetails, Action? openAdd)
        {
            Name = name;
            Count = count;
            Build = build;
            OpenDetails = openDetails;
            OpenAdd = openAdd;
        }

        public string Name { get; }
        public Func<int> Count { get; }
        public Func<int, UiTable> Build { get; }
        public Action<int> OpenDetails { get; }
        public Action? OpenAdd { get; }
        public bool CanAdd => OpenAdd is not null;
    }

    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly IClientRepository _clients;
    private readonly IBackofficeRepository _backoffice;
    private readonly IVehicleRepository _vehicles;
    private readonly IReservationRepository _reservations;
    private readonly INavigator _navigator;
    private readonly Tab[] _tabs;


    public BackofficeDashboardScreen(IUiRenderer ui, IPrompts prompts, IClientRepository clients,
        IBackofficeRepository backoffice, IVehicleRepository vehicles,
        IReservationRepository reservations, INavigator navigator)
    {
        _ui = ui; _prompts = prompts; _clients = clients; _backoffice = backoffice; _vehicles = vehicles;
        _reservations = reservations; _navigator = navigator;
        _tabs = BuildTabs();
    }

    private Tab[] BuildTabs() =>
    [
        new(UiStrings.TabReservations,
            () => _reservations.All.Count,
            i => SelectableList.Build(_reservations.All, i,
                new[] { UiStrings.Client, UiStrings.Vehicle, UiStrings.From, UiStrings.To, UiStrings.Status },
                r => new[] { r.Client.FullName, $"{r.Vehicle.Make} {r.Vehicle.Model}", r.Event.FromDate.ToString(UiFormats.Date), _ui.FormatDate(r.Event.ToDate), _ui.ReservationStatus(r) }),
            i => _navigator.OpenReservationDetails(_reservations.All[i]),
            openAdd: null),
        new(UiStrings.TabVehicles,
            () => _vehicles.All.Count,
            i => SelectableList.Build(_vehicles.All, i,
                new[] { UiStrings.Make, UiStrings.Model, UiStrings.Year, UiStrings.Registration, UiStrings.PricePerDay, UiStrings.Status },
                v => new[] { v.Make, v.Model, v.Year.ToString(), v.Registration.ToString(), UiFormats.Money(v.PricePerDay), _ui.VehicleStatus(v) }),
            i => _navigator.OpenVehicleDetails(_vehicles.All[i]),
            _navigator.OpenAddVehicle),
        new(UiStrings.TabClients,
            () => _clients.All.Count,
            i => SelectableList.Build(_clients.All, i,
                new[] { UiStrings.FullName, UiStrings.Email, UiStrings.DrivingLicence },
                c => new[] { c.FullName, c.Email.Value, c.DrivingLicence is not null ? UiStrings.Yes : UiStrings.No }),
            i => _navigator.OpenClientDetails(_clients.All[i]),
            _navigator.OpenAddClient),
        new(UiStrings.TabAdmin,
            () => _backoffice.All.Count,
            i => SelectableList.Build(_backoffice.All, i,
                new[] { UiStrings.FullName, UiStrings.Email },
                b => new[] { b.FullName, b.Email.Value }),
            i => _navigator.OpenBackofficeUserDetails(_backoffice.All[i]),
            _navigator.OpenAddBackofficeUser),
    ];

    public void Run()
    {
        var tabNames = Array.ConvertAll(_tabs, t => t.Name);
        int tabIndex = 0;
        int selectedIndex = 0;

        while (true)
        {
            var tab = _tabs[tabIndex];
            _ui.Clear();
            _ui.DrawTabs(tabNames, tabIndex);
            _ui.WriteLine();

            var count = tab.Count();
            selectedIndex = TabNavigation.ClampSelection(selectedIndex, count);
            _ui.Render(tab.Build(selectedIndex));
            _ui.WriteLine();
            _ui.Hint(BuildHints(tab));

            var key = _ui.ReadKey();
            if (Keys.Back.Matches(key)) { _ui.Line(UiStrings.LoggedOut, UiRole.Success); _ui.WaitForKey(); return; }
            if (Keys.NextTab.Matches(key)) { tabIndex = TabNavigation.Cycle(tabIndex, _tabs.Length, key); selectedIndex = 0; continue; }
            if (Keys.Down.Matches(key) && selectedIndex < count - 1) { selectedIndex++; continue; }
            if (Keys.Up.Matches(key) && selectedIndex > 0) { selectedIndex--; continue; }
            if (Keys.Select.Matches(key) && count > 0) { tab.OpenDetails(selectedIndex); continue; }
            if (Keys.Add.Matches(key) && tab.CanAdd) { tab.OpenAdd!(); continue; }
        }
    }

    private static HintItem[] BuildHints(Tab tab)
    {
        var hints = new List<HintItem> { (Keys.Select, UiStrings.HintDetails) };
        if (tab.CanAdd) hints.Add((Keys.Add, UiStrings.HintAdd));
        hints.Add((Keys.Back, UiStrings.HintLogout));
        return hints.ToArray();
    }
}

