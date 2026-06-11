using Spectre.Console;

namespace WypozyczalniaSamochodow.App.Presentation.UIConfig;

internal static class Theme
{
    public static Color ColorOf(UiRole role) => role switch
    {
        UiRole.Success => Color.Green,
        UiRole.Error => Color.Red,
        UiRole.Warning => Color.Yellow,
        UiRole.Muted => Color.Grey,
        UiRole.Heading => Color.Green,
        UiRole.Prompt => Color.Green,
        UiRole.Accent => Color.Yellow,
        UiRole.Info => Color.Blue,
        UiRole.Cosmetic => Color.Aqua,
        _ => Color.Default,
    };

    public static string MarkupOf(UiRole role) =>
        role == UiRole.Default ? string.Empty : ColorOf(role).ToMarkup();

    public static string SelectionStyle =>
        $"{SelectionForeground.ToMarkup()} on {SelectionBackground.ToMarkup()}";

    private static Color SelectionForeground => Color.Black;
    private static Color SelectionBackground => Color.Yellow;
}
