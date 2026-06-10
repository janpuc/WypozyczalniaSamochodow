using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet;

internal sealed record Vin
{
    public string Value { get; }
    public Vin(string value) { throw new NotImplementedException(); }
    public override string ToString() => Value;
}
