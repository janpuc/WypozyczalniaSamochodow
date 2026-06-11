using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;


internal sealed class AddVehicleEventScreen : IScreen
{
    private sealed record EventTypeOption(string Label, bool IsOpenEnded, Func<DateRange, string?, VehicleEvent> Create);

    private static readonly EventTypeOption[] EventTypes =
    {
        new(UiStrings.EventBrokenDown, true, (range, note) => new BrokenDownEvent(range, note)),
        new(UiStrings.EventMaintenance, false, (range, note) => new MaintenanceEvent(range, note)),
        new(UiStrings.EventSuspended, true, (range, note) => new SuspendedEvent(range.From, note)),
        new(UiStrings.EventInspection, false, (range, note) => new InspectionEvent(range, note)),
        new(UiStrings.EventDetailing, false, (range, note) => new DetailingEvent(range, note))
    };

    private readonly Vehicle _vehicle;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly VehicleService _vehicleService;
    private readonly IClock _clock;

    public AddVehicleEventScreen(Vehicle vehicle, IUiRenderer ui, IPrompts prompts, VehicleService vehicleService, IClock clock)
    { _vehicle = vehicle; _ui = ui; _prompts = prompts; _vehicleService = vehicleService; _clock = clock; }

    public void Run()
    {
        _ui.Clear();
        _ui.Heading(UiStrings.TitleAddEvent);

        var type = _prompts.PromptChoice(UiStrings.PromptEventType, EventTypes.Select(x => x.Label));
        var eventType = EventTypes.Single(x => x.Label == type);

        _ui.Guard(() =>
        {
            var from = _prompts.PromptDate(UiStrings.PromptStartDate, notBefore: _clock.Today);
            var description = _prompts.PromptText(UiStrings.PromptDescriptionOptional, allowEmpty: true);
            var note = description.NullIfBlank();
            var range = eventType.IsOpenEnded
                ? DateRange.OpenEnded(from)
                : DateRange.Closed(from, _prompts.PromptDate(UiStrings.PromptEndDate, notBefore: from));
            var ev = eventType.Create(range, note);
            _vehicleService.ScheduleEvent(_vehicle, ev);
            _ui.Success(UiStrings.EventAdded);
        });
    }
}
