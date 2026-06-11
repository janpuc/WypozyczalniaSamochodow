using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

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
    {
        throw new NotImplementedException();
    }

    public void Run()
    {
        throw new NotImplementedException();
    }
}
