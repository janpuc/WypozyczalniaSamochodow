using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal static class ScreenActions
{
    public static bool Guard(this IUiRenderer ui, Action action)
    {
        try
        {
            action();
            return true;
        }
        catch (DomainException ex)
        {
            ui.Error(ex.Message);
            return false;
        }
    }
}
