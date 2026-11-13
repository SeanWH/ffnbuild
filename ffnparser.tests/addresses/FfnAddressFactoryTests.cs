namespace ffnparser.tests.addresses;

using ffnparser.data.model.addresses;

using Xunit;

public sealed class FfnAddressFactoryTests
{
    [Theory]
    [InlineData("https://fanfiction.net/u/67890/SomeAuthor")]
    [InlineData("http://fanfiction.net/u/67890/SomeAuthor")]
    [InlineData("www.fanfiction.net/u/67890/SomeAuthor")]
    public void GetAddress_FullUserUrl_ReturnsUserAddress(string address)
    {
        var result = FfnAddressFactory.GetAddress(address);

        var user = Assert.IsType<UserAddress>(result);
        Assert.Equal(address, user.Address);
        Assert.Equal(67890, user.LinkedId);
        Assert.Equal("SomeAuthor", user.UserName);
    }

    [Fact]
    public void GetAddress_NonFanfictionNetUrl_ReturnsPageAddress()
    {
        var result = FfnAddressFactory.GetAddress("https://example.com/some/path");
        var page = Assert.IsType<PageAddress>(result);
        Assert.Equal("https://example.com/some/path", page.Address);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\r\n")]
    public void GetAddress_NullOrWhitespace_ReturnsNull(string? address)
    {
        var result = FfnAddressFactory.GetAddress(address!);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("https://fanfiction.net")]
    [InlineData("http://fanfiction.net/community/Writers-Guild/1234/")]
    [InlineData("www.fanfiction.net/forum/General/")]
    public void GetAddress_OtherMultiSegmentUrl_ReturnsPageAddress(string address)
    {
        var result = FfnAddressFactory.GetAddress(address);

        var page = Assert.IsType<PageAddress>(result);
        Assert.Equal(address, page.Address);
    }

    [Fact]
    public void GetAddress_RelativeUserPathWithFourOrMoreParts_SkipsRelativeBranchAndHitsParts1Check()
    {
        // Splits into ["u", "u", "12345", "AuthorName"] -> parts.Length == 4
        // (covers parts.Length < 4 false branch)
        var result = FfnAddressFactory.GetAddress("https://www.fanfiction.net/u/12345/AuthorName");

        var user = Assert.IsType<UserAddress>(result);
        Assert.Equal("https://www.fanfiction.net/u/12345/AuthorName", user.Address);
        Assert.Equal(12345, user.LinkedId);
        Assert.Equal("AuthorName", user.UserName);
    }

    [Fact]
    public void GetAddress_RelativeUserPathWithThreeParts_PrependsDomainAndReturnsUserAddress()
    {
        // Splits into ["u", "12345", "AuthorName"] -> parts.Length == 3 (< 4)
        // and parts[0] == "u"
        var result = FfnAddressFactory.GetAddress("u/12345/AuthorName");

        var user = Assert.IsType<UserAddress>(result);
        Assert.Equal("https://www.fanfiction.net/u/12345/AuthorName", user.Address);
        Assert.Equal(12345, user.LinkedId);
        Assert.Equal("AuthorName", user.UserName);
    }

    [Theory]
    [InlineData("fanfiction.net")]
    [InlineData("https://www.fanfiction.net")]
    [InlineData("http://fanfiction.net")]
    public void GetAddress_SingleSegmentAfterProtocolRemoval_ReturnsPageAddress(string address)
    {
        var result = FfnAddressFactory.GetAddress(address);

        var page = Assert.IsType<PageAddress>(result);
        Assert.Equal(address, page.Address);
    }

    [Theory]
    [InlineData("https://fanfiction.net/s/12345/1/My-Story-Title")]
    [InlineData("http://fanfiction.net/s/12345/1/My-Story-Title")]
    [InlineData("www.fanfiction.net/s/12345/1/My-Story-Title")]
    public void GetAddress_StoryUrl_ReturnsStoryAddress(string address)
    {
        var result = FfnAddressFactory.GetAddress(address);

        var story = Assert.IsType<StoryAddress>(result);
        Assert.Equal(address, story.Address);
        Assert.Equal(12345, story.LinkedId);
        Assert.Equal("My-Story-Title", story.StoryTitle);
        Assert.Equal("1", story.ChapterId);
    }
}