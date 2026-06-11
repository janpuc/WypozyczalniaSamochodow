using AwesomeAssertions;

using Spectre.Console;

using WypozyczalniaSamochodow.App.Domain.Shared;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.App.Presentation.UIConfig;

using Xunit;

using Keys = WypozyczalniaSamochodow.App.Presentation.UIConfig.Keys;


namespace WypozyczalniaSamochodow.Tests.Presentation;

public sealed class UIConfigTests
{
    [Fact]
    public void UiTableAndBindingsStoreValues()
    {
        var table = new UiTable()
            .AddColumn("A", UiRole.Accent)
            .AddColumns("B", "C")
            .AddRow("1", "2", "3")
            .AddEmptyRow();

        table.Columns.Should().HaveCount(3);
        table.Rows.Should().HaveCount(2);
        table.Rows[0].Should().Equal("1", "2", "3");
        table.Rows[1].Should().BeEmpty();

        Keys.Back.Label.Should().Be("Esc");
        Keys.Select.Matches(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)).Should().BeTrue();
        Keys.Add.Matches(new ConsoleKeyInfo('+', ConsoleKey.OemPlus, false, false, false)).Should().BeTrue();

        var item = (Keys.Delete, UiStrings.HintDelete);
        HintItem hint = item;
        hint.Key.Should().BeSameAs(Keys.Delete);
        hint.Description.Should().Be(UiStrings.HintDelete);
    }

    [Fact]
    public void ThemeAndMarkupHelpersMapRoles()
    {
        Theme.ColorOf(UiRole.Success).Should().Be(Color.Green);
        Theme.ColorOf(UiRole.Heading).Should().Be(Color.Green);
        Theme.ColorOf((UiRole)999).Should().Be(Color.Default);
        Theme.MarkupOf(UiRole.Default).Should().BeEmpty();
        Theme.SelectionStyle.Should().Contain("on");

        ConsoleMarkup.Wrap("x", string.Empty).Should().Be("x");
        ConsoleMarkup.Wrap("x", "green").Should().Be("[green]x[/]");
        UiFormats.Date.Should().Be("yyyy-MM-dd");
        UiFormats.DateTime.Should().Be("yyyy-MM-dd HH:mm");
        UiFormats.Money(new Money(12.34m)).Should().Contain("12");
    }
}
