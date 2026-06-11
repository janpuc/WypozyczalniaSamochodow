using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

namespace WypozyczalniaSamochodow.App.Presentation.Formating;

internal static class ReservationStatusRoles
{
    private static readonly Dictionary<Type, UiRole> Roles = new()
    {
        [typeof(PendingReservation)] = UiRole.Warning,
        [typeof(ActiveReservation)] = UiRole.Success,
        [typeof(CompletedReservation)] = UiRole.Muted,
        [typeof(CancelledReservation)] = UiRole.Muted
    };

    public static UiRole? For(ReservationStatus status) =>
        Roles.TryGetValue(status.GetType(), out var role) ? role : null;
}
