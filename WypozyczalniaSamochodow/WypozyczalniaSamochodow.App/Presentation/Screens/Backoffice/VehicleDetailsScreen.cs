using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Insurance;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class VehicleDetailsScreen : IScreen
{
    private enum VehicleTab
    {
        Details,
        Insurances,
        Events,
        Reservations
    }

    private readonly Vehicle _vehicle;
    private readonly IUiRenderer _ui;
    private readonly VehicleService _vehicleService;
    private readonly IReservationRepository _reservations;
    private readonly IClock _clock;
    private readonly INavigator _navigator;

    public VehicleDetailsScreen(Vehicle vehicle, IUiRenderer ui,
        VehicleService vehicleService, IReservationRepository reservations, IClock clock, INavigator navigator)
    {
        _vehicle = vehicle; _ui = ui; _vehicleService = vehicleService; _reservations = reservations; _clock = clock;
        _navigator = navigator;
    }

    public void Run()
    {
        var tab = VehicleTab.Details;
        int selectedIndex = 0;
        var tabNames = new[] { UiStrings.TabDetails, UiStrings.TabInsurances, UiStrings.TabEvents, UiStrings.TabReservations };

        while (true)
        {
            _ui.Clear();
            _ui.Heading($"{_vehicle.Make} {_vehicle.Model}");
            _ui.WriteLine();
            _ui.DrawTabs(tabNames, (int)tab);
            _ui.WriteLine();

            int count = 0;
            if (tab == VehicleTab.Details) DrawDetails();
            else if (tab == VehicleTab.Insurances) { var ins = _vehicle.Insurances.OrderBy(i => i.IssueDate).ToList(); count = ins.Count; DrawInsurances(ins, selectedIndex); }
            else if (tab == VehicleTab.Events) { var evs = _vehicle.Schedule.NonReservationEvents.OrderBy(e => e.FromDate).ToList(); count = evs.Count; DrawEvents(evs, selectedIndex); }
            else { var rs = _reservations.OfVehicle(_vehicle).OrderBy(r => r.Event.FromDate).ToList(); count = rs.Count; DrawReservations(rs, selectedIndex); }

            _ui.WriteLine();
            DrawHint(tab);

            var key = _ui.ReadKey();
            if (Keys.Back.Matches(key)) return;
            if (Keys.NextTab.Matches(key))
            {
                tab = (VehicleTab)TabNavigation.Cycle((int)tab, tabNames.Length, key);
                selectedIndex = 0;
                continue;
            }
            if (Keys.Down.Matches(key) && selectedIndex < count - 1) { selectedIndex++; continue; }
            if (Keys.Up.Matches(key) && selectedIndex > 0) { selectedIndex--; continue; }

            if (tab == VehicleTab.Details && Keys.Delete.Matches(key))
            {
                if (_reservations.HasActiveOf(_vehicle)) { _ui.Error(UiStrings.VehicleHasActiveReservations); continue; }
                if (_ui.ConfirmDelete(UiStrings.EntityVehicle, $"{_vehicle.Make} {_vehicle.Model}"))
                { _vehicleService.Remove(_vehicle); _ui.Success(UiStrings.Removed); return; }
            }
            if (tab == VehicleTab.Insurances)
            {
                if (Keys.Add.Matches(key)) { OpenAddInsurance(); continue; }
                if (Keys.Delete.Matches(key))
                {
                    var ins = _vehicle.Insurances.OrderBy(i => i.IssueDate).ToList();
                    if (selectedIndex < ins.Count && _ui.ConfirmDelete(UiStrings.EntityInsurance, ins[selectedIndex].PolicyName))
                    { _vehicleService.RemoveInsurance(_vehicle, ins[selectedIndex]); _ui.Success(UiStrings.Removed); }
                }
            }
            if (tab == VehicleTab.Events)
            {
                if (Keys.Add.Matches(key))
                { OpenAddVehicleEvent(); continue; }
                if (Keys.Repair.Matches(key))
                {
                    var bd = _vehicle.Schedule.ActiveBrokenDownOn(_clock.Today);
                    if (bd is not null && bd.LinkedRepair is null)
                        OpenCreateRepair(bd);
                    continue;
                }
                if (Keys.Delete.Matches(key))
                {
                    var evs = _vehicle.Schedule.NonReservationEvents.OrderBy(e => e.FromDate).ToList();
                    if (selectedIndex < evs.Count && _ui.ConfirmDelete(UiStrings.EntityEvent, evs[selectedIndex].Describe()))
                        _ui.Guard(() => { _vehicleService.RemoveEvent(_vehicle, evs[selectedIndex]); _ui.Success(UiStrings.Removed); });
                }
            }
            if (tab == VehicleTab.Reservations && Keys.Select.Matches(key))
            {
                var rs = _reservations.OfVehicle(_vehicle).OrderBy(r => r.Event.FromDate).ToList();
                if (selectedIndex < rs.Count)
                    OpenReservationDetails(rs[selectedIndex]);
            }
        }
    }

    private void OpenAddInsurance() => _navigator.OpenAddInsurance(_vehicle);

    private void OpenAddVehicleEvent() => _navigator.OpenAddVehicleEvent(_vehicle);

    private void OpenCreateRepair(BrokenDownEvent brokenDown) => _navigator.OpenCreateRepair(_vehicle, brokenDown);

    private void OpenReservationDetails(Domain.Reservations.Reservation reservation) =>
        _navigator.OpenReservationDetails(reservation);

    private void DrawHint(VehicleTab tab)
    {
        switch (tab)
        {
            case VehicleTab.Details:
                _ui.Hint((Keys.Delete, UiStrings.HintDeleteVehicle), (Keys.NextTab, UiStrings.HintNext), (Keys.Back, UiStrings.HintBack));
                break;
            case VehicleTab.Insurances:
                _ui.Hint((Keys.Add, UiStrings.HintAdd), (Keys.Delete, UiStrings.HintDelete), (Keys.NextTab, UiStrings.HintNext), (Keys.Back, UiStrings.HintBack));
                break;
            case VehicleTab.Events:
                _ui.Hint((Keys.Add, UiStrings.HintAdd), (Keys.Repair, UiStrings.HintRepair), (Keys.Delete, UiStrings.HintDelete), (Keys.NextTab, UiStrings.HintNext), (Keys.Back, UiStrings.HintBack));
                break;
            default:
                _ui.Hint((Keys.Select, UiStrings.HintDetails), (Keys.NextTab, UiStrings.HintNext), (Keys.Back, UiStrings.HintBack));
                break;
        }
    }

    private void DrawDetails()
    {
        var t = _ui.CreateDetailsTable();
        _ui.AddVehicleRows(t, _vehicle);
        _ui.Render(t);
    }

    private void DrawInsurances(List<Insurance> ins, int selectedIndex)
    {
        if (ins.Count == 0) { _ui.Line(UiStrings.NoInsurances, UiRole.Muted); return; }
        var t = SelectableList.Build(ins, selectedIndex,
            new[] { UiStrings.Company, UiStrings.Policy, UiStrings.PolicyDisplayName, UiStrings.From, UiStrings.To, UiStrings.Cost },
            x => new[] { x.Company, x.Number.ToString(), x.PolicyName, x.IssueDate.ToString(UiFormats.Date), x.ExpiryDate.ToString(UiFormats.Date), UiFormats.Money(x.Cost) });
        _ui.Render(t);
    }

    private void DrawEvents(List<VehicleEvent> evs, int selectedIndex)
    {
        if (evs.Count == 0) { _ui.Line(UiStrings.NoEvents, UiRole.Muted); return; }
        var t = SelectableList.Build(evs, selectedIndex,
            new[] { UiStrings.From, UiStrings.To, UiStrings.Type, UiStrings.Description, UiStrings.Status },
            ev =>
            {
                var isNow = ev.IsActiveOn(_clock.Today);
                var isPast = ev.IsPastOn(_clock.Today);
                var status = isNow ? _ui.Colorize(UiStrings.EventStatusActive, UiRole.Error)
                    : isPast ? _ui.Colorize(UiStrings.EventStatusCompleted, UiRole.Muted)
                    : _ui.Colorize(UiStrings.EventStatusPlanned, UiRole.Warning);
                return new[] { ev.FromDate.ToString(UiFormats.Date), _ui.FormatDate(ev.ToDate), _ui.EventLabelColored(ev, isNow), ev.Description ?? UiStrings.Empty, status };
            });
        _ui.Render(t);
    }

    private void DrawReservations(List<Domain.Reservations.Reservation> rs, int selectedIndex)
    {
        if (rs.Count == 0) { _ui.Line(UiStrings.NoReservations, UiRole.Muted); return; }
        var t = SelectableList.Build(rs, selectedIndex,
            new[] { UiStrings.Client, UiStrings.From, UiStrings.To, UiStrings.Status },
            r => new[] { r.Client.FullName, r.Event.FromDate.ToString(UiFormats.Date), _ui.FormatDate(r.Event.ToDate), _ui.ReservationStatus(r) });
        _ui.Render(t);
    }
}
