using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;

namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal interface IDomainViewFormatter
{
    UiTable CreateDetailsTable();
    string FormatDate(DateOnly? date);
    string EventLabel(VehicleEvent ev);
    string EventLabelColored(VehicleEvent ev, bool active);
    string VehicleStatus(Vehicle vehicle);
    string ReservationStatus(Reservation reservation);
    string PaymentLabel(Payment payment);
    void AddVehicleRows(UiTable table, Vehicle vehicle);
    void AddReservationRows(UiTable table, Reservation reservation);
}
