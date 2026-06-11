using Spectre.Console;
using Spectre.Console.Testing;

namespace WypozyczalniaSamochodow.Tests.TestSupport;

internal sealed class TestConsoleScope : IDisposable
{
    private readonly IAnsiConsole _previous;

    public TestConsole Console { get; } = new();

    public TestConsoleScope()
    {
        _previous = AnsiConsole.Console;
        AnsiConsole.Console = Console;
        AnsiConsole.Console.Profile.Capabilities.Interactive = true;
    }

    public void Dispose()
    {
        AnsiConsole.Console = _previous;
        Console.Dispose();
    }
}
