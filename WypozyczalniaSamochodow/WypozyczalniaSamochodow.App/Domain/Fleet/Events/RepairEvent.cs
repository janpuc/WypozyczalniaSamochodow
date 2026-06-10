using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events;

internal sealed class RepairEvent : VehicleEvent
{
    public BrokenDownEvent Cause { get; }

    public RepairEvent(DateRange period, BrokenDownEvent cause, string? description = null) : base(period, description)
    {
        if (period.To is null)
            throw new DomainException("Naprawa musi mieć datę zakończenia.");
        Cause = cause ?? throw new DomainException("Naprawa musi być powiązana z awarią.");
    }
    
    public override string Describe() => "Naprawa";
    public override T Accept<T>(IVehicleEventVisitor<T> visitor) => visitor.Visit(this);

}
