using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;

namespace WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;

internal sealed class VehicleDetailsScreen : IScreen
{
    private readonly Vehicle _vehicle;
    private readonly IUiRenderer _ui;
    private readonly VehicleService _vehicleService;
    private readonly IReservationRepository _reservations;
    private readonly IClock _clock;
    private readonly INavigator _navigator;

    public VehicleDetailsScreen(Vehicle vehicle, IUiRenderer ui,
        VehicleService vehicleService, IReservationRepository reservations, IClock clock, INavigator navigator)
    {
        throw new NotImplementedException();
    }

    public void Run()
    {
        throw new NotImplementedException();
    }
}
