using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;

using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Application.Reservations;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Infrastructure.Security;
using WypozyczalniaSamochodow.App.Infrastructure.Seed;
using WypozyczalniaSamochodow.App.Infrastructure.Time;
using WypozyczalniaSamochodow.App.Presentation;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Formating;
using WypozyczalniaSamochodow.App.Presentation.Navigation;

namespace WypozyczalniaSamochodow.App;

internal static class CompositionRoot
{
    public static AppShell Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();
        services.AddSingleton<IClientRepository, InMemoryClientRepository>();
        services.AddSingleton<IBackofficeRepository, InMemoryBackofficeRepository>();
        services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();
        services.AddSingleton<IReservationRepository, InMemoryReservationRepository>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<ReservationService>();
        services.AddSingleton<VehicleService>();
        services.AddSingleton<UserAccountService>();
        services.AddSingleton<ITextStyler, TextStyler>();
        services.AddSingleton<IDomainViewFormatter, DomainViewFormatter>();
        services.AddSingleton<IUiRenderer, UiRenderer>();
        services.AddSingleton<IPrompts, Prompts>();
        services.AddSingleton<INavigator, ScreenNavigator>();
        services.AddSingleton<AppShell>();
        services.AddSingleton<DemoSeedScenario>();


        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<DemoSeedScenario>().Run();

        return provider.GetRequiredService<AppShell>();
    }
}
