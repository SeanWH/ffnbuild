namespace ffnbuild.Tests;

using ffnbuild.ext;

using Xunit;

public class HtmlStringExtensionTests
{
    [Fact]
    public void NullTagReturnsTargetString()
    {
        string target = "testVal";
        string? tag = null;
        string expected = "testVal";

        Assert.Equal(expected, target.AddTag(tag));
    }

    [Fact]
    public void EmptyTagReturnsTargetString()
    {
        string target = "testVal";
        string? tag = string.Empty;
        string expected = "testVal";

        Assert.Equal(expected, target.AddTag(tag));
    }

    [Fact]
    public void WhitespaceTagReturnsTargetString()
    {
        string target = "testVal";
        string? tag = "    ";
        string expected = "testVal";

        Assert.Equal(expected, target.AddTag(tag));
    }

    [Fact]
    public void AngleBracketsRemovedFromTag()
    {
        string target = "testVal";
        string? tag = "<b>";
        string expected = "<b>testVal</b>";
        string notExpected = "<<b>>testVal</<b>>";

        Assert.NotEqual(notExpected, target.AddTag(tag));
        Assert.Equal(expected, target.AddTag(tag));
    }

    [Fact]
    public void LeftAngleBracketRemovedFromTag()
    {
        string target = "testVal";
        string? tag = "<b";
        string expected = "<b>testVal</b>";
        string notExpected = "<<b>testVal</<b>>";

        Assert.NotEqual(notExpected, target.AddTag(tag));
        Assert.Equal(expected, target.AddTag(tag));
    }

    [Fact]
    public void RightAngleBracketRemovedFromTag()
    {
        string target = "testVal";
        string? tag = "b>";
        string expected = "<b>testVal</b>";
        string notExpected = "<<b>testVal</<<b>";

        Assert.NotEqual(notExpected, target.AddTag(tag));
        Assert.Equal(expected, target.AddTag(tag));
    }

    [Fact]
    public void BoldReturnsBoldTag()
    {
        string target = "testVal";
        string expected = "<b>testVal</b>";

        Assert.Equal(expected, target.Bold());
    }

    [Fact]
    public void ItalicReturnsItalicTag()
    {
        string target = "testVal";
        string expected = "<i>testVal</i>";

        Assert.Equal(expected, target.Italic());
    }

    [Fact]
    public void UnderlineReturnsUnderlineTag()
    {
        string target = "testVal";
        string expected = "<u>testVal</u>";

        Assert.Equal(expected, target.Underline());
    }

    [Fact]
    public void Heading1Returns_h1_Tag()
    {
        string target = "testVal";
        string expected = "<h1>testVal</h1>";

        Assert.Equal(expected, target.Heading1());
    }

    [Fact]
    public void Heading2Returns_h2_Tag()
    {
        string target = "testVal";
        string expected = "<h2>testVal</h2>";

        Assert.Equal(expected, target.Heading2());
    }

    [Fact]
    public void Heading3Returns_h3_Tag()
    {
        string target = "testVal";
        string expected = "<h3>testVal</h3>";

        Assert.Equal(expected, target.Heading3());
    }

    [Fact]
    public void Heading4Returns_h4_Tag()
    {
        string target = "testVal";
        string expected = "<h4>testVal</h4>";

        Assert.Equal(expected, target.Heading4());
    }

    [Fact]
    public void Heading5Returns_h5_Tag()
    {
        string target = "testVal";
        string expected = "<h5>testVal</h5>";

        Assert.Equal(expected, target.Heading5());
    }

    [Fact]
    public void Heading6Returns_h6_Tag()
    {
        string target = "testVal";
        string expected = "<h6>testVal</h6>";

        Assert.Equal(expected, target.Heading6());
    }

    [Fact]
    public void ParagraphReturns_p_Tag()
    {
        string target = "testVal";
        string expected = "<p>testVal</p>";

        Assert.Equal(expected, target.Paragraph());
    }

    [Fact]
    public void BreakReturns_br_Tag()
    {
        string target = "testVal";
        string expected = "<br>testVal";

        Assert.Equal(expected, target.Break());
    }
}