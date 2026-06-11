using Spectre.Console;

using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation;

internal sealed class UiRenderer : IUiRenderer
{
    private const string TabSeparator = " | ";
    private const string ActiveTabSuffix = " <";
    private const string InactiveTabSuffix = "  ";

    private readonly ITextStyler _styler;
    private readonly IDomainViewFormatter _formatter;

    public UiRenderer(ITextStyler styler, IDomainViewFormatter formatter)
    {
        _styler = styler;
        _formatter = formatter;
    }

    public void Clear() => AnsiConsole.Clear();

    public void WriteLine() => AnsiConsole.WriteLine();

    public void Line(string text, UiRole role = UiRole.Default) => AnsiConsole.MarkupLine(_styler.Colorize(text, role));

    public void Hint(params HintItem[] items) => Hint(string.Empty, items);

    public void Hint(string prefix, params HintItem[] items)
    {
        var parts = items.Select(i => $"[{i.Key.Label}] {i.Description}");
        var body = string.Join("  ", parts);
        if (prefix.Length > 0)
            body = body.Length > 0 ? $"{prefix}  {body}" : prefix;
        Line(body, UiRole.Muted);
    }

    public void Heading(string title) =>
        AnsiConsole.Write(new Rule(_styler.Colorize(title, UiRole.Heading)).Centered());

    public void Banner(string text) =>
        AnsiConsole.Write(new FigletText(text).Centered().Color(Theme.ColorOf(UiRole.Heading)));

    public void Render(UiTable table)
    {
        var t = new Table().Border(TableBorder.Rounded).BorderColor(Theme.ColorOf(UiRole.Muted));
        foreach (var (text, role) in table.Columns)
            t.AddColumn(_styler.Colorize(text, role));
        foreach (var row in table.Rows)
        {
            if (row.Length == 0) t.AddEmptyRow();
            else t.AddRow(row);
        }
        AnsiConsole.Write(t);
    }

    public ConsoleKeyInfo ReadKey()
    {
        var key = AnsiConsole.Console.Input.ReadKey(true);
        return key ?? throw new InvalidOperationException("Nie można odczytać klawisza z wejścia konsoli.");
    }

    public string Menu(string title, IEnumerable<string> choices) =>
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title(_styler.Colorize(title, UiRole.Accent)).AddChoices(choices));

    public void RunWithStatus(string message, Action action) =>
        AnsiConsole.Status().Start(message, _ => action());

    public void WaitForKey(string? message = null)
    {
        AnsiConsole.WriteLine(message ?? UiStrings.PressAnyKey);
        AnsiConsole.Console.Input.ReadKey(true);
    }

    public void Error(string message) { Line(message, UiRole.Error); WaitForKey(); }
    public void Success(string message) { Line(message, UiRole.Success); WaitForKey(); }

    public bool ConfirmDelete(string entityType, string entityName) =>
        AnsiConsole.Confirm(_styler.Colorize(string.Format(UiStrings.ConfirmDelete, entityType, entityName), UiRole.Error));

    public bool ConfirmCancel(string entityType, string entityName) =>
        AnsiConsole.Confirm(_styler.Colorize(string.Format(UiStrings.ConfirmCancel, entityType, entityName), UiRole.Warning));

    public void DrawTabs(string[] names, int activeIndex)
    {
        var parts = new List<string>();
        for (int i = 0; i < names.Length; i++)
        {
            bool active = i == activeIndex;
            var prefix = active ? UiStrings.RowSelected : UiStrings.RowUnselected;
            var suffix = active ? ActiveTabSuffix : InactiveTabSuffix;
            parts.Add(_styler.Colorize($"{prefix}{names[i]}{suffix}", active ? UiRole.Accent : UiRole.Muted));
        }
        AnsiConsole.MarkupLine(string.Join(TabSeparator, parts));
    }

    public string Colorize(string text, UiRole role) => _styler.Colorize(text, role);
    public string Highlight(string text) => _styler.Highlight(text);

    public UiTable CreateDetailsTable() => _formatter.CreateDetailsTable();
    public string FormatDate(DateOnly? date) => _formatter.FormatDate(date);
    public string EventLabel(VehicleEvent ev) => _formatter.EventLabel(ev);
    public string EventLabelColored(VehicleEvent ev, bool active) => _formatter.EventLabelColored(ev, active);
    public string VehicleStatus(Vehicle vehicle) => _formatter.VehicleStatus(vehicle);
    public string ReservationStatus(Reservation reservation) => _formatter.ReservationStatus(reservation);
    public string PaymentLabel(Payment payment) => _formatter.PaymentLabel(payment);
    public void AddVehicleRows(UiTable table, Vehicle vehicle) => _formatter.AddVehicleRows(table, vehicle);
    public void AddReservationRows(UiTable table, Reservation reservation) => _formatter.AddReservationRows(table, reservation);
}
