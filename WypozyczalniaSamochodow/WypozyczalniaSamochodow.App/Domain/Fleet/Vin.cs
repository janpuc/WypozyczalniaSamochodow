using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Fleet;

internal sealed record Vin
{
    public string Value { get; }
    public Vin(string value) { 
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("VIN nie może być pusty.");
        Value = value.ToUpperInvariant();
    }
    public override string ToString() => Value;
}
