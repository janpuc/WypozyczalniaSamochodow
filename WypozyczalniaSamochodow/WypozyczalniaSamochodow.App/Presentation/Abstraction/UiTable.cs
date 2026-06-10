using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal sealed class UiTable
{
    private readonly List<(string Text, UiRole Role)> _columns = new();
    private readonly List<string[]> _rows = new();

    public IReadOnlyList<(string Text, UiRole Role)> Columns => _columns;
    public IReadOnlyList<string[]> Rows => _rows;

    public UiTable AddColumn(string text, UiRole role = UiRole.Default)
    {
        _columns.Add((text, role));
        return this;
    }

    public UiTable AddColumns(params string[] texts)
    {
        foreach (var text in texts)
            _columns.Add((text, UiRole.Default));
        return this;
    }

    public UiTable AddRow(params string[] cells)
    {
        _rows.Add(cells);
        return this;
    }

    public UiTable AddEmptyRow()
    {
        _rows.Add(Array.Empty<string>());
        return this;
    }
}
