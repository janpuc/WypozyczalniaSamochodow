using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

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
        throw new NotImplementedException();
    }

    public void Run()
    {
        throw new NotImplementedException();
    }
}
