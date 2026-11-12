namespace ffnparser.tests.addresses;

using ffnparser.data.extensions;
using ffnparser.data.model.addresses;

using System;

using Xunit;

public sealed class PageAddressTests
{
    // Minimal test double with read-only properties for cross-type IFfnAddress comparisons
    private sealed class DummyAddress : IFfnAddress
    {
        public string Address { get; }

        public int LinkedId { get; }

        public string LinkType { get; }

        public string Text => "dummy-text";

        public DummyAddress(string linkType = "dummy", string? address = "dummy-address", int linkedId = 999999)
        {
            LinkType = linkType;
            Address = address!;
            LinkedId = linkedId;
        }

        public int CompareTo(IFfnAddress? other) => 0;

        public bool Equals(IFfnAddress? other) => false;
    }

    [Theory]
    [InlineData("user", -1)]  // "page" < "user"
    [InlineData("group", 1)]  // "page" > "group"
    public void CompareTo_DifferentLinkType_OrdersByLinkTypeOrdinal(string otherLinkType, int expectedSign)
    {
        var sut = new PageAddress("https://example.com");
        IFfnAddress other = new DummyAddress(otherLinkType, "https://example.com");

        var actual = sut.CompareTo(other);

        Assert.Equal(expectedSign, Math.Sign(actual));
    }

    [Fact]
    public void CompareTo_Null_ReturnsPositive()
    {
        var sut = new PageAddress("https://example.com");

        Assert.True(sut.CompareTo(null) > 0);
    }

    [Theory]
    [InlineData("https://example.com", "https://fanfiction.net", -1)]
    [InlineData("https://fanfiction.net", "https://example.com", 1)]
    [InlineData("https://example.com", "https://example.com", 0)]
    public void CompareTo_SameLinkType_OrdersByAddressOrdinal(string leftAddr, string rightAddr, int expectedSign)
    {
        var left = new PageAddress(leftAddr);
        var right = new PageAddress(rightAddr);

        var actual = left.CompareTo(right);

        Assert.Equal(expectedSign, Math.Sign(actual));
    }

    [Fact]
    public void CompareTo_SameLinkType_OtherAddressIsNull_ReturnsPositive()
    {
        var sut = new PageAddress("https://example.com");
        IFfnAddress other = new DummyAddress("page", null);

        var actual = sut.CompareTo(other);

        Assert.True(actual > 0);
    }

    [Fact]
    public void Constructor_WithAllParameters_SetsPropertiesCorrectly()
    {
        var sut = new PageAddress("https://example.com", 1);

        Assert.Equal("https://example.com", sut.Address);
        Assert.Equal(1, sut.LinkedId);
        Assert.Equal("page", sut.LinkType);
        Assert.Equal("https://example.com", sut.Text);
    }

    [Fact]
    public void Constructor_WithDefaultLinkedId_SetsZero()
    {
        var sut = new PageAddress("https://example.com");

        Assert.Equal("https://example.com", sut.Address);
        Assert.Equal(0, sut.LinkedId);
    }

    [Fact]
    public void EqualityOperators_HandleNullsAndValuesProperly()
    {
        PageAddress? nullLeft = null;
        PageAddress? nullRight = null;
        var a1 = new PageAddress("https://example.com", 1);
        var a2 = new PageAddress("https://example.com", 1);
        var diffLinkedId = new PageAddress("https://example.com", 2);
        var diffAddress = new PageAddress("https://example.com", 1);

        // Both null
        Assert.True(nullLeft == nullRight!);
        Assert.False(nullLeft != nullRight!);

        // One null
        Assert.False(nullLeft == a1);
        Assert.True(nullLeft != a1);
        Assert.False(a1 == nullRight!);
        Assert.True(a1 != nullRight!);

        // Non-null equal
        Assert.True(a1 == a2);
        Assert.False(a1 != a2);

        // Non-null unequal
        Assert.False(a1 == diffLinkedId);
        Assert.True(a1 != diffLinkedId);
        Assert.True(a1 == diffAddress);
        Assert.False(a1 != diffAddress);
    }

    [Fact]
    public void Equals_IFfnAddress_AllFieldsEqual_ReturnsTrue()
    {
        var first = new PageAddress("https://example.com", 1);
        IFfnAddress second = new PageAddress("https://example.com", 1);

        Assert.True(first.Equals(second));
    }

