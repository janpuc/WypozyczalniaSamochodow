using WypozyczalniaSamochodow.App.Domain.Shared;
namespace WypozyczalniaSamochodow.App.Domain.Reservations;

internal sealed class PendingReservation : ReservationStatus
{
    public override string Label => "Oczekująca";

    public override bool CanActivate => true;
    public override bool CanCancel => true;

    public override ReservationStatus Activate(int mileageBefore)
    {
        if (mileageBefore < 0)
            throw new DomainException("Przebieg nie może być ujemny.");
        return new ActiveReservation(mileageBefore);
    }

    public override ReservationStatus Cancel() => new CancelledReservation();
}
