using Microsoft.Extensions.DependencyInjection;

using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Screens;
using WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;
using WypozyczalniaSamochodow.App.Presentation.Screens.Client;

namespace WypozyczalniaSamochodow.App.Presentation.Navigation;

internal sealed class ScreenNavigator : INavigator
{
    private readonly IServiceProvider _provider;

    public ScreenNavigator(IServiceProvider provider) => _provider = provider;

    private void Open<TScreen>(params object[] runtimeArguments) where TScreen : IScreen =>
        ActivatorUtilities.CreateInstance<TScreen>(_provider, runtimeArguments).Run();

    public void OpenLogin() => Open<LoginScreen>();
    public void OpenRegister() => Open<RegisterScreen>();
    public void OpenBackofficeDashboard() => Open<BackofficeDashboardScreen>();
    public void OpenClientDashboard(Domain.Users.Client client) => Open<ClientDashboardScreen>(client);
    public void OpenNewReservation(Domain.Users.Client client) => Open<NewReservationFlow>(client);
    public void OpenClientReservationDetails(Reservation reservation) => Open<ClientReservationDetailsScreen>(reservation);
    public void OpenReservationDetails(Reservation reservation) => Open<ReservationDetailsScreen>(reservation);
    public void OpenVehicleDetails(Vehicle vehicle) => Open<VehicleDetailsScreen>(vehicle);
    public void OpenClientDetails(Domain.Users.Client client) => Open<ClientDetailsScreen>(client);
    public void OpenBackofficeUserDetails(Domain.Users.Backoffice user) => Open<BackofficeUserDetailsScreen>(user);
    public void OpenAddVehicle() => Open<AddVehicleScreen>();
    public void OpenAddClient() => Open<AddClientScreen>();
    public void OpenAddBackofficeUser() => Open<AddBackofficeUserScreen>();
    public void OpenAddInsurance(Vehicle vehicle) => Open<AddInsuranceScreen>(vehicle);
    public void OpenAddVehicleEvent(Vehicle vehicle) => Open<AddVehicleEventScreen>(vehicle);
    public void OpenCreateRepair(Vehicle vehicle, BrokenDownEvent brokenDown) => Open<CreateRepairScreen>(vehicle, brokenDown);
}


