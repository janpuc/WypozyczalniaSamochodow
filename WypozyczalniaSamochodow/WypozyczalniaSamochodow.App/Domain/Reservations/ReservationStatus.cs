using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Reservations;

internal abstract class ReservationStatus
{
    public abstract string Label { get; }

    public virtual bool CanActivate => false;
    public virtual bool CanComplete => false;
    public virtual bool CanCancel => false;

    public virtual ReservationStatus Activate(int mileageBefore) =>
        throw new DomainException($"Nie można aktywować rezerwacji w stanie {Label}.");
    public virtual ReservationStatus Complete(int mileageAfter, string? note) =>
        throw new DomainException($"Nie można zakończyć rezerwacji w stanie {Label}.");
    public virtual ReservationStatus Cancel() =>
        throw new DomainException($"Nie można anulować rezerwacji w stanie {Label}.");

    public virtual int? MileageBefore => null;
    public virtual int? MileageAfter => null;
    public virtual string? CompletionNote => null;
}
