using OrlenSolutions.Domain.Shared;

namespace OrlenSolutions.Domain.Fleet;

internal sealed record RegistrationNumber
{
    public string Value { get; }
    public RegistrationNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Numer rejestracyjny nie może być pusty.");
        Value = value.ToUpperInvariant();
    }
    public override string ToString() => Value;
}
