using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.Tests.TestSupport;

internal sealed class FixedClock : IClock
{
    public FixedClock(DateTime now) => Now = now;

    public FixedClock(DateOnly today) => Now = today.ToDateTime(TimeOnly.MinValue);

    public DateTime Now { get; }
}
