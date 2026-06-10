using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Domain.Reservations;

internal sealed class Reservation
{
    public Client Client { get; }
    public Vehicle Vehicle { get; private set; }
    public ReservationEvent Event { get; private set; }
    public ReservationStatus Status { get; private set; }
    public Payment Payment { get; private set; }

    public Reservation(Client client, Vehicle vehicle, DateRange period, Payment payment, IClock clock)
    {
        if (period.To is null)
            throw new DomainException("Rezerwacja musi mieć datę zakończenia.");
        if (period.From < clock.Today)
            throw new DomainException("Data rozpoczęcia nie może być w przeszłości.");
        client.EnsureCanRent(clock);

        Client = client;
        Vehicle = vehicle;
        Event = vehicle.Reserve(period, clock);
        Payment = payment;
        Status = new PendingReservation();
    }

    private Reservation(Client client, Vehicle vehicle, ReservationEvent ev, Payment payment, ReservationStatus status)
    {
        Client = client;
        Vehicle = vehicle;
        Event = ev;
        Payment = payment;
        Status = status;
    }

    public static Reservation Rehydrate(Client client, Vehicle vehicle, ReservationEvent ev, Payment payment, ReservationStatus status) =>
        new(client, vehicle, ev, payment, status);

    public bool CanActivate => Status.CanActivate;
    public bool CanComplete => Status.CanComplete;
    public bool CanCancel => Status.CanCancel;
    public bool CanSwapVehicle => Status is PendingReservation;

    public void Activate(int mileageBefore, IClock clock)
    {
      Client.EnsureCanRent(clock);
      Status = Status.Activate(mileageBefore);
    }
    public void Complete(int mileageAfter, string? note) => Status = Status.Complete(mileageAfter, note);
    public void Cancel()
    {
        Status = Status.Cancel();
        Vehicle.ReleaseReservation(Event);
    }

    public void Reschedule(DateRange newPeriod)
    {
        if (newPeriod.To is null)
            throw new DomainException("Rezerwacja musi mieć datę zakończenia.");
        Vehicle.RescheduleReservation(Event, newPeriod);
    }

    public void SwapVehicle(Vehicle newVehicle, IClock clock)
    {
        if (Status is not PendingReservation)
            throw new DomainException("Pojazd można wymienić tylko dla rezerwacji oczekującej.");
        if (!newVehicle.IsAvailableFor(Event.Period, clock))
            throw new DomainException("Nowy pojazd nie jest dostępny w wybranym terminie.");
        Vehicle.ReleaseReservation(Event);
        Vehicle = newVehicle;
        newVehicle.AddEvent(Event);
    }

    public bool BelongsTo(Client client) => ReferenceEquals(Client, client);
    public bool BelongsTo(Vehicle vehicle) => ReferenceEquals(Vehicle, vehicle);
}
