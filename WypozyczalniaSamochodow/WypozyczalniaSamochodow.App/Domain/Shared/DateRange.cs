namespace WypozyczalniaSamochodow.App.Domain.Shared;

internal sealed record DateRange
{
    public DateOnly From { get; }
    public DateOnly? To { get; }

    public DateRange(DateOnly from, DateOnly? to) { 
        if (to.HasValue && to.Value < from)
            throw new DomainException("Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
        From = from;
        To = to;
    }

    public static DateRange Closed(DateOnly from, DateOnly to) => new(from, to);
    public static DateRange OpenEnded(DateOnly from) => new(from, null);

    public DateOnly EffectiveTo => To ?? DateOnly.MaxValue;

    public bool Overlaps(DateRange other) =>
        From <= other.EffectiveTo && EffectiveTo >= other.From;

    public bool Contains(DateOnly date) =>
        From <= date && EffectiveTo >= date;

    public int Days => To.HasValue
        ? To.Value.DayNumber - From.DayNumber + 1
        : throw new DomainException("Przedział otwarty nie ma określonej liczby dni.");
}
