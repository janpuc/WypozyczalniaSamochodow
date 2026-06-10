using AwesomeAssertions;
using WypozyczalniaSamochodow.App.Domain.Payments;
using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Domain.Users;
using Xunit;

namespace WypozyczalniaSamochodow.Tests.Domain;

public sealed class PaymentsTests
{
    private static readonly Money Amount = new(100m);
    private static readonly DateTime Date = new(2026, 5, 25);

    public static IEnumerable<object[]> PaymentSamples() =>
    [
        [new CashPayment(Amount, Date), "Gotówka", "Gotówka"],
        [new DebitCardPayment(Amount, Date, "1234"), "Karta **** 1234", "Karta debetowa"],
        [new BankTransferPayment(Amount, Date, "PL123"), "Przelew (PL123)", "Przelew bankowy"],
        [new BitcoinPayment(Amount, Date, "bc1q123"), "Bitcoin (bc1q123)", "Bitcoin"],
        [new PayPalPayment(Amount, Date, new Email("pay@example.com")), "PayPal (pay@example.com)", "PayPal"],
    ];

    [Theory]
    [MemberData(nameof(PaymentSamples))]
    public void PaymentExposesDescriptionAndMethodName(object payment, string expectedDescribe, string expectedMethod)
    {
        var p = (Payment)payment;
        p.Describe().Should().Be(expectedDescribe);
        p.MethodName.Should().Be(expectedMethod);
    }

    [Fact]
    public void PaymentTypesValidateInputs()
    {
        var amount = new Money(100m);
        var date = new DateTime(2026, 5, 25);

        Action tooShortCard = () => new DebitCardPayment(amount, date, "123");
        Action nonDigitCard = () => new DebitCardPayment(amount, date, "12ab");
        Action emptyIban = () => new BankTransferPayment(amount, date, "");
        Action blankWallet = () => new BitcoinPayment(amount, date, " ");

        tooShortCard.Should().Throw<DomainException>();
        nonDigitCard.Should().Throw<DomainException>();
        emptyIban.Should().Throw<DomainException>();
        blankWallet.Should().Throw<DomainException>();
    }
}

