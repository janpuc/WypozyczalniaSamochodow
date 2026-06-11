using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class BackofficeUserDetailsScreen : IScreen
{
    private readonly Domain.Users.Backoffice _user;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly UserAccountService _users;

    public BackofficeUserDetailsScreen(Domain.Users.Backoffice user, IUiRenderer ui, IPrompts prompts,
        UserAccountService users)
    {
        _user = user; _ui = ui; _prompts = prompts; _users = users;
    }

    public void Run()
    {
        while (true)
        {
            _ui.Clear();
            _ui.Heading(string.Format(UiStrings.TitleDetailsFor, _user.FullName));
            _ui.WriteLine();
            var t = _ui.CreateDetailsTable();
            t.AddRow(UiStrings.FullName, _user.FullName);
            t.AddRow(UiStrings.Email, _user.Email.Value);
            _ui.Render(t);
            _ui.WriteLine();
            _ui.Hint((Keys.Edit, UiStrings.HintEdit), (Keys.ResetPassword, UiStrings.HintResetPassword), (Keys.Delete, UiStrings.HintDelete), (Keys.Back, UiStrings.HintBack));

            var key = _ui.ReadKey();
            if (Keys.Back.Matches(key)) return;
            if (Keys.Edit.Matches(key))
            {
                _ui.Guard(() =>
                {
                    _users.UpdateProfile(_user,
                        _prompts.PromptFullName(_user.FullName),
                        _prompts.PromptEmail(_user.Email.Value));
                    _ui.Success(UiStrings.Updated);
                });
            }
            if (Keys.ResetPassword.Matches(key))
            {
                _ui.Guard(() =>
                {
                    _users.ResetPassword(_user, _prompts.PromptPassword(PasswordPolicy.MinimumLength));
                    _ui.Success(UiStrings.PasswordReset);
                });
            }
            if (Keys.Delete.Matches(key))
            {
                if (_ui.ConfirmDelete(UiStrings.EntityUser, _user.FullName))
                { _users.RemoveBackofficeUser(_user); _ui.Success(UiStrings.Removed); return; }
            }
        }
    }
}
