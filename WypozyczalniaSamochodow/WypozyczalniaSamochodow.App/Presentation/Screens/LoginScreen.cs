using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens;

internal sealed class LoginScreen : IScreen
{
    private readonly AuthService _auth;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly INavigator _navigator;

    public LoginScreen(AuthService auth, IUiRenderer ui, IPrompts prompts, INavigator navigator)
    {
        _auth = auth; _ui = ui; _prompts = prompts; _navigator = navigator;
    }

    public void Run()
    {
        _ui.Clear();
        _ui.Heading(UiStrings.TitleLogin);
        _ui.WriteLine();

        var email = _prompts.PromptEmail();
        var password = _prompts.PromptPassword();

        User? user = null;
        _ui.RunWithStatus(UiStrings.StatusLoggingIn, () => { user = _auth.Login(email, password); });

        if (user is null)
        {
            _ui.Line(UiStrings.LoginFailed, UiRole.Error);
            _ui.WaitForKey();
            return;
        }

        _ui.Line(UiStrings.LoginSuccess, UiRole.Success);
        _ui.WaitForKey();

        if (user is Domain.Users.Backoffice)
            _navigator.OpenBackofficeDashboard();
        else
            _navigator.OpenClientDashboard((Domain.Users.Client)user);
    }
}
