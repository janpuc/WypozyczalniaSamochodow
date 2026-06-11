using System.Globalization;

using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Presentation.UIConfig;

internal static class UiFormats
{
    private static readonly CultureInfo PolishCulture = CultureInfo.GetCultureInfo("pl-PL");

    public const string Date = "yyyy-MM-dd";
    public const string DateTime = "yyyy-MM-dd HH:mm";

    public static string Money(Money money) => money.Value.ToString("C", PolishCulture);
}
