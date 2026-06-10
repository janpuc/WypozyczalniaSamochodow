using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events;

internal sealed class BrokenDownEvent : VehicleEvent
{
    public RepairEvent? LinkedRepair { get; private set; }

    public BrokenDownEvent(DateRange period, string? description = null) : base(period, description) { throw new NotImplementedException(); }

    public override string Describe() => "Niesprawny";
    public override T Accept<T>(IVehicleEventVisitor<T> visitor) => visitor.Visit(this);

    internal RepairEvent RegisterRepair(DateRange repairPeriod, string? description = null) { throw new NotImplementedException(); }

}
