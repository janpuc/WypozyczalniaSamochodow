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
        _vehicle = vehicle; _ui = ui; _prompts = prompts; _vehicleService = vehicleService; }
    }

    public void Run()
    {
        _ui.Clear();
        _ui.Heading(UiStrings.TitleAddInsurance);
        _ui.Guard(() =>
        {
            var company = _prompts.PromptText(UiStrings.PromptCompany);
            var policy = new PolicyNumber(_prompts.PromptText(UiStrings.PromptPolicyNumber));
            var name = _prompts.PromptText(UiStrings.PromptPolicyName);
            var issue = _prompts.PromptDate(UiStrings.PromptIssueDate);
            var expiry = _prompts.PromptDate(UiStrings.PromptExpiryDate, notBefore: issue.AddDays(1));
            var cost = new Money(_prompts.PromptDecimal(UiStrings.Cost));
            _vehicleService.AddInsurance(_vehicle, new Insurance(company, policy, name, issue, expiry, cost));
            _ui.Success(UiStrings.InsuranceAdded);
        });
    }
}
