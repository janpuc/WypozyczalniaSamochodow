using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class ClientDetailsScreen : IScreen
{
    private readonly Domain.Users.Client _client;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly UserAccountService _users;
    private readonly IReservationRepository _reservations;

    public ClientDetailsScreen(Domain.Users.Client client, IUiRenderer ui, IPrompts prompts,
        UserAccountService users, IReservationRepository reservations)
    {
        _client = client; _ui = ui; _prompts = prompts; _users = users; _reservations = reservations;
    }

    public void Run()
    {
        while (true)
        {
            _ui.Clear();
            _ui.Heading(string.Format(UiStrings.TitleDetailsFor, _client.FullName));
            _ui.WriteLine();
            var t = _ui.CreateDetailsTable();
            t.AddRow(UiStrings.FullName, _client.FullName);
            t.AddRow(UiStrings.Email, _client.Email.Value);
            t.AddRow(UiStrings.DrivingLicence, _client.DrivingLicence is null ? UiStrings.No : _client.DrivingLicence.Number);
            _ui.Render(t);
            _ui.WriteLine();
            _ui.Hint((Keys.Edit, UiStrings.HintEdit), (Keys.Delete, UiStrings.HintDelete), (Keys.Back, UiStrings.HintBack));

            var key = _ui.ReadKey();
            if (Keys.Back.Matches(key)) return;
            if (Keys.Edit.Matches(key))
            {
                _ui.Guard(() =>
                {
                    _users.UpdateProfile(_client,
                        _prompts.PromptFullName(_client.FullName),
                        _prompts.PromptEmail(_client.Email.Value));
                    _ui.Success(UiStrings.Updated);
                });
            }
            if (Keys.Delete.Matches(key))
            {
                if (_reservations.HasActiveOf(_client)) { _ui.Error(UiStrings.ClientHasActiveReservations); continue; }
                if (_ui.ConfirmDelete(UiStrings.EntityClient, _client.FullName))
                { _users.RemoveClient(_client); _ui.Success(UiStrings.Removed); return; }
            }
        }
    }
}
