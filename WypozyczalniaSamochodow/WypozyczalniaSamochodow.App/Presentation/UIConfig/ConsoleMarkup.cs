namespace WypozyczalniaSamochodow.App.Presentation.UIConfig;

internal static class ConsoleMarkup
{
    public static string Wrap(string escapedText, string tag) =>
        tag.Length == 0 ? escapedText : $"[{tag}]{escapedText}[/]";
}
