using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events;

internal sealed class BrokenDownEvent : VehicleEvent
{
    public RepairEvent? LinkedRepair { get; private set; }

    public BrokenDownEvent(DateRange period, string? description = null) : base(period, description) { }

    public override string Describe() => "Niesprawny";
    public override T Accept<T>(IVehicleEventVisitor<T> visitor) => visitor.Visit(this);

    internal RepairEvent RegisterRepair(DateRange repairPeriod, string? description = null) {
        if (repairPeriod.From < Period.From)
            throw new DomainException("Naprawa nie może zaczynać się przed awarią.");

        var repair = new RepairEvent(repairPeriod, this, description);
        LinkedRepair = repair;
        ReplacePeriod(new DateRange(Period.From, repair.FromDate));
        return repair;
    }

}
