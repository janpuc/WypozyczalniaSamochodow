using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events;

internal sealed class ReservationEvent : VehicleEvent
{
    public ReservationEvent(DateRange period, string? description = null) : base(period, description)
    {
        if (period.To is null)
            throw new DomainException("Rezerwacja musi mieć datę zakończenia.");
    }
    
    public override string Describe() => "Rezerwacja";
    public override T Accept<T>(IVehicleEventVisitor<T> visitor) => visitor.Visit(this);
}
