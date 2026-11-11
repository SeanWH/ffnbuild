namespace ffnparser.tests.addresses;

using ffnparser.data.extensions;
using ffnparser.data.model.addresses;

public class GroupAddressTests
{
    // Minimal test double to verify non-GroupAddress IFfnAddress comparisons
    private sealed class DummyAddress : IFfnAddress
    {
        public string Address => "dummy-address";

        public int LinkedId => 999999;

        public string LinkType { get; }

        public string Text => "dummy-text";

        public DummyAddress(string linkType = "dummy")
        {
            LinkType = linkType;
        }

        public int CompareTo(IFfnAddress? other) => 0;

        public bool Equals(IFfnAddress? other) => false;
    }

    [Fact]
    public void CompareTo_Null_ThrowsNullReferenceException()
    {
        var sut = new GroupAddress("addr", "Writers Guild", 42);

        Assert.Throws<NullReferenceException>(() => sut.CompareTo(null!));
    }

    [Theory]
    // 1. GroupName differs (first level)
    [InlineData("addrZ", "Alpha", 99, "addrA", "Beta", 10, -1)]
    [InlineData("addrA", "Beta", 10, "addrZ", "Alpha", 99, 1)]
    // 2. GroupName equal, Address differs (second level)
    [InlineData("addrA", "Alpha", 99, "addrB", "Alpha", 10, -1)]
    [InlineData("addrB", "Alpha", 10, "addrA", "Alpha", 99, 1)]
    // 3. GroupName and Address equal, LinkedId differs (third level)
    [InlineData("addrA", "Alpha", 10, "addrA", "Alpha", 20, -1)]
    [InlineData("addrA", "Alpha", 20, "addrA", "Alpha", 10, 1)]
    // 4. All three equal
    [InlineData("addrA", "Alpha", 10, "addrA", "Alpha", 10, 0)]
    public void CompareTo_OtherGroupAddress_OrdersByGroupNameThenAddressThenLinkedId(
        string leftAddr, string leftGroup, int leftId,
        string rightAddr, string rightGroup, int rightId,
        int expectedSign)
    {
        var left = new GroupAddress(leftAddr, leftGroup, leftId);
        var right = new GroupAddress(rightAddr, rightGroup, rightId);

        var actual = left.CompareTo(right);

        Assert.Equal(expectedSign, Math.Sign(actual));
    }

    [Theory]
    [InlineData("user", -1)]  // "group" < "user"
    [InlineData("alpha", 1)]  // "group" > "alpha"
    [InlineData("group", 0)]  // "group" == "group"
    public void CompareTo_OtherIFfnAddressImplementation_ComparesLinkTypeOrdinal(string otherLinkType, int expectedSign)
    {
        var sut = new GroupAddress("addr", "Writers Guild", 42);
        IFfnAddress other = new DummyAddress(otherLinkType);

        var actual = sut.CompareTo(other);

        Assert.Equal(expectedSign, Math.Sign(actual));
    }

    [Fact]
    public void CompareTo_SameReference_ReturnsZero()
    {
        var sut = new GroupAddress("addr", "Writers Guild", 42);

        Assert.Equal(0, sut.CompareTo(sut));
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var sut = new GroupAddress("https://example.com", "Writers Guild", 42);

        Assert.Equal("https://example.com", sut.Address);
        Assert.Equal("Writers Guild", sut.GroupName);
        Assert.Equal(42, sut.LinkedId);
        Assert.Equal("group", sut.LinkType);
        Assert.Equal(IFfnAddress.FormatText("Writers Guild"), sut.Text);
    }

    [Fact]
    public void EqualityOperators_HandleNullsAndValuesProperly()
    {
        GroupAddress? nullLeft = null;
        GroupAddress? nullRight = null;
        var a1 = new GroupAddress("addr", "Alpha", 10);
        var a2 = new GroupAddress("addr", "Alpha", 10);
        var b = new GroupAddress("addr", "Beta", 10);

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
        Assert.False(a1 == b);
        Assert.True(a1 != b);
    }

    [Fact]
    public void Equals_IFfnAddress_AllFieldsEqual_ReturnsTrue()
    {
        var first = new GroupAddress("addr", "Writers Guild", 42);
        IFfnAddress second = new GroupAddress("addr", "Writers Guild", 42);

        Assert.True(first.Equals(second));
    }

    [Theory]
    [InlineData("other-addr", "Writers Guild", 42)]
    [InlineData("addr", "Other Guild", 42)]
    [InlineData("addr", "Writers Guild", 99)]
    public void Equals_IFfnAddress_AnyFieldDifferent_ReturnsFalse(string address, string groupName, int linkedId)
    {
        var first = new GroupAddress("addr", "Writers Guild", 42);
        IFfnAddress second = new GroupAddress(address, groupName, linkedId);

        Assert.False(first.Equals(second));
    }

    [Fact]
    public void Equals_IFfnAddress_NullOrDifferentImplementation_ReturnsFalse()
    {
        var sut = new GroupAddress("addr", "Writers Guild", 42);
        IFfnAddress? nullAddress = null;
        IFfnAddress otherType = new DummyAddress();

        Assert.False(sut.Equals(nullAddress!));
        Assert.False(sut.Equals(otherType));
    }

    [Fact]
    public void Equals_Object_DifferentType_ReturnsFalse()
    {
        var sut = new GroupAddress("addr", "Writers Guild", 42);

        Assert.False(sut.Equals("not a GroupAddress"));
    }

    [Fact]
    public void Equals_Object_EqualGroupAddress_ReturnsTrue()
    {
        var first = new GroupAddress("addr", "Writers Guild", 42);
        object second = new GroupAddress("addr", "Writers Guild", 42);

        Assert.True(first.Equals(second));
    }

    [Fact]
    public void Equals_Object_Null_ReturnsFalse()
    {
        var sut = new GroupAddress("addr", "Writers Guild", 42);

        Assert.False(sut.Equals((object?)null));
    }

    [Fact]
    public void Equals_Object_SameReference_ReturnsTrue()
    {
        var sut = new GroupAddress("addr", "Writers Guild", 42);
        object obj = sut;

        Assert.True(sut.Equals(obj));
    }

    [Fact]
    public void GetHashCode_ComputesExpectedXorHash()
    {
        var sut = new GroupAddress("addr", "Writers Guild", 42);
        var expected = "Writers Guild".Hash256() ^ "addr".Hash256() ^ "group".Hash256() ^ 42.GetHashCode();

        Assert.Equal(expected, sut.GetHashCode());
    }

    [Fact]
    public void GetHashCode_EqualInstancesProduceSameHashCode()
    {
        var first = new GroupAddress("addr", "Writers Guild", 42);
        var second = new GroupAddress("addr", "Writers Guild", 42);

        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void RelationalOperators_HandleNullsAndOrderingProperly()
    {
        GroupAddress? nullLeft = null;
        GroupAddress? nullRight = null;
        var smaller = new GroupAddress("addr", "Alpha", 10);
        var equal = new GroupAddress("addr", "Alpha", 10);
        var larger = new GroupAddress("addr", "Beta", 10);

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