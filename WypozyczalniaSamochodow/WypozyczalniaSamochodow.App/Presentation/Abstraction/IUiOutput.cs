using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal interface IUiOutput
{
    void Clear();
    void WriteLine();
    void Line(string text, UiRole role = UiRole.Default);
    void Hint(params HintItem[] items);
    void Hint(string prefix, params HintItem[] items);
    void Heading(string title);
    void Banner(string text);
    void Render(UiTable table);
    ConsoleKeyInfo ReadKey();
    string Menu(string title, IEnumerable<string> choices);
    void RunWithStatus(string message, Action action);
    void WaitForKey(string? message = null);
    void Error(string message);
    void Success(string message);
    bool ConfirmDelete(string entityType, string entityName);
    bool ConfirmCancel(string entityType, string entityName);
    void DrawTabs(string[] names, int activeIndex);
}
