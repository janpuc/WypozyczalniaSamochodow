using AwesomeAssertions;

using WypozyczalniaSamochodow.App.Presentation;
using WypozyczalniaSamochodow.App.Presentation.Abstraction;
using WypozyczalniaSamochodow.Tests.TestSupport;

using Xunit;

namespace WypozyczalniaSamochodow.Tests.Presentation;

public sealed class PromptsTests
{
    private readonly IPrompts _prompts = new Prompts();

    [Fact]
    public void PromptsValidateAndNormalizeValues()
    {
        using var scope = new TestConsoleScope();

        scope.Console.Input.PushTextWithEnter("  Jan Kowalski  ");
        _prompts.PromptFullName().Should().Be("  Jan Kowalski  ");

        scope.Console.Input.PushTextWithEnter("jan@example.com");
        _prompts.PromptEmail().Should().Be("jan@example.com");
    }

    [Fact]
    public void PromptEmailRejectsInvalidFormatThenAcceptsValid()
    {
        using var scope = new TestConsoleScope();

        scope.Console.Input.PushTextWithEnter("bad-email");
        scope.Console.Input.PushTextWithEnter("jan@example.com");

        _prompts.PromptEmail().Should().Be("jan@example.com");
    }
}
