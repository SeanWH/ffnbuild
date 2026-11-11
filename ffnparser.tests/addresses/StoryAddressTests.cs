namespace ffnparser.tests.addresses;

using System;

using ffnparser.data.model.addresses;

using Xunit;

public class StoryAddressTests
{
    private static StoryAddress CreateDefault(
            string address = "https://fanfiction.net/s/12345/1/My-Story",
            int linkedId = 12345,
            string storyTitle = "My Story",
            string chapterId = "1",
            string chapterTitle = "Chapter 1")
    {
        return new StoryAddress(address, linkedId, storyTitle, chapterId, chapterTitle);
    }

    // Helper stub to test branches where IFfnAddress is NOT a StoryAddress
    private class FakeOtherAddress : IFfnAddress
    {
        public string Address => "https://fanfiction.net/u/999/FredFlintstone";
        public int LinkedId => 999;
        public string LinkType => "user"; // Or another valid LinkType string
        public string Text => "FredFlintstone";

        public int CompareTo(IFfnAddress? other) => 0;

        public bool Equals(IFfnAddress? other) => false;
    }

    [Theory]
    [InlineData(100, 200, -1)] // Ordinal: 100 < 200
    [InlineData(200, 100, 1)]  // Ordinal: 200 > 100
    [InlineData(10, 2, -1)]    // Ordinal comparison checks char-by-char ('1' < '2')
    [InlineData(123, 123, 0)]  // Equal LinkedIds
    public void CompareTo_AnotherStoryAddress_ComparesLinkedIdOrdinally(int idA, int idB, int expectedSign)
    {
        var storyA = CreateDefault(linkedId: idA, chapterId: "1");
        var storyB = CreateDefault(linkedId: idB, chapterId: "99"); // Different chapterId to prove only LinkedId matters

        int result = storyA.CompareTo(storyB);

        Assert.Equal(expectedSign, Math.Sign(result));
    }

    [Fact]
    public void CompareTo_DifferentIFfnAddressType_ReturnsPositiveOne()
    {
        var sut = CreateDefault();
        IFfnAddress otherType = new FakeOtherAddress();

        int result = sut.CompareTo(otherType);

        Assert.Equal(1, result);
    }

    [Fact]
    public void CompareTo_Null_ReturnsPositiveOne()
    {
        var sut = CreateDefault();

        int result = sut.CompareTo(null);

        Assert.Equal(1, result);
    }

    [Fact]
    public void CompareTo_SameReference_ReturnsZero()
    {
        var sut = CreateDefault();

        int result = sut.CompareTo(sut);

        Assert.Equal(0, result);
    }

    [Fact]
    public void Constructor_SetsAllPropertiesCorrectly()
    {
        // Arrange & Act
        var sut = new StoryAddress(
            address: "https://www.fanfiction.net/s/11511312/9/In-Sapphire-Light",
            linkedId: 11511312,
            storyTitle: "In-Sapphire-Light",
            chapterId: "9",
            chapterTitle: "Chapter 9");

        // Assert
        Assert.Equal("https://www.fanfiction.net/s/11511312/9/In-Sapphire-Light", sut.Address);
        Assert.Equal(11511312, sut.LinkedId);
        Assert.Equal("In-Sapphire-Light", sut.StoryTitle);
        Assert.Equal("9", sut.ChapterId);
        Assert.Equal("Chapter 9", sut.ChapterTitle);
        Assert.Equal("story", sut.LinkType);
        Assert.Equal(IFfnAddress.FormatText("In Sapphire Light"), sut.Text);
    }

    [Fact]
    public void EqualityOperators_HandleNullsAndValues()
    {
        StoryAddress? nullA = null;
        StoryAddress? nullB = null;
        var story1 = CreateDefault(linkedId: 100);
        var story1Duplicate = CreateDefault(linkedId: 100, chapterId: "2");
        var story2 = CreateDefault(linkedId: 200);

        // == operator
        Assert.True(nullA == nullB);
        Assert.False(nullA == story1);
        Assert.False(story1 == nullA);
        Assert.True(story1 == story1Duplicate);
        Assert.False(story1 == story2);

        // != operator
        Assert.False(nullA != nullB);
        Assert.True(nullA != story1);
        Assert.True(story1 != nullA);
        Assert.False(story1 != story1Duplicate);
        Assert.True(story1 != story2);
    }

