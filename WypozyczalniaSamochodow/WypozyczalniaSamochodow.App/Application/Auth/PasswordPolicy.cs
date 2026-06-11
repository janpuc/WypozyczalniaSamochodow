namespace WypozyczalniaSamochodow.App.Application.Auth;

internal static class PasswordPolicy
{
    public const int MinimumLength = 8;

    public static bool IsSatisfiedBy(string? plainPassword) =>
        !string.IsNullOrWhiteSpace(plainPassword) && plainPassword.Length >= MinimumLength;
}
