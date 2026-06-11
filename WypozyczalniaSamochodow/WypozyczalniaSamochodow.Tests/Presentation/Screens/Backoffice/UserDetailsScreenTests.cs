using AwesomeAssertions;

using NSubstitute;
using WypozyczalniaSamochodow.App.Application.Auth;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;
using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation.Screens.Backoffice;

public sealed class UserDetailsScreenTests
{
    [Fact]
    public void ClientDetailsCanEditAndDeleteWhenNoActiveReservations()
    {
        var client = ScreenTestData.CreateClient();
        var clients = new InMemoryClientRepository();
        clients.Add(client);
        var reservations = new InMemoryReservationRepository();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.E, ConsoleKey.D, ConsoleKey.Escape).EnqueueConfirmations(true);
        var prompts = new ScriptedPrompts().EnqueueText("Nowe Imie", "nowy@example.com");

        new ClientDetailsScreen(client, ui, prompts, ScreenTestData.Users(clients, new InMemoryBackofficeRepository()), reservations).Run();

        clients.All.Should().BeEmpty();
        ui.Successes.Should().Contain(UiStrings.Updated);
        ui.Successes.Should().Contain(UiStrings.Removed);
    }

    [Fact]
    public void BackofficeUserCanEditResetPasswordAndDelete()
    {
        var user = ScreenTestData.CreateBackoffice();
        var backoffice = new InMemoryBackofficeRepository();
        backoffice.Add(user);
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns("hash");
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.E, ConsoleKey.P, ConsoleKey.D, ConsoleKey.Escape).EnqueueConfirmations(true);
        var prompts = new ScriptedPrompts().EnqueueText("Admin Nowy", "admin.nowy@example.com").EnqueueSecret("secret123");

        new BackofficeUserDetailsScreen(user, ui, prompts, ScreenTestData.Users(new InMemoryClientRepository(), backoffice, hasher)).Run();

        backoffice.All.Should().BeEmpty();
        ui.Successes.Should().Contain(UiStrings.Updated);
        ui.Successes.Should().Contain(UiStrings.PasswordReset);
        ui.Successes.Should().Contain(UiStrings.Removed);
    }

    [Fact]
    public void ClientDetailsShowErrorsAndBlockDeleteWhenReservationsAreActive()
    {
        var client = ScreenTestData.CreateClient();
        var vehicle = ScreenTestData.CreateVehicle();
        var clients = new InMemoryClientRepository();
        clients.Add(client);
        var reservations = new InMemoryReservationRepository();
        var reservation = ScreenTestData.CreateReservation(client, vehicle);
        reservation.Activate(1000, ScreenTestData.Clock);
        reservations.Add(reservation);
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.E, ConsoleKey.D, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts().EnqueueText("Nowe Imie", "zly-email");

        new ClientDetailsScreen(client, ui, prompts, ScreenTestData.Users(clients, new InMemoryBackofficeRepository()), reservations).Run();

        clients.All.Should().ContainSingle();
        ui.Errors.Should().Contain("Podaj prawidłowy adres email.");
        ui.Errors.Should().Contain(UiStrings.ClientHasActiveReservations);
    }

    [Fact]
    public void ClientDetailsAllowCancellingDelete()
    {
        var client = ScreenTestData.CreateClient();
        var clients = new InMemoryClientRepository();
        clients.Add(client);
        var reservations = new InMemoryReservationRepository();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.D, ConsoleKey.Escape).EnqueueConfirmations(false);
        var prompts = new ScriptedPrompts();

        new ClientDetailsScreen(client, ui, prompts, ScreenTestData.Users(clients, new InMemoryBackofficeRepository()), reservations).Run();

        clients.All.Should().ContainSingle();
    }

    [Fact]
    public void BackofficeUserDetailsShowErrorsAndAllowCancellingDelete()
    {
        var user = ScreenTestData.CreateBackoffice();
        var backoffice = new InMemoryBackofficeRepository();
        backoffice.Add(user);
        var hasher = Substitute.For<IPasswordHasher>();
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.E, ConsoleKey.P, ConsoleKey.D, ConsoleKey.Escape).EnqueueConfirmations(false);
        var prompts = Substitute.For<IPrompts>();
        prompts.PromptFullName(user.FullName).Returns("Admin Nowy");
        prompts.PromptEmail(user.Email.Value).Returns("zly-email");
        prompts.PromptPassword(PasswordPolicy.MinimumLength).Returns(_ => throw new DomainException("Hasło nieprawidłowe."));

        new BackofficeUserDetailsScreen(user, ui, prompts, ScreenTestData.Users(new InMemoryClientRepository(), backoffice, hasher)).Run();

        backoffice.All.Should().ContainSingle();
        ui.Errors.Should().Contain("Podaj prawidłowy adres email.");
        ui.Errors.Should().Contain("Hasło nieprawidłowe.");
    }
}
