namespace ffnparser.tests.extensions;

using ffnparser.data.extensions;

public class StringExtensionTests
{
    [Fact]
    public void Hash256_ReturnsConsistentHash()
    {
        // Arrange
        var text = "Hello, World!";
        var expected = -1125416244;

        // Act
        var actual = text.Hash256();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Hash256_ThrowsArgumentNullException_ForNullInput()
    {
        // Arrange
        string? text = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => text!.Hash256());
    }

    [Fact]
    public void TranslateHtmlCode_DecodesHtmlEntities()
    {
        // Arrange
        var htmlEncoded = "Hello, &lt;World&gt;!";
        var expected = "Hello, <World>!";
        // Act
        var actual = htmlEncoded.TranslateHtmlCode();
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TranslateHtmlCode_ThrowsArgumentNullException_ForNullInput()
    {
        // Arrange
        string? text = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => text!.TranslateHtmlCode());
    }
}