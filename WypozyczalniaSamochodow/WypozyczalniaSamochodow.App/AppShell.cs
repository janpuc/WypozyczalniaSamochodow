using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App;

internal sealed class AppShell
{
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly INavigator _navigator;

    public AppShell(IUiRenderer ui, IPrompts prompts, INavigator navigator)
    {
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
