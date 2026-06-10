using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;

namespace WypozyczalniaSamochodow.App.Domain.Payments;

internal sealed class PayPalPayment : Payment
{
    public Email Account { get; }
    public PayPalPayment(Money amount, DateTime paidAt, Email account) : base(amount, paidAt)
    {
        Account = account;
    }
    public override string MethodName => "PayPal";
    public override string Describe() => $"PayPal ({Account})";
}
