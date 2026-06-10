namespace WypozyczalniaSamochodow.App.Domain.Shared;

internal interface IClock
{
    DateTime Now { get; }
    DateOnly Today => DateOnly.FromDateTime(Now);
}
