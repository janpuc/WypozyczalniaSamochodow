using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal static class SelectableList
{
    public static UiTable Build<T>(IReadOnlyList<T> items, int selectedIndex, string[] columns, Func<T, string[]> rowFactory)
    {
        var table = new UiTable();
        foreach (var column in columns)
            table.AddColumn(column, UiRole.Accent);

        if (items.Count == 0)
        {
            table.AddEmptyRow();
            return table;
        }

        for (int i = 0; i < items.Count; i++)
        {
            var row = rowFactory(items[i]);
            if (row.Length > 0)
                row[0] = $"{(i == selectedIndex ? UiStrings.RowSelected : UiStrings.RowUnselected)}{row[0]}";
            table.AddRow(row);
        }

        return table;
    }
}
