namespace WypozyczalniaSamochodow.App.Domain.Users;

internal sealed partial record Email
{
    public string Value { get; }

    public Email(string value)
    {

    }

    private Email(string normalized, bool _) => Value = normalized;

    public static bool TryCreate(string value, out Email email)
    {
        throw new NotImplementedException();
    }

    private static bool TryNormalize(string? value, out string normalized)
    {
        throw new NotImplementedException();
    }

    public override string ToString() => Value;


}
