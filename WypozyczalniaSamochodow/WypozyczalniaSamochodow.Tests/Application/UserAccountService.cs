using AwesomeAssertions;
using WypozyczalniaSamochodow.App.Application.Users;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Application;

public sealed class UserAccountServiceTests
{
    private static UserAccountService Service(
        InMemoryClientRepository clients, InMemoryReservationRepository reservations)
        => new(clients, new InMemoryBackofficeRepository(), reservations, new FakePasswordHasher());

    [Fact]
    public void RemoveClientRejectsClientWithActiveReservation()
    {
        var clients = new InMemoryClientRepository();
        var reservations = new InMemoryReservationRepository();
        var client = ScreenTestData.CreateClient();
        clients.Add(client);
        var reservation = ScreenTestData.CreateReservation(client, ScreenTestData.CreateVehicle());
        reservation.Activate(1000, ScreenTestData.Clock);
        reservations.Add(reservation);

        var act = () => Service(clients, reservations).RemoveClient(client);

        act.Should().Throw<DomainException>().WithMessage("Nie można usunąć klienta z aktywną rezerwacją.");
        clients.All.Should().ContainSingle();
    }

    [Fact]
    public void RemoveClientSucceedsWhenNoActiveReservation()
    {
        var clients = new InMemoryClientRepository();
        var client = ScreenTestData.CreateClient();
        clients.Add(client);

        Service(clients, new InMemoryReservationRepository()).RemoveClient(client);

        clients.All.Should().BeEmpty();
    }

    [Fact]
    public void UpdateProfileIsAtomicWhenEmailIsInvalid()
    {
        var clients = new InMemoryClientRepository();
        var client = ScreenTestData.CreateClient(fullName: "Jan Kowalski", email: "jan@example.com");
        clients.Add(client);

        var act = () => Service(clients, new InMemoryReservationRepository())
            .UpdateProfile(client, "Nowe Imie", "zly-email");

        act.Should().Throw<DomainException>();
        client.FullName.Should().Be("Jan Kowalski");
        client.Email.Value.Should().Be("jan@example.com");
    }

    [Fact]
    public void UpdateProfileRejectsEmailTakenByAnotherUser()
    {
        var clients = new InMemoryClientRepository();
        var client = ScreenTestData.CreateClient(fullName: "Jan Kowalski", email: "jan@example.com");
        var other = ScreenTestData.CreateClient(fullName: "Anna Nowak", email: "anna@example.com");
        clients.Add(client);
        clients.Add(other);

        var act = () => Service(clients, new InMemoryReservationRepository())
            .UpdateProfile(client, "Jan Kowalski", "anna@example.com");

        act.Should().Throw<DomainException>().WithMessage("Użytkownik z tym emailem już istnieje.");
        client.Email.Value.Should().Be("jan@example.com");
    }

    [Fact]
    public void UpdateProfileRejectsBlankFullName()
    {
        var clients = new InMemoryClientRepository();
        var client = ScreenTestData.CreateClient(fullName: "Jan Kowalski", email: "jan@example.com");
        clients.Add(client);

        var act = () => Service(clients, new InMemoryReservationRepository())
            .UpdateProfile(client, "   ", "jan@example.com");

        act.Should().Throw<DomainException>().WithMessage("Imię i nazwisko nie może być puste.");
        client.FullName.Should().Be("Jan Kowalski");
    }

    [Fact]
    public void CreateClientRejectsPasswordBelowPolicy()
    {
        var clients = new InMemoryClientRepository();

        var act = () => Service(clients, new InMemoryReservationRepository())
            .CreateClient("Jan Kowalski", new Email("nowy@example.com"), "short", null);

        act.Should().Throw<DomainException>().WithMessage("Hasło musi mieć co najmniej 8 znaków.");
        clients.All.Should().BeEmpty();
    }

    [Fact]
    public void UpdateProfileAllowsKeepingOwnEmail()
    {
        var clients = new InMemoryClientRepository();
        var client = ScreenTestData.CreateClient(fullName: "Jan Kowalski", email: "jan@example.com");
        clients.Add(client);

        Service(clients, new InMemoryReservationRepository())
            .UpdateProfile(client, "Jan Nowak", "jan@example.com");

        client.FullName.Should().Be("Jan Nowak");
        client.Email.Value.Should().Be("jan@example.com");
    }
}
