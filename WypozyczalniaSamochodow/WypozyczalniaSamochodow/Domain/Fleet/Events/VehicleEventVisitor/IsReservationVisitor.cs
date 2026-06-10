namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;

internal sealed class IsReservationVisitor : IVehicleEventVisitor<bool>
{
    public bool Visit(ReservationEvent _) => true;
    public bool Visit(BrokenDownEvent _) => false;
    public bool Visit(RepairEvent _) => false;
    public bool Visit(MaintenanceEvent _) => false;
    public bool Visit(InspectionEvent _) => false;
    public bool Visit(DetailingEvent _) => false;
    public bool Visit(SuspendedEvent _) => false;
}
