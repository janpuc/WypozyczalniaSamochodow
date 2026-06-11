using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class AddVehicleScreen : IScreen
{
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly VehicleService _vehicleService;
    public AddVehicleScreen(IUiRenderer ui, IPrompts prompts, VehicleService vehicleService)
    { _ui = ui; _prompts = prompts; _vehicleService = vehicleService; }

    public void Run()
    {
        _ui.Clear();
        _ui.Heading(UiStrings.TitleAddVehicle);
        _ui.Guard(() =>
        {
            var make = _prompts.PromptText(UiStrings.Make);
            var model = _prompts.PromptText(UiStrings.Model);
            var reg = new RegistrationNumber(_prompts.PromptText(UiStrings.PromptRegistration));
            var vin = new Vin(_prompts.PromptText(UiStrings.Vin));
            var color = _prompts.PromptText(UiStrings.Color);
            var price = new Money(_prompts.PromptDecimal(UiStrings.PromptPricePerDay));
            var year = _prompts.PromptInt(UiStrings.PromptYear);
            var purchase = _prompts.PromptDate(UiStrings.PurchaseDate);
            _vehicleService.Add(new Vehicle(make, model, reg, vin, color, price, year, purchase));
            _ui.Success(UiStrings.VehicleAdded);
        });
    }
}
