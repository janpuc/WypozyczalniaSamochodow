using WypozyczalniaSamochodow.App.Domain.Shared;

namespace WypozyczalniaSamochodow.App.Domain.Payments;

internal sealed class DebitCardPayment : Payment
{
    public string CardLast4 { get; }
    public DebitCardPayment(Money amount, DateTime paidAt, string cardLast4) : base(amount, paidAt)
    {
        if (string.IsNullOrWhiteSpace(cardLast4) || cardLast4.Length != 4 || !cardLast4.All(char.IsAsciiDigit))
            throw new DomainException("Ostatnie 4 cyfry karty muszą być dokładnie czterema cyframi.");
        CardLast4 = cardLast4;
    }
    public override string MethodName => "Karta debetowa";
    public override string Describe() => $"Karta **** {CardLast4}";
}
