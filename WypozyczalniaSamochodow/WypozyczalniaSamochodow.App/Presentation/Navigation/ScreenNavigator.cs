using Microsoft.Extensions.DependencyInjection;

using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Screens;

namespace WypozyczalniaSamochodow.App.Presentation.Navigation;

internal sealed class ScreenNavigator : INavigator
{
    private readonly IServiceProvider _provider;

    public ScreenNavigator(IServiceProvider provider) => _provider = provider;

    private void Open<TScreen>(params object[] runtimeArguments) where TScreen : IScreen =>
        ActivatorUtilities.CreateInstance<TScreen>(_provider, runtimeArguments).Run();

    public void OpenLogin() => Open<LoginScreen>();
    public void OpenRegister() => Open<RegisterScreen>();
    public void OpenBackofficeDashboard() => throw new NotImplementedException();
    public void OpenClientDashboard(Domain.Users.Client client) => throw new NotImplementedException();
    public void OpenNewReservation(Domain.Users.Client client) => throw new NotImplementedException();
    public void OpenClientReservationDetails(Reservation reservation) => throw new NotImplementedException();
    public void OpenReservationDetails(Reservation reservation) => throw new NotImplementedException();
    public void OpenVehicleDetails(Vehicle vehicle) => throw new NotImplementedException();
    public void OpenClientDetails(Domain.Users.Client client) => throw new NotImplementedException();
    public void OpenBackofficeUserDetails(Domain.Users.Backoffice user) => throw new NotImplementedException();
    public void OpenAddVehicle() => throw new NotImplementedException();
    public void OpenAddClient() => throw new NotImplementedException();
    public void OpenAddBackofficeUser() => throw new NotImplementedException();
    public void OpenAddInsurance(Vehicle vehicle) => throw new NotImplementedException();
    public void OpenAddVehicleEvent(Vehicle vehicle) => throw new NotImplementedException();
    public void OpenCreateRepair(Vehicle vehicle, BrokenDownEvent brokenDown) => throw new NotImplementedException();
}

