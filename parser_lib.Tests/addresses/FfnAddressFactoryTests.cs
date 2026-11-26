namespace parser_lib.Tests.addresses;

using System;

using NUnit.Framework;

using parser_lib.addresses;

[TestFixture]
public class FfnAddressFactoryTests
{
    [Test]
    public void GetAddress_Ao3UserUrl_ReturnsUserAddress()
    {
        var url = "https://archiveofourown.org/users/HowlingGuardian/pseuds/HowlingGuardian";
        var result = FfnAddressFactory.GetAddress(url);

        Assert.That(result, Is.InstanceOf<UserAddress>());
        var user = (UserAddress)result!;
        Assert.Multiple(() =>
        {
            Assert.That(user.Site, Is.EqualTo("a03"));
            Assert.That(user.UserName, Is.EqualTo("HowlingGuardian"));
            Assert.That(user.A03Pseud, Is.EqualTo("HowlingGuardian"));
        });
    }

    [Test]
    public void GetAddress_Ao3WorkUrl_ThrowsIndexOutOfRangeInStoryConstructor()
    {
        // Factory matches parts.Length == 2 && parts[0] == "works", but
        // StoryAddress(address, site, parts) reads parts[3]
        var url = "https://archiveofourown.org/works/30633947";
        Assert.Throws<IndexOutOfRangeException>(() => FfnAddressFactory.GetAddress(url));
    }

    [TestCase("https://fanfiction.net/community/Test-Group-Name/12345", "ffn", "Test Group Name", 12345, "Test-Group-Name")]
    [TestCase("/community/Test-Group-Name-02/67890", "ffn", "Test Group Name 02", 67890, "Test-Group-Name-02")]
    public void GetAddress_FfnGroupUrl_ReturnsGroupAddress(string url, string expectedSite, string expectedText, int expectedId, string expectedName)
    {
        var result = FfnAddressFactory.GetAddress(url);

        Assert.That(result, Is.InstanceOf<GroupAddress>());
        var group = (GroupAddress)result!;

        Assert.Multiple(() =>
        {
            Assert.That(group.Site, Is.EqualTo(expectedSite));
            Assert.That(group.LinkedId, Is.EqualTo(expectedId));
            Assert.That(group.GroupName, Is.EqualTo(expectedName));
            Assert.That(group.Text, Is.EqualTo(expectedText));
        });
    }

    [TestCase("https://fanfiction.net/s/12345/1/Story-Title", "ffn", 12345, "1", "Story Title")]
    [TestCase("http://fanfiction.net/s/67890/1/Another-Story", "ffn", 67890, "1", "Another Story")]
    [TestCase("/s/111/1/Relative-Story", "ffn", 111, "1", "Relative Story")]
    public void GetAddress_FfnStoryUrl_ReturnsStoryAddress(string url, string expectedSite, int expectedId, string expectedChapterId, string expectedText)
    {
        var result = FfnAddressFactory.GetAddress(url);

        Assert.That(result, Is.InstanceOf<StoryAddress>());
        var story = (StoryAddress)result!;
        Assert.Multiple(() =>
        {
            Assert.That(story.Site, Is.EqualTo(expectedSite));
            Assert.That(story.LinkedId, Is.EqualTo(expectedId));
            Assert.That(story.ChapterIndex, Is.EqualTo(expectedChapterId));
            Assert.That(story.Text, Is.EqualTo(expectedText));
        });
    }

    [TestCase("https://fanfiction.net/u/9876/Author-Name", "ffn", 9876, "Author Name")]
    [TestCase("/u/5432/Relative_Author", "ffn", 5432, "Relative Author")]
    public void GetAddress_FfnUserUrl_ReturnsUserAddress(string url, string expectedSite, int expectedId, string expectedText)
    {
        var result = FfnAddressFactory.GetAddress(url);

        Assert.That(result, Is.InstanceOf<UserAddress>());
        var user = (UserAddress)result!;
        Assert.Multiple(() =>
        {
            Assert.That(user.Site, Is.EqualTo(expectedSite));
            Assert.That(user.LinkedId, Is.EqualTo(expectedId));
            Assert.That(user.Text, Is.EqualTo(expectedText));
        });
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void GetAddress_NullOrWhitespace_ReturnsNull(string? url)
    {
        Assert.That(FfnAddressFactory.GetAddress(url!), Is.Null);
    }

    [TestCase("https://fanfiction.net")]
    [TestCase("https://fanfiction.net")]
    [TestCase("https://example.com")]
    public void GetAddress_SinglePartOrUnmatchedMultiPart_ReturnsPageAddress(string url)
    {
        var result = FfnAddressFactory.GetAddress(url);

        Assert.That(result, Is.InstanceOf<PageAddress>());
        Assert.That(result!.Address, Is.EqualTo(url));
    }
}