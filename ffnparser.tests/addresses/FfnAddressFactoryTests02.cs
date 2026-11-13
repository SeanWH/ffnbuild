namespace ffnparser.tests.addresses;

using System;

using ffnparser.data.model.addresses;

using Xunit;

public class FfnAddressFactoryTests02
{
    [Theory]
    [InlineData("/s/not-a-number/1/Story-Title")]
    [InlineData("/u/not-a-number/AuthorName")]
    [InlineData("u/not-a-number/AuthorName")]
    public void GetAddress_WithNonNumericId_ThrowsFormatException(string input)
    {
        // Act & Assert
        Assert.Throws<FormatException>(() => FfnAddressFactory.GetAddress(input));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void GetAddress_WithNullOrWhitespace_ReturnsNull(string? address)
    {
        // Act
        var result = FfnAddressFactory.GetAddress(address!);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("/book/Harry-Potter/")]
    [InlineData("https://fanfiction.net")]
    [InlineData("https://fanfiction.net")]
    [InlineData("u/12345/AuthorName/extra-segment")]
    public void GetAddress_WithOtherMultiSegmentPaths_FallsBackToPageAddress(string input)
    {
        // Act
        var result = FfnAddressFactory.GetAddress(input);

        // Assert
        var pageAddress = Assert.IsType<PageAddress>(result);
        Assert.Equal(input, pageAddress.Address);
    }

    [Fact]
    public void GetAddress_WithRelativeUserUrlWithoutLeadingSlash_PrependsDomainAndReturnsUserAddress()
    {
        // Arrange: parts = ["u", "123456", "AuthorName"] -> parts.Length == 3
        // && parts[0] == "u"
        const string input = "u/123456/AuthorName";

        // Act
        var result = FfnAddressFactory.GetAddress(input);

        // Assert
        var userAddress = Assert.IsType<UserAddress>(result);
        Assert.Equal("https://www.fanfiction.net/u/123456/AuthorName", userAddress.Address);
        Assert.Equal(123456, userAddress.LinkedId);
        Assert.Equal("AuthorName", userAddress.UserName);
    }

    [Theory]
    [InlineData("just-a-slug")]
    [InlineData("https://www.fanfiction.net")]
    [InlineData("http://fanfiction.net")]
    public void GetAddress_WithSingleSegmentAddress_ReturnsPageAddress(string address)
    {
        // Act
        var result = FfnAddressFactory.GetAddress(address);

        // Assert
        var pageAddress = Assert.IsType<PageAddress>(result);
        Assert.Equal(address, pageAddress.Address);
    }

    [Theory]
    [InlineData("/u/987654/RelativeSlashAuthor", 987654, "RelativeSlashAuthor")]
    [InlineData("https://fanfiction.net/u/555444/HttpsAuthor", 555444, "HttpsAuthor")]
    [InlineData("http://fanfiction.net/u/333222/HttpAuthor", 333222, "HttpAuthor")]
    public void GetAddress_WithStandardUserUrl_ReturnsUserAddress(
        string input,
        int expectedId,
        string expectedUserName)
    {
        // Act
        var result = FfnAddressFactory.GetAddress(input);

        // Assert
        var userAddress = Assert.IsType<UserAddress>(result);
        Assert.Equal(input, userAddress.Address);
        Assert.Equal(expectedId, userAddress.LinkedId);
        Assert.Equal(expectedUserName, userAddress.UserName);
    }

    [Theory]
    [InlineData("/s/12345678/1/Story-Title", 12345678, "Story-Title", "1")]
    [InlineData("https://fanfiction.net/s/87654321/14/Another-Story", 87654321, "Another-Story", "14")]
    [InlineData("http://fanfiction.net/s/112233/5/Http-Story", 112233, "Http-Story", "5")]
    public void GetAddress_WithStoryUrl_ReturnsStoryAddress(
        string input,
        int expectedStoryId,
        string expectedStoryName,
        string expectedChapter)
    {
        // Act
        var result = FfnAddressFactory.GetAddress(input);

        // Assert
        var storyAddress = Assert.IsType<StoryAddress>(result);
        Assert.Equal(input, storyAddress.Address);
        Assert.Equal(expectedStoryId, storyAddress.LinkedId);
        Assert.Equal(expectedStoryName, storyAddress.StoryTitle);
        Assert.Equal(expectedChapter, storyAddress.ChapterId);
    }

    [Theory]
    [InlineData("/s/12345")]
    [InlineData("/u/12345")]
    [InlineData("u/12345")]
    public void GetAddress_WithTruncatedStoryOrUserUrl_ThrowsIndexOutOfRangeException(string input)
    {
        // Act & Assert
        Assert.Throws<IndexOutOfRangeException>(() => FfnAddressFactory.GetAddress(input));
    }
}