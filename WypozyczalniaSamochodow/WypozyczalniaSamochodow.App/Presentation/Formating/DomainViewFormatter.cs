using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Formating;

internal sealed class DomainViewFormatter : IDomainViewFormatter
{
    private readonly ITextStyler _styler;
    private readonly IClock _clock;
    private readonly EventMetadataVisitor _metadataVisitor = new();

    public DomainViewFormatter(ITextStyler styler, IClock clock)
    {
        _styler = styler;
        _clock = clock;
    }

    public UiTable CreateDetailsTable() => new UiTable().AddColumns(UiStrings.FieldKey, UiStrings.FieldValue);

    public string FormatDate(DateOnly? date) => date.HasValue ? date.Value.ToString(UiFormats.Date) : UiStrings.DateEmpty;

    public string EventLabel(VehicleEvent ev) => ev.Describe();

    public string EventLabelColored(VehicleEvent ev, bool active)
    {
        if (!active) return ev.Describe();
        var meta = ev.Accept(_metadataVisitor);
        return _styler.Colorize(meta.DisplayName, meta.Role);
    }

    public string VehicleStatus(Vehicle vehicle)
    {
        var active = vehicle.Schedule.ActiveNonReservationOn(_clock.Today);
        if (active is null) return _styler.Colorize(UiStrings.StatusAvailable, UiRole.Success);
        if (active is BrokenDownEvent bd && bd.LinkedRepair is not null)
            return EventLabelColored(bd.LinkedRepair, true);
        return EventLabelColored(active, true);
    }

    public string ReservationStatus(Reservation reservation)
    {
        var role = ReservationStatusRoles.For(reservation.Status);
        return role.HasValue ? _styler.Colorize(reservation.Status.Label, role.Value) : reservation.Status.Label;
    }

    public string PaymentLabel(Payment payment) => payment.MethodName;

    public void AddVehicleRows(UiTable table, Vehicle vehicle)
    {
        table.AddRow(UiStrings.Make, vehicle.Make);
        table.AddRow(UiStrings.Model, vehicle.Model);
        table.AddRow(UiStrings.Registration, vehicle.Registration.ToString());
        table.AddRow(UiStrings.Vin, vehicle.Vin.ToString());
        table.AddRow(UiStrings.Color, vehicle.Color);
        table.AddRow(UiStrings.PricePerDay, UiFormats.Money(vehicle.PricePerDay));
        table.AddRow(UiStrings.Year, vehicle.Year.ToString());
        table.AddRow(UiStrings.PurchaseDate, vehicle.PurchaseDate.ToString(UiFormats.Date));
        table.AddRow(UiStrings.InsuranceActive, vehicle.HasActiveInsuranceOn(_clock.Today)
            ? _styler.Colorize(UiStrings.Yes, UiRole.Success)
            : _styler.Colorize(UiStrings.No, UiRole.Error));
        table.AddRow(UiStrings.Status, VehicleStatus(vehicle));
    }

    public void AddReservationRows(UiTable table, Reservation reservation)
    {
        table.AddRow(UiStrings.Client, reservation.Client.FullName);
        table.AddRow(UiStrings.Vehicle, $"{reservation.Vehicle.Make} {reservation.Vehicle.Model} ({reservation.Vehicle.Registration})");
        table.AddRow(UiStrings.From, reservation.Event.FromDate.ToString(UiFormats.Date));
        table.AddRow(UiStrings.To, FormatDate(reservation.Event.ToDate));
        table.AddRow(UiStrings.MileageBefore, reservation.Status.MileageBefore.HasValue
            ? string.Format(UiStrings.MileageKm, reservation.Status.MileageBefore) : UiStrings.Empty);
        table.AddRow(UiStrings.MileageAfter, reservation.Status.MileageAfter.HasValue
            ? string.Format(UiStrings.MileageKm, reservation.Status.MileageAfter) : UiStrings.Empty);
        if (reservation.Status.CompletionNote is not null)
            table.AddRow(UiStrings.CompletionNote, reservation.Status.CompletionNote);
        table.AddRow(UiStrings.Status, reservation.Status.Label);
    }
}

