namespace ffnbuild.Tests;

using Xunit;

public class TextToDigitConverterTests
{
    [Fact]
    public void Convert_NullInput_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, TextToDigitConverter.Convert(null));
    }

    [Fact]
    public void Convert_EmptyInput_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, TextToDigitConverter.Convert(string.Empty));
    }

    [Fact]
    public void Convert_ValidInput_ReturnsConvertedString()
    {
        Assert.Equal("06", TextToDigitConverter.Convert("one two three"));
        Assert.Equal("45", TextToDigitConverter.Convert("forty-five"));
        Assert.Equal("90", TextToDigitConverter.Convert("ninety"));
    }

    [Fact]
    public void Convert_InvalidInput_ReturnsOriginalString()
    {
        Assert.Equal("hello world", TextToDigitConverter.Convert("hello world"));
        Assert.Equal("123abc", TextToDigitConverter.Convert("123abc"));
    }
}