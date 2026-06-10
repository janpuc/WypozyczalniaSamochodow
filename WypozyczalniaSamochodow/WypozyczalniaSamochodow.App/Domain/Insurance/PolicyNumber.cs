using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Insurance;

internal sealed record PolicyNumber
{
    public string Value { get; }
    public PolicyNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Numer polisy nie może być pusty.");

        Value = value.Trim().ToUpperInvariant();
    }
    public override string ToString() => Value;
}
