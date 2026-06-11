using Microsoft.Extensions.DependencyInjection;

using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Application.Fleet;
using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Application.Reservations;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Insurance;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Navigation;

namespace WypozyczalniaSamochodow.Tests.TestSupport;

internal static class ScreenTestData
{
    public static FixedClock Clock { get; } = new(new DateOnly(2026, 5, 25));

    public static INavigator Navigator(IUiRenderer ui, IPrompts prompts,
        IClientRepository clients, IBackofficeRepository backoffice,
        IVehicleRepository vehicles, IReservationRepository reservations, IPasswordHasher? hasher = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IClock>(Clock);
        services.AddSingleton<IPasswordHasher>(hasher ?? new FakePasswordHasher());
        services.AddSingleton(clients);
        services.AddSingleton(backoffice);
        services.AddSingleton(vehicles);
        services.AddSingleton(reservations);
        services.AddSingleton(ui);
        services.AddSingleton(prompts);
        services.AddSingleton<AuthService>();
        services.AddSingleton<ReservationService>();
        services.AddSingleton<VehicleService>();
        services.AddSingleton<UserAccountService>();
        services.AddSingleton<INavigator, ScreenNavigator>();
        return services.BuildServiceProvider().GetRequiredService<INavigator>();
    }

    public static Password Password => Password.FromHash("hash");

    public static Client CreateClient(string fullName = "Jan Kowalski", string email = "jan@example.com", DrivingLicence? licence = null)
    => new(fullName, new Email(email), Password, licence ?? new DrivingLicence("ABC123", new DateOnly(2026, 12, 31)));

    public static Backoffice CreateBackoffice(string fullName = "Admin", string email = "admin@example.com")
    => new(fullName, new Email(email), Password);

    public static Vehicle CreateVehicle(string make = "Toyota", string model = "Corolla", string color = "Silver", int year = 2022,
    decimal pricePerDay = 100m, DateOnly? purchaseDate = null)
    {
        var vehicle = new Vehicle(make, model, new RegistrationNumber("kr123"), new Vin("vin123"), color, new Money(pricePerDay), year, purchaseDate ?? new DateOnly(2024, 1, 1));
        vehicle.AddInsurance(new Insurance("PZU", new PolicyNumber("POL-1"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m)));
        return vehicle;
    }

    public static Vehicle CreateVehicleWithoutInsurance(string make = "Toyota", string model = "Corolla")
    => new(make, model, new RegistrationNumber("kr123"), new Vin("vin123"), "Silver", new Money(100m), 2022, new DateOnly(2024, 1, 1));

    public static Reservation CreateReservation(Client client, Vehicle vehicle, DateRange? period = null, Payment? payment = null)
    => new(client, vehicle,
        period ?? DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)),
        payment ?? new CashPayment(new Money(300m), Clock.Now), Clock);

    public static Client CreateClientWithoutLicence(string fullName = "Jan Kowalski", string email = "jan@example.com")
    => new(fullName, new Email(email), Password);

    public static UserAccountService Users(IClientRepository clients, IBackofficeRepository backoffice,
    IPasswordHasher? hasher = null, IReservationRepository? reservations = null)
    => new(clients, backoffice, reservations ?? new InMemoryReservationRepository(), hasher ?? new FakePasswordHasher());

    public static INavigator Navigator(IUiRenderer ui, IPrompts prompts,
    IVehicleRepository vehicles, IReservationRepository reservations, IPasswordHasher? hasher = null)
    => Navigator(ui, prompts, new InMemoryClientRepository(), new InMemoryBackofficeRepository(),
        vehicles, reservations, hasher);
    public static ReservationService Reservations(IReservationRepository reservations, IVehicleRepository vehicles)
    => new(reservations, vehicles, Clock);

    public static ReservationService Reservations(IVehicleRepository vehicles)
        => new(new InMemoryReservationRepository(), vehicles, Clock);

    public static ReservationService Reservations()
        => new(new InMemoryReservationRepository(), new InMemoryVehicleRepository(), Clock);
}
