using System.Globalization;

namespace WypozyczalniaSamochodow.App.Domain.Shared;

internal sealed record Money
{
    public decimal Value { get; }

    public Money(decimal value)
    {
        if (value < 0)
            throw new DomainException("Kwota nie może być ujemna.");
        Value = value;
    }

    public static Money Zero => new(0m);

    public static Money operator *(Money money, int multiplier) => new(money.Value * multiplier);
    public static Money operator +(Money a, Money b) => new(a.Value + b.Value);

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
