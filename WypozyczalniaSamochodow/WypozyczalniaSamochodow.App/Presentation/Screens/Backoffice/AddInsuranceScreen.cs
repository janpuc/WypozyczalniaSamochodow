using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class AddInsuranceScreen : IScreen
{
    private readonly Vehicle _vehicle;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly VehicleService _vehicleService;
    public AddInsuranceScreen(Vehicle vehicle, IUiRenderer ui, IPrompts prompts, VehicleService vehicleService)
    {
        throw new NotImplementedException();
    }

    public void Run()
    {
        throw new NotImplementedException();
    }
}
