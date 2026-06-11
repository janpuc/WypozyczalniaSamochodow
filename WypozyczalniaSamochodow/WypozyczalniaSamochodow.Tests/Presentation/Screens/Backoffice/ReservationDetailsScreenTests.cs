using AwesomeAssertions;

using NSubstitute;

using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Infrastructure.Persistence;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Screens.Backoffice;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation.Screens.Backoffice;


public sealed class ReservationDetailsScreenTests
{
    [Fact]
    public void PendingReservationCanActivateAndComplete()
    {
        var client = ScreenTestData.CreateClient();
        var vehicle = ScreenTestData.CreateVehicle();
        var reservation = ScreenTestData.CreateReservation(client, vehicle);
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.A, ConsoleKey.Z, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts().EnqueueInt(1200, 1500).EnqueueText("done");
        var vehicles = new InMemoryVehicleRepository();

        new ReservationDetailsScreen(reservation, ui, prompts, ScreenTestData.Reservations(vehicles)).Run();

        reservation.Status.Should().BeOfType<CompletedReservation>();
        ui.Successes.Should().Contain(UiStrings.ReservationActivated);
        ui.Successes.Should().Contain(UiStrings.ReservationCompleted);
    }

    [Fact]
    public void PendingReservationRejectsInvalidActivationMileage()
    {
        var reservation = ScreenTestData.CreateReservation(ScreenTestData.CreateClient(), ScreenTestData.CreateVehicle());
        var ui = CreateUiRenderer(ConsoleKey.A, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts().EnqueueInt(-1);
        var vehicles = new InMemoryVehicleRepository();

        new ReservationDetailsScreen(reservation, ui, prompts, ScreenTestData.Reservations(vehicles)).Run();

        ui.Received().Error("Przebieg nie może być ujemny.");
    }

    [Fact]
    public void ActiveReservationRejectsInvalidCompletionMileage()
    {
        var client = ScreenTestData.CreateClient();
        var vehicle = ScreenTestData.CreateVehicle();
        var reservation = ScreenTestData.CreateReservation(client, vehicle);
        reservation.Activate(1200, ScreenTestData.Clock);

        var ui = CreateUiRenderer(ConsoleKey.Z, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts().EnqueueInt(1000).EnqueueText("done");
        var vehicles = new InMemoryVehicleRepository();

        new ReservationDetailsScreen(reservation, ui, prompts, ScreenTestData.Reservations(vehicles)).Run();

        ui.Received().Error("Przebieg końcowy nie może być mniejszy niż początkowy.");
    }

    [Fact]
    public void PendingReservationCanBeCancelled()
    {
        var client = ScreenTestData.CreateClient();
        var vehicle = ScreenTestData.CreateVehicle();
        var reservation = ScreenTestData.CreateReservation(client, vehicle);
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.D, ConsoleKey.Escape).EnqueueConfirmations(true);
        var prompts = new ScriptedPrompts();
        var vehicles = new InMemoryVehicleRepository();

        new ReservationDetailsScreen(reservation, ui, prompts, ScreenTestData.Reservations(vehicles)).Run();

        reservation.Status.Should().BeOfType<CancelledReservation>();
        ui.Successes.Should().Contain(UiStrings.Cancelled);
    }

    [Fact]
    public void PendingReservationCanSwapVehicle()
    {
        var client = ScreenTestData.CreateClient();
        var current = ScreenTestData.CreateVehicle();
        var other = ScreenTestData.CreateVehicle("Skoda", "Octavia");
        var reservation = ScreenTestData.CreateReservation(client, current);
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(current);
        vehicles.Add(other);
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.W, ConsoleKey.Enter, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();

        new ReservationDetailsScreen(reservation, ui, prompts, ScreenTestData.Reservations(vehicles)).Run();

        reservation.Vehicle.Should().BeSameAs(other);
        ui.Successes.Should().Contain(UiStrings.VehicleSwapped);
    }

    [Fact]
    public void PendingReservationCanSurfaceSwapFailure()
    {
        var client = ScreenTestData.CreateClient();
        var current = ScreenTestData.CreateVehicle();
        var other = ScreenTestData.CreateVehicle("Skoda", "Octavia");
        var reservation = ScreenTestData.CreateReservation(client, current);
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(current);
        vehicles.Add(other);

        var ui = Substitute.For<IUiRenderer>();
        ui.CreateDetailsTable().Returns(new UiTable());
        var keys = new Queue<ConsoleKey>(new[] { ConsoleKey.W, ConsoleKey.Enter, ConsoleKey.Escape });
        ui.ReadKey().Returns(_ =>
        {
            var key = keys.Count > 0 ? keys.Dequeue() : ConsoleKey.Escape;
            if (key == ConsoleKey.Enter)
            {
                reservation.Activate(0, ScreenTestData.Clock);
                reservation.Complete(1, null);
            }

            var ch = key switch
            {
                ConsoleKey.Enter => '\r',
                ConsoleKey.Escape => '\u001b',
                _ => '\0',
            };
            return new ConsoleKeyInfo(ch, key, false, false, false);
        });

        var prompts = new ScriptedPrompts();

        new ReservationDetailsScreen(reservation, ui, prompts, ScreenTestData.Reservations(vehicles)).Run();

        ui.Received().Error("Pojazd można wymienić tylko dla rezerwacji oczekującej.");
    }

    [Fact]
    public void PendingReservationCanSurfaceCancelFailure()
    {
        var client = ScreenTestData.CreateClient();
        var vehicle = ScreenTestData.CreateVehicle();
        var reservation = ScreenTestData.CreateReservation(client, vehicle);
        var ui = Substitute.For<IUiRenderer>();
        ui.CreateDetailsTable().Returns(new UiTable());
        ui.ReadKey().Returns(new ConsoleKeyInfo('\r', ConsoleKey.D, false, false, false), new ConsoleKeyInfo('\u001b', ConsoleKey.Escape, false, false, false));
        ui.ConfirmCancel(Arg.Any<string>(), Arg.Any<string>()).Returns(_ =>
        {
            reservation.Activate(0, ScreenTestData.Clock);
            reservation.Complete(1, null);
            return true;
        });
        var prompts = new ScriptedPrompts();
        var vehicles = new InMemoryVehicleRepository();

        new ReservationDetailsScreen(reservation, ui, prompts, ScreenTestData.Reservations(vehicles)).Run();

        ui.Received().Error("Nie można anulować rezerwacji w stanie Zakończona.");
    }

    [Fact]
    public void PendingReservationCanReportMissingSwapCandidates()
    {
        var client = ScreenTestData.CreateClient();
        var current = ScreenTestData.CreateVehicle();
        var reservation = ScreenTestData.CreateReservation(client, current);
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(current);
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.W, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();

        new ReservationDetailsScreen(reservation, ui, prompts, ScreenTestData.Reservations(vehicles)).Run();

        ui.Errors.Should().Contain(UiStrings.NoVehiclesAvailable);
    }

    [Fact]
    public void PendingReservationSwapScreenIgnoresUnknownKeys()
    {
        var client = ScreenTestData.CreateClient();
        var current = ScreenTestData.CreateVehicle();
        var other = ScreenTestData.CreateVehicle("Skoda", "Octavia");
        var reservation = ScreenTestData.CreateReservation(client, current);
        var vehicles = new InMemoryVehicleRepository();
        vehicles.Add(current);
        vehicles.Add(other);
        var ui = new ScriptedUiRenderer().EnqueueKeys(ConsoleKey.W, ConsoleKey.F1, ConsoleKey.Escape, ConsoleKey.Escape);
        var prompts = new ScriptedPrompts();

        new ReservationDetailsScreen(reservation, ui, prompts, ScreenTestData.Reservations(vehicles)).Run();

        reservation.Vehicle.Should().BeSameAs(current);
    }

    private static IUiRenderer CreateUiRenderer(params ConsoleKey[] keys)
    {
        var ui = Substitute.For<IUiRenderer>();
        ui.CreateDetailsTable().Returns(new UiTable());
        var queue = new Queue<ConsoleKey>(keys);
        ui.ReadKey().Returns(_ =>
        {
            var key = queue.Count > 0 ? queue.Dequeue() : ConsoleKey.Escape;
            var c = key switch
            {
                ConsoleKey.Enter => '\r',
                ConsoleKey.Escape => '\u001b',
                _ => '\0',
            };
            return new ConsoleKeyInfo(c, key, false, false, false);
        });
        return ui;
    }
}

