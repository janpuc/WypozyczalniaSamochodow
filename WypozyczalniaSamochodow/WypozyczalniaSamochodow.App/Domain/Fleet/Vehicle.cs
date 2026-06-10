using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events.VehicleEventVisitor;
using WypozyczalniaSamochodow.App.Domain.Insurance;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet;

internal sealed class Vehicle
{
    private static readonly CanRemoveVisitor _canRemoveVisitor = new();

    private readonly List<Insurance.Insurance> _insurances = new();
    private readonly Schedule _schedule = new();

    public string Make { get; private set; }
    public string Model { get; private set; }
    public RegistrationNumber Registration { get; private set; }
    public Vin Vin { get; private set; }
    public string Color { get; private set; }
    public Money PricePerDay { get; private set; }
    public int Year { get; private set; }
    public DateOnly PurchaseDate { get; private set; }

    public IReadOnlyCollection<Insurance.Insurance> Insurances => _insurances;
    public Schedule Schedule => _schedule;

    public Vehicle(string make, string model, RegistrationNumber registration, Vin vin,
        string color, Money pricePerDay, int year, DateOnly purchaseDate) { throw new NotImplementedException(); }

    public bool HasActiveInsuranceOn(DateOnly date) => _insurances.Any(i => i.IsValidOn(date));

    public void AddInsurance(Insurance.Insurance insurance) => _insurances.Add(insurance);

    public void RemoveInsurance(Insurance.Insurance insurance) { throw new NotImplementedException(); }

    private bool WouldLeaveReservationUninsured(Insurance.Insurance toRemove) =>
        _schedule.Events.OfType<ReservationEvent>().Any(reservation =>
            toRemove.Covers(reservation.Period) &&
            !_insurances.Any(other => !ReferenceEquals(other, toRemove) && other.Covers(reservation.Period)));

    public void AddEvent(VehicleEvent ev) => _schedule.Add(ev);
    public void ReleaseReservation(ReservationEvent ev) => _schedule.Remove(ev);

    public ReservationEvent Reserve(DateRange range, IClock clock) { throw new NotImplementedException(); }

    public void RescheduleReservation(ReservationEvent reservation, DateRange newRange) { throw new NotImplementedException(); }

    public void RemoveEvent(VehicleEvent ev) { throw new NotImplementedException(); }

    public void Rename(string make, string model) { throw new NotImplementedException(); }

    public void ChangeRegistration(RegistrationNumber registration) => Registration = registration;
    public void ChangeVin(Vin vin) => Vin = vin;

    public void Repaint(string color) { throw new NotImplementedException(); }

    public void Reprice(Money newPrice) { throw new NotImplementedException(); }

    public void SetYear(int year) { throw new NotImplementedException(); }

    public void SetPurchaseDate(DateOnly date) => PurchaseDate = date;

    private static void EnsureMakeProvided(string make) { throw new NotImplementedException(); }

    private static void EnsureModelProvided(string model) { throw new NotImplementedException(); }

    private static void EnsureColorProvided(string color) { throw new NotImplementedException(); }

    private static void EnsurePricePositive(Money price) { throw new NotImplementedException(); }

    private static void EnsureYearValid(int year) { throw new NotImplementedException(); }

    public bool IsAvailableFor(DateRange range, IClock clock) =>
        !_schedule.Events.Any(e => e.Period.Overlaps(range)) &&
        _insurances.Any(i => i.Covers(range)) &&
        _schedule.ActiveNonReservationOn(clock.Today) is null;
}
