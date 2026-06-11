using AwesomeAssertions;

using WypozyczalniaSamochodow.App.Domain.Reservations;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.Formating;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation;

public sealed class PresentationHelpersTests
{
    [Fact]
    public void TextStylerEscapesAndWrapsText()
    {
        var styler = new TextStyler();

        styler.Colorize("Toyota", UiRole.Accent).Should().Contain("Toyota");
        styler.Highlight("Toyota").Should().Contain("Toyota").And.Contain("on");
    }

    private sealed class UnknownReservationStatus : ReservationStatus
    {
        public override string Label => "Nieznany";
    }

    [Fact]
    public void ReservationStatusRolesMapsKnownStatusesAndFallsBackToNull()
    {
        ReservationStatusRoles.For(new UnknownReservationStatus()).Should().BeNull();
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("   ", true)]
    [InlineData("wartość", false)]
    public void NullIfBlankCollapsesBlankToNull(string? input, bool expectNull)
    {
        var result = input.NullIfBlank();

        if (expectNull) result.Should().BeNull();
        else result.Should().Be(input);
    }

    [Fact]
    public void SelectableListRendersEmptyRowForNoItems()
    {
        var table = SelectableList.Build(System.Array.Empty<int>(), 0, new[] { "Kolumna" }, _ => new[] { "x" });

        table.Rows.Should().ContainSingle();
        table.Rows[0].Should().BeEmpty();
    }

    [Fact]
    public void SelectableListMarksSelectionAndToleratesEmptyRows()
    {
        var table = SelectableList.Build(
            new[] { 1, 2 }, 0, new[] { "Kolumna" },
            i => i == 1 ? System.Array.Empty<string>() : new[] { "wiersz" });

        table.Rows.Should().HaveCount(2);
        table.Rows[0].Should().BeEmpty();
        table.Rows[1][0].Should().Be(UiStrings.RowUnselected + "wiersz");
    }

    [Fact]
    public void TabNavigationCyclesForwardBackwardAndClampsSelection()
    {
        var forward = new ConsoleKeyInfo('\t', ConsoleKey.Tab, shift: false, alt: false, control: false);
        var backward = new ConsoleKeyInfo('\t', ConsoleKey.Tab, shift: true, alt: false, control: false);

        TabNavigation.Cycle(0, 3, forward).Should().Be(1);
        TabNavigation.Cycle(0, 3, backward).Should().Be(2);

        TabNavigation.ClampSelection(5, 3).Should().Be(2);
        TabNavigation.ClampSelection(1, 3).Should().Be(1);
        TabNavigation.ClampSelection(0, 0).Should().Be(0);
    }
}

