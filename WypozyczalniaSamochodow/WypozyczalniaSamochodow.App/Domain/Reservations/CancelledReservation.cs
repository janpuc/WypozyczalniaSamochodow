namespace WypozyczalniaSamochodow.App.Domain.Reservations;

internal sealed class CancelledReservation : ReservationStatus
{
    public override string Label => "Anulowana";
}
