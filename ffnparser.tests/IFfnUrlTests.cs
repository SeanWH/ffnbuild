namespace ffnparser.tests;

using ffnparser.data.model.addresses;

public class IFfnUrlTests
{
    [Fact]
    public void FormatText_ReplacesHyphensAndUnderscoresWithSpaces()
    {
        // Arrange
        string input = "Hello-World_This-Is_A-Test";
        string expected = "Hello World This Is A Test";
        // Act
        var actual = IFfnUrl.FormatText(input);
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FormatText_ReturnsEmptyString_ForNullOrWhitespaceInput()
    {
        // Arrange
        string? nullInput = null;
        string whitespaceInput = "   ";
        string emptyInput = "";
        // Act
        var resultNull = IFfnUrl.FormatText(nullInput);
        var resultWhitespace = IFfnUrl.FormatText(whitespaceInput);
        var resultEmpty = IFfnUrl.FormatText(emptyInput);
        // Assert
        Assert.Equal(string.Empty, resultNull);
        Assert.Equal(string.Empty, resultWhitespace);
        Assert.Equal(string.Empty, resultEmpty);
    }
}