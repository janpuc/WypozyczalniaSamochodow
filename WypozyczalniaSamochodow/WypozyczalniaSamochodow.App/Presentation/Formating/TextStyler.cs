using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

using Spectre.Console;

namespace WypozyczalniaSamochodow.App.Presentation.Formating;

internal sealed class TextStyler : ITextStyler
{
    public string Colorize(string text, UiRole role) =>
        ConsoleMarkup.Wrap(Markup.Escape(text), Theme.MarkupOf(role));

    public string Highlight(string text) =>
        ConsoleMarkup.Wrap(Markup.Escape(text), Theme.SelectionStyle);
}
