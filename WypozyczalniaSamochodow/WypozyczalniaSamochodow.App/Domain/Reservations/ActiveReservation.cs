using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Reservations;

internal sealed class ActiveReservation : ReservationStatus
{
    private readonly int _mileageBefore;
    public ActiveReservation(int mileageBefore) { _mileageBefore = mileageBefore; }
    public override string Label => "Aktywna";
    public override bool CanComplete => true;
    public override int? MileageBefore => _mileageBefore;

    public override ReservationStatus Complete(int mileageAfter, string? note)
    {
        if (mileageAfter < 0)
            throw new DomainException("Przebieg nie może być ujemny.");
        if (mileageAfter < _mileageBefore)
            throw new DomainException("Przebieg końcowy nie może być mniejszy niż początkowy.");
        return new CompletedReservation(_mileageBefore, mileageAfter, note);
    }
}
