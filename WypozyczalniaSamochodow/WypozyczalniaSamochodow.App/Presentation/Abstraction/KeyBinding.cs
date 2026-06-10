namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal sealed class KeyBinding
{
    private readonly ConsoleKey[] _keys;

    public KeyBinding(string label, params ConsoleKey[] keys)
    {
        Label = label;
        _keys = keys;
    }

    public string Label { get; }

    public bool Matches(ConsoleKeyInfo pressed) => Array.IndexOf(_keys, pressed.Key) >= 0;
}
