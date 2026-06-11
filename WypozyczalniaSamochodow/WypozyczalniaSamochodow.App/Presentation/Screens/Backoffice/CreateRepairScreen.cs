using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class CreateRepairScreen : IScreen
{
    private readonly Vehicle _vehicle;
    private readonly BrokenDownEvent _cause;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly VehicleService _vehicleService;
    private readonly IClock _clock;

    public CreateRepairScreen(Vehicle vehicle, BrokenDownEvent cause, IUiRenderer ui, IPrompts prompts, VehicleService vehicleService, IClock clock)
    { _vehicle = vehicle; _cause = cause; _ui = ui; _prompts = prompts; _vehicleService = vehicleService; _clock = clock; }

    public void Run()
    {
        _ui.Clear();
        _ui.Heading(UiStrings.TitleCreateRepair);
        _ui.Guard(() =>
        {
            var from = _prompts.PromptDate(UiStrings.PromptStartDate, defaultValue: _clock.Today, notBefore: _clock.Today);
            var to = _prompts.PromptDate(UiStrings.PromptEndDate, notBefore: from);
            var defaultDesc = _cause.Description is not null ? string.Format(UiStrings.RepairDescriptionPrefix, _cause.Description) : "";
            var desc = _prompts.PromptText(UiStrings.Description, defaultDesc, allowEmpty: true);
            _vehicleService.ScheduleEvent(_vehicle, _cause.RegisterRepair(DateRange.Closed(from, to), desc.NullIfBlank()));
            _ui.Success(UiStrings.RepairCreated);
        });
    }
}
