using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;


internal sealed class BackofficeDashboardScreen : IScreen
{
    private readonly IUiRenderer _ui;
    private readonly IPrompts _prompts;
    private readonly IClientRepository _clients;
    private readonly IBackofficeRepository _backoffice;
    private readonly IVehicleRepository _vehicles;
    private readonly IReservationRepository _reservations;
    private readonly INavigator _navigator;

    public BackofficeDashboardScreen(IUiRenderer ui, IPrompts prompts, IClientRepository clients,
        IBackofficeRepository backoffice, IVehicleRepository vehicles,
        IReservationRepository reservations, INavigator navigator)
    {
        throw new NotImplementedException();
    }
    public void Run()
    {
        throw new NotImplementedException();
    }
}

