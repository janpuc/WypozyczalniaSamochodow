using WypozyczalniaSamochodow.App.Application.Repositories;
using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Insurance;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Infrastructure.Seed;

internal sealed class DemoSeedScenario
{
    private readonly IClientRepository _clients;
    private readonly IBackofficeRepository _backoffice;
    private readonly IVehicleRepository _vehicles;
    private readonly IReservationRepository _reservations;
    private readonly IPasswordHasher _hasher;
    private readonly IClock _clock;

    public DemoSeedScenario(IClientRepository clients, IBackofficeRepository backoffice,
        IVehicleRepository vehicles, IReservationRepository reservations,
        IPasswordHasher hasher, IClock clock)
    {
        _clients = clients;
        _backoffice = backoffice;
        _vehicles = vehicles;
        _reservations = reservations;
        _hasher = hasher;
        _clock = clock;
    }

    public void Run()
    {
        var today = _clock.Today;
        var defaultPass = Password.FromPlain("Test123!", _hasher);
        var adminPass = Password.FromPlain("Admin123!", _hasher);

        var jan = new Client("Jan Kowalski", new Email("test@example.com"), defaultPass,
            new DrivingLicence("ABC123456", today.AddYears(3)));
        var anna = new Client("Anna Nowak", new Email("anna@example.com"), defaultPass,
            new DrivingLicence("EXP987654", today.AddMonths(-2)));
        var piotr = new Client("Piotr Wiśniewski", new Email("piotr@example.com"), defaultPass);
        _clients.Add(jan);
        _clients.Add(anna);
        _clients.Add(piotr);

        _backoffice.Add(new Backoffice("Admin Backoffice", new Email("admin@example.com"), adminPass));
        _backoffice.Add(new Backoffice("Marek Kowalski", new Email("marek@example.com"), adminPass));

        var toyota = MakeVehicle("Toyota", "Corolla", "KR12345", "JTDBU4EE3B9123456", "Srebrny", 120m, today.Year - 3, today.AddYears(-3).AddMonths(2));
        var bmw = MakeVehicle("BMW", "X5", "KR54321", "5UXKR0C55G0P12345", "Czarny", 350m, today.Year - 2, today.AddYears(-2).AddMonths(4));
        var audi = MakeVehicle("Audi", "A4", "KR99999", "WAUFFAFM3DA012345", "Biały", 200m, today.Year - 4, today.AddYears(-4).AddMonths(7));
        var vw = MakeVehicle("Volkswagen", "Golf", "KR11111", "WVWZZZ1KZBW123456", "Niebieski", 100m, today.Year - 5, today.AddYears(-5).AddMonths(1));
        var mercedes = MakeVehicle("Mercedes-Benz", "C-Class", "KR77777", "WDDGF4HB5CA712345", "Szary", 280m, today.Year - 2, today.AddYears(-2));
        var skoda = MakeVehicle("Skoda", "Octavia", "KR22222", "TMBJF45J6C3123456", "Czerwony", 140m, today.Year - 4, today.AddYears(-4).AddMonths(6));
        var ford = MakeVehicle("Ford", "Focus", "KR33333", "1FADP3F23DL123456", "Zielony", 130m, today.Year - 3, today.AddYears(-3).AddMonths(5));

        foreach (var v in new[] { toyota, bmw, audi, vw, mercedes, skoda, ford })
            _vehicles.Add(v);

        toyota.AddInsurance(new Insurance("PZU", new PolicyNumber("PZU-OC-00123"), "OC", today.AddMonths(-1), today.AddMonths(11), new Money(1800m)));
        toyota.AddInsurance(new Insurance("PZU", new PolicyNumber("PZU-AC-00124"), "AC", today.AddMonths(-1), today.AddMonths(11), new Money(950m)));
        bmw.AddInsurance(new Insurance("Warta", new PolicyNumber("WRT-OC-00456"), "OC", today.AddYears(-1).AddDays(-30), today.AddDays(-30), new Money(3200m)));
        bmw.AddInsurance(new Insurance("Warta", new PolicyNumber("WRT-AC-00457"), "AC", today.AddMonths(-4), today.AddMonths(8), new Money(2800m)));
        audi.AddInsurance(new Insurance("Ergo Hestia", new PolicyNumber("EH-OC-00789"), "OC", today.AddMonths(-5), today.AddMonths(7), new Money(2400m)));
        audi.AddInsurance(new Insurance("Ergo Hestia", new PolicyNumber("EH-AC-00790"), "AC", today.AddMonths(-5), today.AddMonths(7), new Money(1900m)));
        vw.AddInsurance(new Insurance("Generali", new PolicyNumber("GEN-OC-00234"), "OC", today.AddMonths(-2), today.AddMonths(10), new Money(1600m)));
        mercedes.AddInsurance(new Insurance("Allianz", new PolicyNumber("ALZ-OC-00567"), "OC", today.AddMonths(-3), today.AddMonths(9), new Money(2900m)));
        mercedes.AddInsurance(new Insurance("Allianz", new PolicyNumber("ALZ-AC-00568"), "AC", today.AddMonths(-3), today.AddMonths(9), new Money(2500m)));
        ford.AddInsurance(new Insurance("Link4", new PolicyNumber("L4-OC-00891"), "OC", today.AddMonths(-6), today.AddMonths(6), new Money(1750m)));

        SeedReservation(jan, toyota, today.AddDays(-20), today.AddDays(-15),
            new CashPayment(toyota.PricePerDay * 5, At(today.AddDays(-25))),
            new CompletedReservation(45000, 45320, "Auto zwrócone bez uwag"));
        SeedReservation(jan, audi, today, today.AddDays(5),
            new DebitCardPayment(audi.PricePerDay * 5, At(today.AddDays(-2)), "1234"),
            new ActiveReservation(68000));
        SeedReservation(jan, bmw, today.AddDays(3), today.AddDays(8),
            new BankTransferPayment(bmw.PricePerDay * 5, At(today.AddDays(-1)), "PL61109010140000071219812874"),
            new PendingReservation());
        SeedReservation(jan, toyota, today.AddDays(15), today.AddDays(20),
            new BitcoinPayment(toyota.PricePerDay * 5, At(today.AddDays(-3)), "bc1qar0srrr7xfkvy5l643lydnw9re59gtzzwf5mdq"),
            new CancelledReservation());
        SeedReservation(anna, toyota, today.AddDays(30), today.AddDays(33),
            new PayPalPayment(toyota.PricePerDay * 3, At(today), anna.Email),
            new PendingReservation());

        var vwBroken = new BrokenDownEvent(DateRange.OpenEnded(today.AddDays(-3)), "Awaria silnika");
        vw.AddEvent(vwBroken);
        vw.AddEvent(vwBroken.RegisterRepair(DateRange.Closed(today, today.AddDays(3)), "(Awaria silnika) - Naprawa w warsztacie"));

        mercedes.AddEvent(new MaintenanceEvent(DateRange.Closed(today.AddDays(4), today.AddDays(8)), "Przegląd techniczny"));
        toyota.AddEvent(new InspectionEvent(DateRange.Closed(today.AddDays(10), today.AddDays(11)), "Coroczna inspekcja"));
        audi.AddEvent(new DetailingEvent(DateRange.Closed(today.AddDays(-7), today.AddDays(-5)), "Detailing wnętrza i karoserii"));
        skoda.AddEvent(new SuspendedEvent(today.AddDays(-5), "Wycofany z floty do odwołania"));
        ford.AddEvent(new BrokenDownEvent(DateRange.OpenEnded(today.AddDays(-1)), "Stuk z przodu — czeka na diagnostykę"));
    }

    private static DateTime At(DateOnly date) => date.ToDateTime(TimeOnly.MinValue);

    private static Vehicle MakeVehicle(string make, string model, string reg, string vin, string color,
        decimal price, int year, DateOnly purchase) =>
        new(make, model, new RegistrationNumber(reg), new Vin(vin), color, new Money(price), year, purchase);

    private void SeedReservation(Client client, Vehicle vehicle, DateOnly from, DateOnly to,
        Payment payment, ReservationStatus status)
    {
        var ev = new ReservationEvent(DateRange.Closed(from, to), "Rezerwacja");
        if (status is not CancelledReservation)
            vehicle.AddEvent(ev);
        _reservations.Add(Reservation.Rehydrate(client, vehicle, ev, payment, status));
    }
}

