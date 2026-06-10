using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events;

internal sealed class DetailingEvent : VehicleEvent
{
    public DetailingEvent(DateRange period, string? description = null) : base(period, description) { }
    public override string Describe() => "Detailing pojazdu";
    public override T Accept<T>(IVehicleEventVisitor<T> visitor) => visitor.Visit(this);
}
