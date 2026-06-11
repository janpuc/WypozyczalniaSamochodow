using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class AddBackofficeUserScreen : IScreen
{
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly UserAccountService _users;

    public AddBackofficeUserScreen(IUiRenderer ui, IPrompts prompts, UserAccountService users)
    {
        throw new NotImplementedException();
    }

    public void Run()
    {
        throw new NotImplementedException();
    }
}
