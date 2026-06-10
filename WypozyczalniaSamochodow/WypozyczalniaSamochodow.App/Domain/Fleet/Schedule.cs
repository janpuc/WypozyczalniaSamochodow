using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet;

internal sealed class Schedule
{
    private readonly List<VehicleEvent> _events = new();
    private readonly IsReservationVisitor _isReservationVisitor = new();

    public IReadOnlyList<VehicleEvent> Events => _events;

    public void Add(VehicleEvent ev) { throw new NotImplementedException(); }

    public void Remove(VehicleEvent ev) => _events.Remove(ev);
    
    public void Reschedule(VehicleEvent ev, DateRange newPeriod) { throw new NotImplementedException(); }

    public bool WouldConflict(VehicleEvent newOne) =>
        _events.Any(existing => newOne.ConflictsWith(existing));

    public IEnumerable<VehicleEvent> ConflictingNonReservationEvents(DateRange range) =>
        _events.Where(e => !e.Accept(_isReservationVisitor) && e.Period.Overlaps(range));

    public VehicleEvent? ActiveNonReservationOn(DateOnly date) =>
        _events.FirstOrDefault(e => !e.Accept(_isReservationVisitor) && e.IsActiveOn(date));

    public BrokenDownEvent? ActiveBrokenDownOn(DateOnly date) =>
        _events.OfType<BrokenDownEvent>().FirstOrDefault(e => e.IsActiveOn(date));

    public IEnumerable<VehicleEvent> NonReservationEvents => _events.Where(e => !e.Accept(_isReservationVisitor));
}
