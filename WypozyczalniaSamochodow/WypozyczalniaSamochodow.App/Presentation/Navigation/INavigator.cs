using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;

namespace WypozyczalniaSamochodow.App.Presentation.Navigation;

internal interface INavigator
{
    void OpenLogin();
    void OpenRegister();
    void OpenBackofficeDashboard();
    void OpenClientDashboard(Domain.Users.Client client);
    void OpenNewReservation(Domain.Users.Client client);
    void OpenVehicleDetails(Vehicle vehicle);
    void OpenClientDetails(Domain.Users.Client client);
    void OpenBackofficeUserDetails(Domain.Users.Backoffice user);
    void OpenAddVehicle();
    void OpenAddClient();
    void OpenAddBackofficeUser();
    void OpenAddInsurance(Vehicle vehicle);
    void OpenAddVehicleEvent(Vehicle vehicle);
    void OpenCreateRepair(Vehicle vehicle, BrokenDownEvent brokenDown);
}
