namespace ffnparser.tests.addresses;

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
        var actual = IFfnAddress.FormatText(input);
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
        var resultNull = IFfnAddress.FormatText(nullInput);
        var resultWhitespace = IFfnAddress.FormatText(whitespaceInput);
        var resultEmpty = IFfnAddress.FormatText(emptyInput);
        // Assert
        Assert.Equal(string.Empty, resultNull);
        Assert.Equal(string.Empty, resultWhitespace);
        Assert.Equal(string.Empty, resultEmpty);
    }
}