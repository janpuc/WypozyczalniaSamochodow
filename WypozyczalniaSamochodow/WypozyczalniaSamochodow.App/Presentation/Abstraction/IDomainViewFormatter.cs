using WypozyczalniaSamochodow.App.Domain.Fleet.Events;

namespace WypozyczalniaSamochodow.App.Presentation.Abstraction;

internal interface IDomainViewFormatter
{
    UiTable CreateDetailsTable();
    string FormatDate(DateOnly? date);
    string EventLabel(VehicleEvent ev);
}