    [Fact]
    public void Equals_Object_CoversAllBranches()
    {
        var storyA = CreateDefault(linkedId: 777);
        object storySameId = CreateDefault(linkedId: 777, storyTitle: "Different Title");
        object storyDiffId = CreateDefault(linkedId: 888);

        Assert.True(storyA.Equals((object)storyA));     // ReferenceEquals
        Assert.False(storyA.Equals((object?)null));     // Null
        Assert.False(storyA.Equals("wrong type"));      // Unrelated object
        Assert.True(storyA.Equals(storySameId));        // Equal LinkedId
        Assert.False(storyA.Equals(storyDiffId));       // Different LinkedId
    }

    [Fact]
    public void Equals_Object_Null_ReturnsFalse()
    {
        var sut = CreateDefault();

        bool result = sut.Equals((object?)null);

        Assert.False(result);
    }

    [Fact]
    public void Equals_Object_SameReference_ReturnsTrue()
    {
        var sut = CreateDefault();

        bool result = sut.Equals((object)sut);

        Assert.True(result);
    }

    [Fact]
    public void Equals_Object_StoryAddressWithDifferentLinkedId_ReturnsFalse()
    {
        var storyA = CreateDefault(linkedId: 777);
        object storyB = CreateDefault(linkedId: 888);

        Assert.False(storyA.Equals(storyB));
    }

    [Fact]
    public void Equals_Object_StoryAddressWithSameLinkedId_ReturnsTrue()
    {
        var storyA = CreateDefault(linkedId: 777, storyTitle: "Title A");
        object storyB = CreateDefault(linkedId: 777, storyTitle: "Title B");

        Assert.True(storyA.Equals(storyB));
    }

    [Fact]
    public void Equals_Object_UnrelatedType_ReturnsFalse()
    {
        var sut = CreateDefault();

        bool result = sut.Equals("not a story address");

        Assert.False(result);
    }

    [Fact]
    public void Equals_Typed_DifferentIFfnAddressType_ReturnsFalse()
    {
        var sut = CreateDefault();
        IFfnAddress otherType = new FakeOtherAddress();

        bool result = sut.Equals(otherType);

        Assert.False(result);
    }

    [Fact]
    public void Equals_Typed_Null_ReturnsFalse()
    {
        var sut = CreateDefault();

        bool result = sut.Equals((IFfnAddress?)null);

        Assert.False(result);
    }

    [Fact]
    public void Equals_Typed_StoryAddressWithDifferentLinkedId_ReturnsFalse()
    {
        var storyA = CreateDefault(linkedId: 555);
        var storyB = CreateDefault(linkedId: 556);

        Assert.False(storyA.Equals((IFfnAddress)storyB));
    }

    [Fact]
    public void Equals_Typed_StoryAddressWithSameLinkedId_ReturnsTrue()
    {
        var storyA = CreateDefault(linkedId: 555, chapterId: "1", chapterTitle: "Ch 1");
        var storyB = CreateDefault(linkedId: 555, chapterId: "2", chapterTitle: "Ch 2");

        Assert.True(storyA.Equals((IFfnAddress)storyB));
    }

    [Fact]
    public void GetHashCode_ReturnsExpectedHashAndMatchesForEqualStories()
    {
        var storyA = CreateDefault(linkedId: 555, chapterId: "1");
        var storyB = CreateDefault(linkedId: 555, chapterId: "2");
        var storyDifferent = CreateDefault(linkedId: 556);

        int expectedHash = 555.GetHashCode() ^ "story".GetHashCode();

        Assert.Equal(expectedHash, storyA.GetHashCode());
        Assert.Equal(storyA.GetHashCode(), storyB.GetHashCode());
        Assert.NotEqual(storyA.GetHashCode(), storyDifferent.GetHashCode());
    }

