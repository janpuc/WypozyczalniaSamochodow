using WypozyczalniaSamochodow.App.Presentation.Abstraction;

namespace WypozyczalniaSamochodow.App.Presentation.UIConfig;

internal static class Keys
{
    public static readonly KeyBinding Back = new("Esc", ConsoleKey.Escape);
    public static readonly KeyBinding Select = new("Enter", ConsoleKey.Enter);
    public static readonly KeyBinding NextTab = new("Tab", ConsoleKey.Tab);
    public static readonly KeyBinding Up = new("↑", ConsoleKey.UpArrow);
    public static readonly KeyBinding Down = new("↓", ConsoleKey.DownArrow);
    public static readonly KeyBinding Add = new("+", ConsoleKey.Add, ConsoleKey.OemPlus);

    public static readonly KeyBinding Delete = new("D", ConsoleKey.D);
    public static readonly KeyBinding Edit = new("E", ConsoleKey.E);
    public static readonly KeyBinding ResetPassword = new("P", ConsoleKey.P);
    public static readonly KeyBinding Repair = new("R", ConsoleKey.R);
    public static readonly KeyBinding Activate = new("A", ConsoleKey.A);
    public static readonly KeyBinding Complete = new("Z", ConsoleKey.Z);
    public static readonly KeyBinding SwapVehicle = new("W", ConsoleKey.W);
    public static readonly KeyBinding ChangeDates = new("C", ConsoleKey.C);
}
