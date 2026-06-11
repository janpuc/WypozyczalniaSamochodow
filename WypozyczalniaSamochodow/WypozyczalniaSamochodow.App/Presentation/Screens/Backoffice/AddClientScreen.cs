using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;


internal sealed class AddClientScreen : IScreen
{
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly UserAccountService _users;

    public AddClientScreen(IUiRenderer ui, IPrompts prompts, UserAccountService users)
    {
        _ui = ui; _prompts = prompts; _users = users;
    }

    public void Run()
    {
        _ui.Clear();
        _ui.Heading(UiStrings.TitleAddClient);
        _ui.Guard(() =>
        {
            var fullName = _prompts.PromptFullName();
            var email = new Email(_prompts.PromptEmail());
            var password = _prompts.PromptPassword();
            DrivingLicence? lic = _prompts.PromptConfirm(UiStrings.ConfirmAddLicence)
                ? _prompts.PromptDrivingLicence() : null;
            _users.CreateClient(fullName, email, password, lic);
            _ui.Success(UiStrings.ClientAdded);
        });
    }
}
