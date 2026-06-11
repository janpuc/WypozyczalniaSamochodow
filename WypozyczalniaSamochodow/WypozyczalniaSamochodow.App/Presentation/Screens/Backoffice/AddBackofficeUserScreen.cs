using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class AddBackofficeUserScreen : IScreen
{
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly UserAccountService _users;

    public AddBackofficeUserScreen(IUiRenderer ui, IPrompts prompts, UserAccountService users)
    {
        _ui = ui; _prompts = prompts; _users = users;
    }

    public void Run()
    {
        _ui.Clear();
        _ui.Heading(UiStrings.TitleAddBackofficeUser);
        _ui.Guard(() =>
        {
            var fullName = _prompts.PromptFullName();
            var email = new Email(_prompts.PromptEmail());
            var password = _prompts.PromptPassword();
            _users.CreateBackofficeUser(fullName, email, password);
            _ui.Success(UiStrings.UserAdded);
        });
    }
}
