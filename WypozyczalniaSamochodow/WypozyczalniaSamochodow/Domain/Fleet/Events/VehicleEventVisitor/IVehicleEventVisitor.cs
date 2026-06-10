namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;

internal interface IVehicleEventVisitor<out T>
{
    T Visit(ReservationEvent ev);
    T Visit(BrokenDownEvent ev);
    T Visit(RepairEvent ev);
    T Visit(MaintenanceEvent ev);
    T Visit(InspectionEvent ev);
    T Visit(DetailingEvent ev);
    T Visit(SuspendedEvent ev);
}