    [Fact]
    public void Equals_IFfnAddress_DifferentAddress_ReturnsFalse()
    {
        var first = new PageAddress("https://example.com", 1);
        IFfnAddress second = new PageAddress("https://fanfiction.net", 1);

        Assert.False(first.Equals(second));
    }

    [Fact]
    public void Equals_IFfnAddress_DifferentLinkedId_ReturnsFalse()
    {
        var first = new PageAddress("https://example.com", 1);
        IFfnAddress second = new PageAddress("https://example.com", 2);

        Assert.False(first.Equals(second));
    }

    [Fact]
    public void Equals_IFfnAddress_DifferentLinkType_ReturnsFalse()
    {
        var sut = new PageAddress("https://example.com", 1);
        IFfnAddress other = new DummyAddress("user", "https://example.com", 1);

        Assert.False(sut.Equals(other));
    }

    [Fact]
    public void Equals_IFfnAddress_Null_ReturnsFalse()
    {
        var sut = new PageAddress("https://example.com", 1);

        Assert.False(sut.Equals((IFfnAddress?)null));
    }

    [Fact]
    public void Equals_Object_DifferentType_ReturnsFalse()
    {
        var sut = new PageAddress("https://example.com", 1);

        Assert.False(sut.Equals("not a PageAddress"));
    }

    [Fact]
    public void Equals_Object_EqualPageAddress_ReturnsTrue()
    {
        var first = new PageAddress("https://example.com", 1);
        object second = new PageAddress("https://example.com", 1);

        Assert.True(first.Equals(second));
    }

    [Fact]
    public void Equals_Object_Null_ReturnsFalse()
    {
        var sut = new PageAddress("https://example.com", 1);

        Assert.False(sut.Equals((object?)null));
    }

    [Fact]
    public void Equals_Object_SameReference_ReturnsTrue()
    {
        var sut = new PageAddress("https://example.com", 1);
        object obj = sut;

        Assert.True(sut.Equals(obj));
    }

    [Fact]
    public void Equals_Object_UnequalPageAddress_ReturnsFalse()
    {
        var first = new PageAddress("https://example.com", 1);
        object second = new PageAddress("https://example.com", 2);

        Assert.False(first.Equals(second));
    }

    [Fact]
    public void GetHashCode_ComputesExpectedXorHash()
    {
        var sut = new PageAddress("https://example.com", 1);

        var addrHash = "https://example.com".Hash256();
        var linkedIdHash = 1.Hash256();
        var linkTypeHash = "page".Hash256();

        var expected = addrHash ^ linkedIdHash ^ linkTypeHash;
        var actual = sut.GetHashCode();


        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetHashCode_EqualInstancesProduceSameHashCode()
    {
        var first = new PageAddress("https://example.com", 1);
        var second = new PageAddress("https://example.com", 1);

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void RelationalOperators_HandleNullsAndOrderingProperly()
    {
        PageAddress? nullLeft = null;
        PageAddress? nullRight = null;
        var smaller = new PageAddress("https://example.com", 1);
        var equal = new PageAddress("https://example.com", 1);
        var larger = new PageAddress("https://fanfiction.net", 1);

        // Both null
        Assert.False(nullLeft! < nullRight!);
        Assert.True(nullLeft! <= nullRight!);
        Assert.False(nullLeft! > nullRight!);
        Assert.True(nullLeft! >= nullRight!);

        // Left is null, right is non-null
        Assert.True(nullLeft! < smaller);
        Assert.True(nullLeft! <= smaller);
        Assert.False(nullLeft! > smaller);
        Assert.False(nullLeft! >= smaller);

        // Left is non-null, right is null (CompareTo(null) > 0)
        Assert.False(smaller < nullRight!);
        Assert.False(smaller <= nullRight!);
        Assert.True(smaller > nullRight!);
        Assert.True(smaller >= nullRight!);

        // Non-null: smaller vs larger
        Assert.True(smaller < larger);
        Assert.True(smaller <= larger);
        Assert.False(smaller > larger);
        Assert.False(smaller >= larger);

        // Non-null: larger vs smaller
        Assert.False(larger < smaller);
        Assert.False(larger <= smaller);
        Assert.True(larger > smaller);
        Assert.True(larger >= smaller);

        // Non-null: equal
        Assert.False(smaller < equal);
        Assert.True(smaller <= equal);
        Assert.False(smaller > equal);
        Assert.True(smaller >= equal);
    }
}