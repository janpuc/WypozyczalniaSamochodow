using WypozyczalniaSamochodow.App.Application.Reservations;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Client;

internal sealed class NewReservationFlow : IScreen
{
    private readonly Domain.Users.Client _client;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly ReservationService _reservations;
    private readonly UserAccountService _users;
    private readonly IClock _clock;

    public NewReservationFlow(Domain.Users.Client client, IUiRenderer ui, IPrompts prompts,
        ReservationService reservations, UserAccountService users, IClock clock)
    {
        _client = client; _ui = ui; _prompts = prompts; _reservations = reservations; _users = users; _clock = clock;
    }

    public void Run()
    {
        throw new NotImplementedException();
    }
}

