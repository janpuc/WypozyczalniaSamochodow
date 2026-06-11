namespace WypozyczalniaSamochodow.App.Presentation.UIConfig;

internal static class StringNormalization
{
    public static string? NullIfBlank(this string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
