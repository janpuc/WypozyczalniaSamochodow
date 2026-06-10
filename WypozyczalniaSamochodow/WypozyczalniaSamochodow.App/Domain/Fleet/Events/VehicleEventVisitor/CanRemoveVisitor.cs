namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;

internal sealed class CanRemoveVisitor : IVehicleEventVisitor<bool>
{
    public bool Visit(ReservationEvent _) => false;
    public bool Visit(BrokenDownEvent _) => true;
    public bool Visit(RepairEvent _) => true;
    public bool Visit(MaintenanceEvent _) => true;
    public bool Visit(InspectionEvent _) => true;
    public bool Visit(DetailingEvent _) => true;
    public bool Visit(SuspendedEvent _) => true;
}
