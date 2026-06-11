using AwesomeAssertions;
using WypozyczalniaSamochodow.App.Domain.Shared;
using Xunit;

namespace WypozyczalniaSamochodow.Tests.Domain;

public sealed class SharedTests
{
    [Fact]
    public void MoneyRejectsNegativeValues()
    {
        var act = () => new Money(-1m);

        act.Should().Throw<DomainException>()
            .WithMessage("Kwota nie może być ujemna.");
    }

    [Fact]
    public void MoneySupportsArithmetic()
    {
        var total = new Money(10m) + new Money(5.5m);
        var perDay = new Money(7m) * 3;

        total.Value.Should().Be(15.5m);
        perDay.Value.Should().Be(21m);
        Money.Zero.Value.Should().Be(0m);
        perDay.ToString().Should().Be("21");
    }

    [Fact]
    public void DateRangeRejectsReversedRange()
    {
        var act = () => new DateRange(new DateOnly(2026, 5, 10), new DateOnly(2026, 5, 9));

        act.Should().Throw<DomainException>()
            .WithMessage("Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
    }

    [Fact]
    public void DateRangeNormalizesAndQueries()
    {
        var range = DateRange.Closed(new DateOnly(2026, 5, 10), new DateOnly(2026, 5, 12));

        range.From.Should().Be(new DateOnly(2026, 5, 10));
        range.To.Should().Be(new DateOnly(2026, 5, 12));
        range.Days.Should().Be(3);
        range.Contains(new DateOnly(2026, 5, 11)).Should().BeTrue();
        range.Contains(new DateOnly(2026, 5, 13)).Should().BeFalse();
        range.Overlaps(DateRange.Closed(new DateOnly(2026, 5, 12), new DateOnly(2026, 5, 14))).Should().BeTrue();
        range.Overlaps(DateRange.Closed(new DateOnly(2026, 5, 13), new DateOnly(2026, 5, 15))).Should().BeFalse();
    }

    [Fact]
    public void DomainExceptionWrapsInnerException()
    {
        var inner = new InvalidOperationException("przyczyna");

        var ex = new DomainException("komunikat", inner);

        ex.Message.Should().Be("komunikat");
        ex.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void OpenEndedRangeHasMaxEffectiveToAndUndefinedDays()
    {
        var range = DateRange.OpenEnded(new DateOnly(2026, 5, 10));

        range.EffectiveTo.Should().Be(DateOnly.MaxValue);
        var days = () => range.Days;
        days.Should().Throw<DomainException>();
    }
}