    [Fact]
    public void GreaterThanOperator_CoversAllNullAndValueBranches()
    {
        StoryAddress? nullA = null;
        StoryAddress? nullB = null;
        var smaller = CreateDefault(linkedId: 100);
        var equal = CreateDefault(linkedId: 100);
        var larger = CreateDefault(linkedId: 200);

        Assert.False(nullA > nullB);       // !ReferenceEquals(left, null) -> false
        Assert.False(nullA > smaller);     // !ReferenceEquals(left, null) -> false
        Assert.True(smaller > nullA);     // left.CompareTo(null) returns 1 -> 1 > 0 is true
        Assert.True(larger > smaller);     // "200".CompareTo("100") > 0 -> true
        Assert.False(smaller > equal);     // "100".CompareTo("100") > 0 -> false
        Assert.False(smaller > larger);    // "100".CompareTo("200") > 0 -> false
    }

    [Fact]
    public void GreaterThanOrEqualOperator_CoversAllNullAndValueBranches()
    {
        StoryAddress? nullA = null;
        StoryAddress? nullB = null;
        var smaller = CreateDefault(linkedId: 100);
        var equal = CreateDefault(linkedId: 100);
        var larger = CreateDefault(linkedId: 200);

        Assert.True(nullA >= nullB);       // ReferenceEquals(left, null) -> ReferenceEquals(right, null) -> true
        Assert.False(nullA >= smaller);    // ReferenceEquals(left, null) -> ReferenceEquals(right, null) -> false
        Assert.True(larger >= smaller);    // "200".CompareTo("100") >= 0 -> true
        Assert.True(smaller >= equal);     // "100".CompareTo("100") >= 0 -> true
        Assert.False(smaller >= larger);   // "100".CompareTo("200") >= 0 -> false
    }

    [Fact]
    public void LessThanOperator_CoversAllNullAndValueBranches()
    {
        StoryAddress? nullA = null;
        StoryAddress? nullB = null;
        var smaller = CreateDefault(linkedId: 100);
        var equal = CreateDefault(linkedId: 100);
        var larger = CreateDefault(linkedId: 200);

        Assert.False(nullA < nullB);       // ReferenceEquals(left, null) -> !ReferenceEquals(right, null) -> false
        Assert.True(nullA < smaller);      // ReferenceEquals(left, null) -> !ReferenceEquals(right, null) -> true
        Assert.False(smaller < nullA);     // left.CompareTo(null) returns -1 -> -1 < 0 is true! (See note below)
        Assert.True(smaller < larger);     // "100".CompareTo("200") < 0 -> true
        Assert.False(smaller < equal);     // "100".CompareTo("100") < 0 -> false
        Assert.False(larger < smaller);    // "200".CompareTo("100") < 0 -> false
    }

    [Fact]
    public void LessThanOrEqualOperator_CoversAllNullAndValueBranches()
    {
        StoryAddress? nullA = null;
        StoryAddress? nullB = null;
        var smaller = CreateDefault(linkedId: 100);
        var equal = CreateDefault(linkedId: 100);
        var larger = CreateDefault(linkedId: 200);

        Assert.True(nullA <= nullB);       // ReferenceEquals(left, null) -> true
        Assert.True(nullA <= smaller);     // ReferenceEquals(left, null) -> true
        Assert.True(smaller <= larger);    // "100".CompareTo("200") <= 0 -> true
        Assert.True(smaller <= equal);     // "100".CompareTo("100") <= 0 -> true
        Assert.False(larger <= smaller);   // "200".CompareTo("100") <= 0 -> false
    }

    [Fact]
    public void ToString_ReturnsFormattedStringWithAllFields()
    {
        // Arrange
        var sut = new StoryAddress(
            address: "https://example.com",
            linkedId: 123,
            storyTitle: "My Story",
            chapterId: "4",
            chapterTitle: "Fourth Chapter");

        string expected = $"StoryTitle: My Story, ChapterTitle: Fourth Chapter, ChapterId: 4, Address: https://example.com, Linked ID: 123, Text: {sut.Text}";

        // Act
        string actual = sut.ToString();

        // Assert
        Assert.Equal(expected, actual);
    }
}