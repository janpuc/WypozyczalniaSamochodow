using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Formating;

internal sealed record EventMetadata(string DisplayName, UiRole Role);

internal sealed class EventMetadataVisitor : IVehicleEventVisitor<EventMetadata>
{
    public EventMetadata Visit(ReservationEvent _) => new(UiStrings.EventReservation, UiRole.Info);
    public EventMetadata Visit(BrokenDownEvent _) => new(UiStrings.EventBrokenDown, UiRole.Error);
    public EventMetadata Visit(RepairEvent _) => new(UiStrings.EventRepair, UiRole.Error);
    public EventMetadata Visit(MaintenanceEvent _) => new(UiStrings.EventMaintenance, UiRole.Warning);
    public EventMetadata Visit(InspectionEvent _) => new(UiStrings.EventInspection, UiRole.Warning);
    public EventMetadata Visit(DetailingEvent _) => new(UiStrings.EventDetailing, UiRole.Cosmetic);
    public EventMetadata Visit(SuspendedEvent _) => new(UiStrings.EventSuspended, UiRole.Error);
}

