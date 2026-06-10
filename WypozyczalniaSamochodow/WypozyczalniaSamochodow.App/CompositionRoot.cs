using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;

using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App;

internal static class CompositionRoot
{
    public static AppShell Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<AppShell>();

        var provider = services.BuildServiceProvider();

        return provider.GetRequiredService<AppShell>();
    }
}
