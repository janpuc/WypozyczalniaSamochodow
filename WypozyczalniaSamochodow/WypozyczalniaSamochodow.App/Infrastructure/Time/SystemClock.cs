using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Infrastructure.Time;

internal sealed class SystemClock(TimeProvider? timeProvider = null) : IClock
{
    private readonly TimeProvider _timeProvider = timeProvider ?? TimeProvider.System;

    public DateTime Now => _timeProvider.GetLocalNow().DateTime;
}
