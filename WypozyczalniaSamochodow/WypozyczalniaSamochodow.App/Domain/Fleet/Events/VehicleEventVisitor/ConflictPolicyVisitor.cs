namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;

internal sealed class ConflictPolicyVisitor : IVehicleEventVisitor<bool>
{
    private readonly VehicleEvent _subject;

    public ConflictPolicyVisitor(VehicleEvent subject)
    {
        _subject = subject;
    }

    public bool Visit(ReservationEvent other) => ConflictsWith(other);

    public bool Visit(BrokenDownEvent other) =>
        !ReferenceEquals(_subject, other) &&
        !(_subject is RepairEvent repair && ReferenceEquals(repair.Cause, other)) &&
        ConflictsWith(other);

    public bool Visit(RepairEvent other) =>
        !ReferenceEquals(_subject, other) &&
        !(_subject is BrokenDownEvent brokenDown && ReferenceEquals(other.Cause, brokenDown)) &&
        ConflictsWith(other);

    public bool Visit(MaintenanceEvent other) => ConflictsWith(other);
    public bool Visit(InspectionEvent other) => ConflictsWith(other);
    public bool Visit(DetailingEvent other) => ConflictsWith(other);
    public bool Visit(SuspendedEvent other) => ConflictsWith(other);

    private bool ConflictsWith(VehicleEvent other) =>
        !ReferenceEquals(_subject, other) && _subject.Period.Overlaps(other.Period);
}
