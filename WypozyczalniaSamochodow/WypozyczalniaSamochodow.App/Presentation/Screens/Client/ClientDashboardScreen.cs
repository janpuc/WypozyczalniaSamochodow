using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Client;


internal sealed class ClientDashboardScreen : IScreen
{
    private enum ClientTab
    {
        Reservations,
        DrivingLicence,
        PersonalData
    }

    private readonly Domain.Users.Client _client;
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly IReservationRepository _reservations;
    private readonly UserAccountService _users;
    private readonly IClock _clock;
    private readonly INavigator _navigator;

    public ClientDashboardScreen(Domain.Users.Client client, IUiRenderer ui, IPrompts prompts,
        IReservationRepository reservations, UserAccountService users, IClock clock, INavigator navigator)
    {
        _client = client; _ui = ui; _prompts = prompts;
        _reservations = reservations; _users = users; _clock = clock; _navigator = navigator;
    }

    public void Run()
    {
        throw new NotImplementedException();
    }

}

