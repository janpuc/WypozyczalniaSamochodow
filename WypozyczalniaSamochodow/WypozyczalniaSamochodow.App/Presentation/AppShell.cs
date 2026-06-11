using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation;

internal sealed class AppShell
{
    private readonly AuthService _auth;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly INavigator _navigator;

    public AppShell(AuthService auth, IUiRenderer ui, IPrompts prompts, INavigator navigator)
    {
        _auth = auth;
        _ui = ui;
        _prompts = prompts;
        _navigator = navigator;
    }

    public void Run()
    {
        while (true)
        {
            _ui.Clear();
            _ui.Banner(UiStrings.AppTitle);
            _ui.WriteLine();
            var choice = _ui.Menu(UiStrings.MainMenuPrompt,
                new[] { UiStrings.MenuLogin, UiStrings.MenuRegister, UiStrings.MenuExit });

            if (choice == UiStrings.MenuLogin)
            {
                _navigator.OpenLogin();
            }
            else if (choice == UiStrings.MenuRegister)
            {
                _navigator.OpenRegister();
                _ui.WaitForKey();
            }
            else if (choice == UiStrings.MenuExit)
            {
                _ui.Clear();
                _ui.Line(UiStrings.Goodbye);
                return;
            }
        }
    }
}
