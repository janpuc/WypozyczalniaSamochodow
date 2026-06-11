using AwesomeAssertions;
using NSubstitute;
using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation.Screens;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation;

public sealed class LoginRegisterScreenTests
{
    [Fact]
    public void LoginScreenShowsFailureAndStops()
    {
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts().EnqueueText("missing@example.com").EnqueueSecret("bad");
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Verify("bad", Arg.Any<string>()).Returns(false);
        var auth = new AuthService(clients, backoffice, hasher);

        new LoginScreen(auth, ui, prompts, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher)).Run();

        ui.Lines.Should().Contain(UiStrings.LoginFailed);
        ui.WaitForKeyCalls.Should().Be(1);
    }

    [Fact]
    public void LoginScreenOpensBackofficeDashboardOnSuccess()
    {
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Escape);
        var prompts = new ScriptedPrompts().EnqueueText("admin@example.com").EnqueueSecret("secret");
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Verify("secret", "hash").Returns(true);
        var admin = ScreenTestData.CreateBackoffice();
        backoffice.Add(admin);
        var auth = new AuthService(clients, backoffice, hasher);

        new LoginScreen(auth, ui, prompts, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher)).Run();

        ui.Lines.Should().Contain(UiStrings.LoginSuccess);
        ui.Lines.Should().Contain(UiStrings.LoggedOut);
    }

    [Fact]
    public void LoginScreenOpensClientDashboardOnSuccess()
    {
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.Escape);
        var prompts = new ScriptedPrompts().EnqueueText("jan@example.com").EnqueueSecret("secret");
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var vehicles = new InMemoryVehicleRepository();
        var reservations = new InMemoryReservationRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Verify("secret", "hash").Returns(true);
        var client = ScreenTestData.CreateClient();
        clients.Add(client);
        var auth = new AuthService(clients, backoffice, hasher);

        new LoginScreen(auth, ui, prompts, ScreenTestData.Navigator(ui, prompts, clients, backoffice, vehicles, reservations, hasher)).Run();

        ui.Lines.Should().Contain(UiStrings.LoginSuccess);
        ui.Lines.Should().Contain(UiStrings.LoggedOut);
    }

    [Fact]
    public void RegisterScreenCanCreateClientWithLicenceOrWithout()
    {
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("Jan Kowalski", "jan@example.com")
            .EnqueueSecret("secret12", "secret12")
            .EnqueueConfirm(true)
            .EnqueueLicence(new DrivingLicence("ABC123", new DateOnly(2026, 12, 31)));
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash("secret12").Returns("hash");
        var auth = new AuthService(clients, backoffice, hasher);

        new RegisterScreen(auth, prompts, ui).Run();

        clients.All.Should().ContainSingle(c => c.FullName == "Jan Kowalski" && c.DrivingLicence != null);
        ui.Lines.Should().Contain(UiStrings.RegisterSuccess);
    }

    [Fact]
    public void RegisterScreenShowsDuplicateEmailError()
    {
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("Jan Kowalski", "jan@example.com")
            .EnqueueSecret("secret12", "secret12")
            .EnqueueConfirm(false);
        var clients = new InMemoryClientRepository();
        clients.Add(ScreenTestData.CreateClient());
        var backoffice = new InMemoryBackofficeRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash("secret12").Returns("hash");
        var auth = new AuthService(clients, backoffice, hasher);

        new RegisterScreen(auth, prompts, ui).Run();

        ui.Lines.Should().Contain(UiStrings.ValidationEmailTaken);
    }

    [Fact]
    public void RegisterScreenShowsInvalidEmailMessage()
    {
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("Jan Kowalski", "zly-email")
            .EnqueueSecret("secret12", "secret12")
            .EnqueueConfirm(false);
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var auth = new AuthService(clients, backoffice, Substitute.For<IPasswordHasher>());

        new RegisterScreen(auth, prompts, ui).Run();

        ui.Lines.Should().Contain(UiStrings.ValidationEmailInvalid);
        clients.All.Should().BeEmpty();
    }

    [Fact]
    public void RegisterScreenShowsWeakPasswordMessage()
    {
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("Jan Kowalski", "jan@example.com")
            .EnqueueSecret("short", "short")
            .EnqueueConfirm(false);
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var auth = new AuthService(clients, backoffice, Substitute.For<IPasswordHasher>());

        new RegisterScreen(auth, prompts, ui).Run();

        ui.Lines.Should().Contain(string.Format(UiStrings.ValidationPasswordTooShort, PasswordPolicy.MinimumLength));
        clients.All.Should().BeEmpty();
    }

    [Fact]
    public void RegisterScreenSurfacesDomainExceptionFromInvalidName()
    {
        var ui = new ScriptedUiRenderer();
        var prompts = new ScriptedPrompts()
            .EnqueueText("   ", "jan@example.com")
            .EnqueueSecret("secret12", "secret12")
            .EnqueueConfirm(false);
        var clients = new InMemoryClientRepository();
        var backoffice = new InMemoryBackofficeRepository();
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash("secret12").Returns("hash");
        var auth = new AuthService(clients, backoffice, hasher);

        new RegisterScreen(auth, prompts, ui).Run();

        ui.Lines.Should().Contain("Imię i nazwisko nie może być puste.");
        clients.All.Should().BeEmpty();
    }
}
