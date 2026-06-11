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

    public ScriptedUiRenderer EnqueueKeys(params ConsoleKey[] keys)
    {
        foreach (var key in keys) _keys.Enqueue(new ConsoleKeyInfo('\0', key, false, false, false));
        return this;
    }

    public ScriptedUiRenderer EnqueueConfirmations(params bool[] values)
    {
        foreach (var value in values) _confirmations.Enqueue(value);
        return this;
    }

    public void Clear() { }
    public void WriteLine() { }
    public void Line(string text, UiRole role = UiRole.Default) => _lines.Add(text);
    public void Hint(params HintItem[] items) { }
    public void Hint(string prefix, params HintItem[] items) { }
    public void Heading(string title) { }
    public void Banner(string text) { }
    public void Render(UiTable table) => _renderedTables.Add(table);

    public ConsoleKeyInfo ReadKey() => _keys.Count > 0
        ? _keys.Dequeue()
        : new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false);

    public string Menu(string title, IEnumerable<string> choices) => _menuChoices.Count > 0
        ? _menuChoices.Dequeue()
        : choices.First();

    public void RunWithStatus(string message, Action action) => action();

    public void WaitForKey(string? message = null) => WaitForKeyCalls++;

    public void Error(string message) => _errors.Add(message);
    public void Success(string message) => _successes.Add(message);

    public bool ConfirmDelete(string entityType, string entityName) => _confirmations.Count > 0 ? _confirmations.Dequeue() : true;
    public bool ConfirmCancel(string entityType, string entityName) => _confirmations.Count > 0 ? _confirmations.Dequeue() : true;

    public void DrawTabs(string[] names, int activeIndex) { }

    public string Colorize(string text, UiRole role) => text;
    public string Highlight(string text) => text;

    public UiTable CreateDetailsTable() => new();
    public string FormatDate(DateOnly? date) => date?.ToString(UiFormats.Date) ?? UiStrings.DateEmpty;
    public string EventLabel(VehicleEvent ev) => ev.Describe();
    public string EventLabelColored(VehicleEvent ev, bool active) => ev.Describe();
    public string VehicleStatus(Vehicle vehicle) => "status";
    public string ReservationStatus(Reservation reservation) => reservation.Status.Label;
    public string PaymentLabel(Payment payment) => payment.Describe();
    public void AddVehicleRows(UiTable table, Vehicle vehicle) { }
    public void AddReservationRows(UiTable table, Reservation reservation) { }
}
