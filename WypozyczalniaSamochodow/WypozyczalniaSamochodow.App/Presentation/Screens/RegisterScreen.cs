using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens;

internal sealed class RegisterScreen : IScreen
{
    private static readonly Dictionary<RegistrationResult, (string Message, UiRole Role)> Outcomes =
        new()
        {
            [RegistrationResult.Success] = (UiStrings.RegisterSuccess, UiRole.Success),
            [RegistrationResult.EmailTaken] = (UiStrings.ValidationEmailTaken, UiRole.Error),
            [RegistrationResult.InvalidEmail] = (UiStrings.ValidationEmailInvalid, UiRole.Error),
            [RegistrationResult.WeakPassword] =
                (string.Format(UiStrings.ValidationPasswordTooShort, PasswordPolicy.MinimumLength), UiRole.Error),
        };

    private readonly AuthService _auth;
    private readonly IPrompts _prompts;
    private readonly IUiRenderer _ui;

    public RegisterScreen(AuthService auth, IPrompts prompts, IUiRenderer ui)
    {
        _auth = auth; _prompts = prompts; _ui = ui;
    }

    public void Run()
    {
        _ui.Clear();
        _ui.Heading(UiStrings.TitleRegister);
        _ui.WriteLine();

        var fullName = _prompts.PromptFullName();
        var email = _prompts.PromptEmail();
        var password = _prompts.PromptPassword(PasswordPolicy.MinimumLength);
        _ = _prompts.PromptPasswordConfirmation(password);

        DrivingLicence? licence = null;
        if (_prompts.PromptConfirm(UiStrings.ConfirmAddLicence))
            licence = _prompts.PromptDrivingLicence();

        try
        {
            var (message, role) = Outcomes[_auth.RegisterClient(fullName, email, password, licence)];
            _ui.Line(message, role);
        }
        catch (DomainException ex)
        {
            _ui.Line(ex.Message, UiRole.Error);
        }
    }
}

