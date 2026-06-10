namespace WypozyczalniaSamochodow.App.Domain.Reservations;

internal sealed class CompletedReservation : ReservationStatus
{
    private readonly int _mileageBefore;
    private readonly int _mileageAfter;
    private readonly string? _note;

    public CompletedReservation(int mileageBefore, int mileageAfter, string? note)
    {
        _mileageBefore = mileageBefore;
        _mileageAfter = mileageAfter;
        _note = note;
    }

    public override string Label => "Zakończona";
    public override int? MileageBefore => _mileageBefore;
    public override int? MileageAfter => _mileageAfter;
    public override string? CompletionNote => _note;
}
