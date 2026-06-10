using System.Text.RegularExpressions;
using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Users;

internal sealed partial record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (!TryNormalize(value, out var normalized))
            throw new DomainException("Podaj prawidłowy adres email.");
        Value = normalized;
    }

    private Email(string normalized, bool _) => Value = normalized;

    public static bool TryCreate(string value, out Email email)
    {
        if (TryNormalize(value, out var normalized))
        {
            email = new Email(normalized, true);
            return true;
        }
        email = null!;
        return false;
    }

    private static bool TryNormalize(string? value, out string normalized)
    {
        normalized = (value ?? string.Empty).Trim().ToLowerInvariant();
        return Pattern().IsMatch(normalized);
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex Pattern();
}
