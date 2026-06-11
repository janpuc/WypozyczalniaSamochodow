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
        string color, Money pricePerDay, int year, DateOnly purchaseDate) { 
        EnsureMakeProvided(make);
        EnsureModelProvided(model);
        EnsureColorProvided(color);
        EnsurePricePositive(pricePerDay);
        EnsureYearValid(year);
        Make = make;
        Model = model;
        Registration = registration;
        Vin = vin;
        Color = color;
        PricePerDay = pricePerDay;
        Year = year;
        PurchaseDate = purchaseDate;
    }

    public bool HasActiveInsuranceOn(DateOnly date) => _insurances.Any(i => i.IsValidOn(date));

    public void AddInsurance(Insurance.Insurance insurance) => _insurances.Add(insurance);

    public void RemoveInsurance(Insurance.Insurance insurance) { 
        if (WouldLeaveReservationUninsured(insurance))
            throw new DomainException("Nie można usunąć polisy — rezerwacja straciłaby pokrycie ubezpieczeniem.");
        _insurances.Remove(insurance);
    }

    private bool WouldLeaveReservationUninsured(Insurance.Insurance toRemove) =>
        _schedule.Events.OfType<ReservationEvent>().Any(reservation =>
            toRemove.Covers(reservation.Period) &&
            !_insurances.Any(other => !ReferenceEquals(other, toRemove) && other.Covers(reservation.Period)));

    public void AddEvent(VehicleEvent ev) => _schedule.Add(ev);
    public void ReleaseReservation(ReservationEvent ev) => _schedule.Remove(ev);

    public ReservationEvent Reserve(DateRange range, IClock clock) { 
        if (!IsAvailableFor(range, clock))
            throw new DomainException("Pojazd nie jest dostępny w wybranym terminie.");
        var reservation = new ReservationEvent(range);
        _schedule.Add(reservation);
        return reservation;
    }

    public void RescheduleReservation(ReservationEvent reservation, DateRange newRange) { 
        if (!_insurances.Any(i => i.Covers(newRange)))
            throw new DomainException("Brak ubezpieczenia pokrywającego nowy termin rezerwacji.");
        _schedule.Reschedule(reservation, newRange);
    }

    public void RemoveEvent(VehicleEvent ev) {  
        if (!ev.Accept(_canRemoveVisitor))
            throw new DomainException("Nie można usunąć zdarzenia typu Rezerwacja z tego widoku.");
        _schedule.Remove(ev);
    }

    public void Rename(string make, string model) { 
        EnsureMakeProvided(make);
        EnsureModelProvided(model);
        Make = make;
        Model = model;
    }

    public void ChangeRegistration(RegistrationNumber registration) => Registration = registration;
    public void ChangeVin(Vin vin) => Vin = vin;

    public void Repaint(string color) {
        EnsureColorProvided(color);
        Color = color;
    }

    public void Reprice(Money newPrice) {
        EnsurePricePositive(newPrice);
        PricePerDay = newPrice;
    }

    public void SetYear(int year) {
        EnsureYearValid(year);
        Year = year;
    }

    public void SetPurchaseDate(DateOnly date) => PurchaseDate = date;

    private static void EnsureMakeProvided(string make) {
        if (string.IsNullOrWhiteSpace(make)) throw new DomainException("Marka nie może być pusta.");
    }

    private static void EnsureModelProvided(string model) {
        if (string.IsNullOrWhiteSpace(model)) throw new DomainException("Model nie może być pusty.");
    }

    private static void EnsureColorProvided(string color) {
        if (string.IsNullOrWhiteSpace(color)) throw new DomainException("Kolor nie może być pusty.");
    }

    private static void EnsurePricePositive(Money price) {
        if (price.Value <= 0) throw new DomainException("Cena za dzień musi być większa od zera.");
    }

    private static void EnsureYearValid(int year) {
        if (year < 1900) throw new DomainException("Rok produkcji jest nieprawidłowy.");
    }

    public bool IsAvailableFor(DateRange range, IClock clock) =>
        !_schedule.Events.Any(e => e.Period.Overlaps(range)) &&
        _insurances.Any(i => i.Covers(range)) &&
        _schedule.ActiveNonReservationOn(clock.Today) is null;
}
