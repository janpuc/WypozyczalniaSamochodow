using AwesomeAssertions;

using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Insurance;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Infrastructure.Security;
using WypozyczalniaSamochodow.App.Infrastructure.Time;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Infrastructure;

public sealed class RepositoryTests
{
    private static Password PasswordFromHash(string hash) => Password.FromHash(hash);

    private static Client CreateClient(string email = "jan@example.com")
        => new("Jan", new Email(email), PasswordFromHash("hash"), new DrivingLicence("ABC", new DateOnly(2026, 12, 31)));

    private static Vehicle CreateVehicle()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", new RegistrationNumber("kr123"), new Vin("vin123"), "Silver", new Money(100m), 2022, new DateOnly(2024, 1, 1));
        vehicle.AddInsurance(new Insurance("PZU", new PolicyNumber("POL-1"), "OC", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), new Money(100m)));
        return vehicle;
    }

    [Fact]
    public void InMemoryRepositoriesStoreAndQueryData()
    {
        var clientRepo = new InMemoryClientRepository();
        var backofficeRepo = new InMemoryBackofficeRepository();
        var vehicleRepo = new InMemoryVehicleRepository();
        var reservationRepo = new InMemoryReservationRepository();
        var client = CreateClient();
        var backoffice = new Backoffice("Admin", new Email("admin@example.com"), PasswordFromHash("hash"));
        var vehicle = CreateVehicle();
        var reservation = new Reservation(client, vehicle, DateRange.Closed(new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)), new CashPayment(new Money(300m), new DateTime(2026, 5, 25)), new FixedClock(new DateOnly(2026, 5, 25)));

        clientRepo.Add(client);
        backofficeRepo.Add(backoffice);
        vehicleRepo.Add(vehicle);
        reservationRepo.Add(reservation);

        clientRepo.FindByEmail(new Email("JAN@example.com")).Should().BeSameAs(client);
        backofficeRepo.FindByEmail(new Email("admin@example.com")).Should().BeSameAs(backoffice);
        reservationRepo.OfClient(client).Should().ContainSingle();
        reservationRepo.OfVehicle(vehicle).Should().ContainSingle();
        reservationRepo.HasActiveOf(client).Should().BeTrue();
        reservationRepo.HasActiveOf(vehicle).Should().BeTrue();

        clientRepo.Remove(client);
        backofficeRepo.Remove(backoffice);
        vehicleRepo.Remove(vehicle);
        reservationRepo.Remove(reservation);

        clientRepo.All.Should().BeEmpty();
        backofficeRepo.All.Should().BeEmpty();
        vehicleRepo.All.Should().BeEmpty();
        reservationRepo.All.Should().BeEmpty();
    }

    [Fact]
    public void Argon2HasherHashesAndVerifiesRoundTrip()
    {
        var hasher = new Argon2PasswordHasher();

        var hash = hasher.Hash("secret");

        hash.Should().NotBeNullOrWhiteSpace();
        hasher.Verify("secret", hash).Should().BeTrue();
        hasher.Verify("wrong", hash).Should().BeFalse();
        hasher.Verify("much-longer-wrong-password", hash).Should().BeFalse();
        hasher.Verify("secret", "not-an-argon2-hash").Should().BeFalse();
    }

    [Fact]
    public void Argon2HasherTreatsMalformedStoredHashAsFailedVerification()
    {
        var hasher = new Argon2PasswordHasher();

        hasher.Verify("secret", "$argon2id$v=19$m=65536,t=3,p=4$6DPPInLlekfc/uf0D19pTg$").Should().BeFalse();
        hasher.Verify("secret", null!).Should().BeFalse();
    }

    [Fact]
    public void SystemClockReturnsCurrentTime()
    {
        var clock = new SystemClock();

        (DateTime.Now - clock.Now).Duration().Should().BeLessThan(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SystemClockUsesInjectedTimeProviderWhenSupplied()
    {
        var clock = new SystemClock(TimeProvider.System);

        (DateTime.Now - clock.Now).Duration().Should().BeLessThan(TimeSpan.FromSeconds(1));
    }
}
