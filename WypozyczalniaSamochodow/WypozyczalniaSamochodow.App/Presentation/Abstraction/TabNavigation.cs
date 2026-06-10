namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal static class TabNavigation
{
    public static int Cycle(int currentIndex, int tabCount, ConsoleKeyInfo key) =>
        key.Modifiers.HasFlag(ConsoleModifiers.Shift)
            ? (currentIndex + tabCount - 1) % tabCount
            : (currentIndex + 1) % tabCount;

    public static int ClampSelection(int selectedIndex, int itemCount) =>
        selectedIndex >= itemCount ? Math.Max(0, itemCount - 1) : selectedIndex;
}
