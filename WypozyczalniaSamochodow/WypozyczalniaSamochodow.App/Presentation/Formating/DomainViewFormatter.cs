using WypozyczalniaSamochodow.App.Domain.Fleet;
using WypozyczalniaSamochodow.App.Domain.Fleet.Events;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;

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

    public UiTable CreateDetailsTable() => throw new NotImplementedException();

    public string FormatDate(DateOnly? date) => throw new NotImplementedException();

    public string EventLabel(VehicleEvent ev) => throw new NotImplementedException();

    public string EventLabelColored(VehicleEvent ev, bool active)
    {
        throw new NotImplementedException();
    }

    public string VehicleStatus(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public string ReservationStatus(Reservation reservation)
    {
        throw new NotImplementedException();
    }

    public string PaymentLabel(Payment payment) => payment.MethodName;

    public void AddVehicleRows(UiTable table, Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public void AddReservationRows(UiTable table, Reservation reservation)
    {
        throw new NotImplementedException();
    }
}
