using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal interface ITextStyler
{
    string Colorize(string text, UiRole role);
    string Highlight(string text);
}
