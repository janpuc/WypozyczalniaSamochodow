namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal readonly record struct HintItem(KeyBinding Key, string Description)
{
    public static implicit operator HintItem((KeyBinding Key, string Description) tuple) =>
        new(tuple.Key, tuple.Description);
}
