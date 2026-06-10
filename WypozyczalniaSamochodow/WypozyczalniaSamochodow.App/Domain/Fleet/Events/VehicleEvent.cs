using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet.Events;

internal abstract class VehicleEvent
{
    public DateRange Period { get; private set; }
    public string? Description { get; private set; }

    protected VehicleEvent(DateRange period, string? description = null)
    {
        Period = period;
        Description = description;
    }
    
    public DateOnly FromDate => Period.From;
    public DateOnly? ToDate => Period.To;
    public DateOnly EffectiveTo => Period.EffectiveTo;

    public abstract string Describe();
    public abstract T Accept<T>(IVehicleEventVisitor<T> visitor);

    public virtual bool ConflictsWith(VehicleEvent other) => other.Accept(new ConflictPolicyVisitor(this));

    public bool IsActiveOn(DateOnly date) => Period.Contains(date);
    public bool IsPastOn(DateOnly date) => Period.To.HasValue && Period.To.Value < date;

    internal void ReplacePeriod(DateRange period) => Period = period;
    internal void UpdateDescription(string? description) => Description = description;
}
