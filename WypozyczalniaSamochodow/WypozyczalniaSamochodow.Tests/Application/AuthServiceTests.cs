using AwesomeAssertions;

using NSubstitute;

using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Users;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Application;

public sealed class AuthServiceTests
{
    private static Password PasswordFromHash(string hash) => Password.FromHash(hash);

    [Fact]
    public void LoginRejectsInvalidEmailWithoutQueryingRepositories()
    {
        var clients = Substitute.For<IClientRepository>();
        var backoffice = Substitute.For<IBackofficeRepository>();
        var hasher = Substitute.For<IPasswordHasher>();
        var sut = new AuthService(clients, backoffice, hasher);

        var result = sut.Login("invalid", "secret");

        result.Should().BeNull();
        clients.DidNotReceive().FindByEmail(Arg.Any<Email>());
        backoffice.DidNotReceive().FindByEmail(Arg.Any<Email>());
    }

    [Fact]
    public void LoginReturnsClientOrBackofficeWhenPasswordMatches()
    {
        var clients = Substitute.For<IClientRepository>();
        var backoffice = Substitute.For<IBackofficeRepository>();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Verify("secret", "hash").Returns(true);

        var client = new Client("Jan", new Email("jan@example.com"), PasswordFromHash("hash"));
        clients.FindByEmail(new Email("jan@example.com")).Returns(client);

        var sut = new AuthService(clients, backoffice, hasher);
        var result = sut.Login("JAN@example.com", "secret");

        result.Should().BeSameAs(client);
        backoffice.DidNotReceive().FindByEmail(Arg.Any<Email>());

        var admin = new Backoffice("Admin", new Email("admin@example.com"), PasswordFromHash("hash"));
        clients.FindByEmail(new Email("admin@example.com")).Returns((Client?)null);
        backoffice.FindByEmail(new Email("admin@example.com")).Returns(admin);

        sut.Login("admin@example.com", "secret").Should().BeSameAs(admin);
    }

    [Fact]
    public void LoginReturnsNullForMissingUserOrWrongPassword()
    {
        var clients = Substitute.For<IClientRepository>();
        var backoffice = Substitute.For<IBackofficeRepository>();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Verify("bad", "hash").Returns(false);

        clients.FindByEmail(Arg.Any<Email>()).Returns((Client?)null);
        backoffice.FindByEmail(Arg.Any<Email>()).Returns((Backoffice?)null);

        var sut = new AuthService(clients, backoffice, hasher);

        sut.Login("missing@example.com", "bad").Should().BeNull();

        var client = new Client("Jan", new Email("jan@example.com"), PasswordFromHash("hash"));
        clients.FindByEmail(new Email("jan@example.com")).Returns(client);
        sut.Login("jan@example.com", "bad").Should().BeNull();
    }

    [Fact]
    public void RegisterClientAddsNewClientAndRejectsDuplicates()
    {
        var clients = Substitute.For<IClientRepository>();
        var backoffice = Substitute.For<IBackofficeRepository>();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash("secret12").Returns("hash");
        clients.IsEmailTaken(Arg.Any<Email>()).Returns(false);
        backoffice.IsEmailTaken(Arg.Any<Email>()).Returns(false);

        var sut = new AuthService(clients, backoffice, hasher);
        var licence = new DrivingLicence("ABC", new DateOnly(2026, 12, 31));

        sut.RegisterClient("Jan Kowalski", "jan@example.com", "secret12", licence).Should().Be(RegistrationResult.Success);
        clients.Received(1).Add(Arg.Is<Client>(c => c.FullName == "Jan Kowalski" && c.Email.Equals(new Email("jan@example.com"))));
        hasher.Received(1).Hash("secret12");

        clients.IsEmailTaken(Arg.Any<Email>()).Returns(true);
        sut.RegisterClient("Jan Kowalski", "jan@example.com", "secret12", licence).Should().Be(RegistrationResult.EmailTaken);
    }

    [Fact]
    public void RegisterClientReturnsInvalidEmailForMalformedAddress()
    {
        var clients = Substitute.For<IClientRepository>();
        var backoffice = Substitute.For<IBackofficeRepository>();
        var hasher = Substitute.For<IPasswordHasher>();
        var sut = new AuthService(clients, backoffice, hasher);

        sut.RegisterClient("Jan", "invalid", "secret12", null).Should().Be(RegistrationResult.InvalidEmail);
        clients.DidNotReceive().Add(Arg.Any<Client>());
    }

    [Fact]
    public void RegisterClientRejectsPasswordBelowPolicy()
    {
        var clients = Substitute.For<IClientRepository>();
        var backoffice = Substitute.For<IBackofficeRepository>();
        var hasher = Substitute.For<IPasswordHasher>();
        var sut = new AuthService(clients, backoffice, hasher);

        sut.RegisterClient("Jan", "jan@example.com", "short", null).Should().Be(RegistrationResult.WeakPassword);
        clients.DidNotReceive().Add(Arg.Any<Client>());
        hasher.DidNotReceive().Hash(Arg.Any<string>());
    }
}
