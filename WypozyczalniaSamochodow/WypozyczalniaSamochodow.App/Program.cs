
using WypozyczalniaSamochodow.App;

internal static class Program
{
    internal static void Main() => Run(CompositionRoot.Build);

    internal static void Run(Func<AppShell> buildShell) => buildShell().Run();
}