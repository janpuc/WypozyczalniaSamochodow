using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.Tests.TestSupport;

internal sealed class ScriptedUiRenderer : IUiRenderer
{
    private readonly Queue<string> _menuChoices = new();
    private readonly Queue<ConsoleKeyInfo> _keys = new();
    private readonly Queue<bool> _confirmations = new();
    private readonly List<string> _lines = new();
    private readonly List<string> _errors = new();
    private readonly List<string> _successes = new();
    private readonly List<UiTable> _renderedTables = new();

    public IReadOnlyList<string> Lines => _lines;
    public IReadOnlyList<string> Errors => _errors;
    public IReadOnlyList<string> Successes => _successes;
    public IReadOnlyList<UiTable> RenderedTables => _renderedTables;
    public int WaitForKeyCalls { get; private set; }

    public ScriptedUiRenderer EnqueueMenu(params string[] choices)
    {
        foreach (var choice in choices) _menuChoices.Enqueue(choice);
        return this;
    }

    public void AddReservationRows(UiTable table, Reservation reservation)
    {
        throw new NotImplementedException();
    }

    public void AddVehicleRows(UiTable table, Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public void Banner(string text)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public string Colorize(string text, UiRole role)
    {
        throw new NotImplementedException();
    }

    public bool ConfirmCancel(string entityType, string entityName)
    {
        throw new NotImplementedException();
    }

    public bool ConfirmDelete(string entityType, string entityName)
    {
        throw new NotImplementedException();
    }

    public UiTable CreateDetailsTable()
    {
        throw new NotImplementedException();
    }

    public void DrawTabs(string[] names, int activeIndex)
    {
        throw new NotImplementedException();
    }

    public ScriptedUiRenderer EnqueueKeys(params ConsoleKey[] keys)
    {
        foreach (var key in keys) _keys.Enqueue(new ConsoleKeyInfo('\0', key, false, false, false));
        return this;
    }

    public void Error(string message)
    {
        throw new NotImplementedException();
    }

    public string EventLabel(VehicleEvent ev)
    {
        throw new NotImplementedException();
    }

    public string EventLabelColored(VehicleEvent ev, bool active)
    {
        throw new NotImplementedException();
    }

    public string FormatDate(DateOnly? date)
    {
        throw new NotImplementedException();
    }

    public void Heading(string title)
    {
        throw new NotImplementedException();
    }

    public string Highlight(string text)
    {
        throw new NotImplementedException();
    }

    public void Hint(params HintItem[] items)
    {
        throw new NotImplementedException();
    }

    public void Hint(string prefix, params HintItem[] items)
    {
        throw new NotImplementedException();
    }

    public void Line(string text, UiRole role = UiRole.Default)
    {
        throw new NotImplementedException();
    }

    public string Menu(string title, IEnumerable<string> choices)
    {
        throw new NotImplementedException();
    }

    public string PaymentLabel(Payment payment)
    {
        throw new NotImplementedException();
    }

    public ConsoleKeyInfo ReadKey()
    {
        throw new NotImplementedException();
    }

    public void Render(UiTable table)
    {
        throw new NotImplementedException();
    }

    public string ReservationStatus(Reservation reservation)
    {
        throw new NotImplementedException();
    }

    public void RunWithStatus(string message, Action action)
    {
        throw new NotImplementedException();
    }

    public void Success(string message)
    {
        throw new NotImplementedException();
    }

    public string VehicleStatus(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public void WaitForKey(string? message = null)
    {
        throw new NotImplementedException();
    }

    public void WriteLine()
    {
        throw new NotImplementedException();
    }
}
