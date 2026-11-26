namespace parser_lib.Tests.extensions;

using System;

using parser_lib.extensions;

[TestFixture]
public class StringExtensionTests
{
    [Test]
    public void Hash256_ReturnsConsistentHash()
    {
        // Arrange
        var text = "Hello, World!";
        var expected = -1125416244;

        // Act
        var actual = text.Hash256();

        // Assert
        Assert.That(expected, Is.EqualTo(actual));
    }

    [Test]
    public void Hash256_ThrowsArgumentNullException_ForNullInput()
    {
        // Arrange
        string? text = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => text!.Hash256());
    }

    [Test]
    public void TranslateHtmlCode_ThrowsArgumentNullException_ForNullInput()
    {
        // Arrange
        string? text = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => text!.TranslateHtmlCode());
    }

    [TestCase("", "")]
    [TestCase("   ", "   ")]
    [TestCase("Plain text without entities", "Plain text without entities")]
    [TestCase("Tom &amp; Jerry", "Tom & Jerry")]
    [TestCase("&lt;div&gt;Hello&lt;/div&gt;", "<div>Hello</div>")]
    [TestCase("&quot;Quoted&quot; &apos;Single&apos;", "\"Quoted\" 'Single'")]
    [TestCase("Numeric &#39;entity&#39; &amp; &#x26;", "Numeric 'entity' & &")]
    public void TranslateHtmlCode_ValidInput_DecodesHtmlEntities(string input, string expected)
    {
        string result = input.TranslateHtmlCode();

        Assert.That(result, Is.EqualTo(expected));
    }
}