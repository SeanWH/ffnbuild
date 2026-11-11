namespace ffnparser.tests.addresses;

using ffnparser.data.extensions;
using ffnparser.data.model.addresses;

public class UserAddressTests
{
    [Fact]
    public void EqualityOperators_HandleNullsAndValuesProperly()
    {
        UserAddress? nullLeft = null;
        UserAddress? nullRight = null;
        var a1 = new UserAddress("addr1", 100, "Alice");
        var a2 = new UserAddress("addr2", 100, "Bob");
        var b = new UserAddress("addr3", 200, "Charlie");

        // Both null
        Assert.True(nullLeft == nullRight);
        Assert.False(nullLeft != nullRight);

        // One null
        Assert.False(nullLeft == a1);
        Assert.True(nullLeft != a1);
        Assert.False(a1 == nullRight);
        Assert.True(a1 != nullRight);

        // Non-null equal LinkedId
        Assert.True(a1 == a2);
        Assert.False(a1 != a2);

        // Non-null unequal LinkedId
        Assert.False(a1 == b);
        Assert.True(a1 != b);
    }

    [Fact]
    public void RelationalOperators_HandleNullsAndOrderingProperly()
    {
        UserAddress? nullLeft = null;
        UserAddress? nullRight = null;
        var smaller = new UserAddress("addr1", 100, "Alice");
        var equal = new UserAddress("addr2", 100, "Bob");
        var larger = new UserAddress("addr3", 200, "Charlie");

        // Both null
        Assert.False(nullLeft < nullRight!);
        Assert.True(nullLeft <= nullRight!);
        Assert.False(nullLeft > nullRight!);
        Assert.True(nullLeft >= nullRight!);

        // Left is null, right is non-null
        Assert.True(nullLeft < smaller);
        Assert.True(nullLeft <= smaller);
        Assert.False(nullLeft > smaller);
        Assert.False(nullLeft >= smaller);

        // Left is non-null, right is null (CompareTo(null) returns -1)
        Assert.True(smaller < nullRight!);
        Assert.True(smaller <= nullRight!);
        Assert.False(smaller > nullRight!);
        Assert.False(smaller >= nullRight!);

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

        // Non-null: equal LinkedId
        Assert.False(smaller < equal);
        Assert.True(smaller <= equal);
        Assert.False(smaller > equal);
        Assert.True(smaller >= equal);
    }

    [Fact]
    public void UserAddress_CompareTo_ReturnsNegativeForDifferentLinkedId()
    {
        // Arrange
        var userAddress1 = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        var userAddress2 = new UserAddress("https://www.fanfiction.net/u/456", 456, "Jane Smith");
        // Act
        int result = userAddress1.CompareTo(userAddress2);
        // Assert
        Assert.True(result < 0);
    }

    [Fact]
    public void UserAddress_CompareTo_ReturnsNegativeForNullOther()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        // Act
        int result = userAddress.CompareTo(null);
        // Assert
        Assert.True(result < 0);
    }

    [Fact]
    public void UserAddress_CompareTo_ReturnsZeroForSameInstance()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        // Act
        int result = userAddress.CompareTo(userAddress);
        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void UserAddress_Constructor_InitializesProperties()
    {
        // Arrange
        string address = "https://www.fanfiction.net/u/123";
        int linkedId = 123;
        string linkType = "user";
        string userName = "John Doe";

        // Act
        var userAddress = new UserAddress(address, linkedId, userName);

        // Assert
        Assert.Equal(address, userAddress.Address);
        Assert.Equal(linkedId, userAddress.LinkedId);
        Assert.Equal(linkType, userAddress.LinkType);
        Assert.Equal(userName, userAddress.UserName);
    }

    [Fact]
    public void UserAddress_Equals_ReturnsFalseForDifferentLinkedId()
    {
        // Arrange
        var userAddress1 = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        var userAddress2 = new UserAddress("https://www.fanfiction.net/u/456", 456, "Jane Smith");
        // Act
        bool result = userAddress1.Equals(userAddress2);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UserAddress_Equals_ReturnsFalseForNull()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        // Act
        bool result = userAddress.Equals(null);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UserAddress_Equals_ReturnsTrueForSameLinkedId()
    {
        // Arrange
        var userAddress1 = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        var userAddress2 = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        // Act
        bool result = userAddress1.Equals(userAddress2);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UserAddress_EqualsObject_ReturnsFalseForDifferentType()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        var differentTypeObject = new object();
        // Act
        bool result = userAddress.Equals(differentTypeObject);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UserAddress_EqualsObject_ReturnsTrue_ForDifferentInstancesOfSameObject()
    {
        var userAddress1 = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        var userAddress2 = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        // Act
        bool result = userAddress1.Equals((object)userAddress2);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UserAddress_EqualsObject_ReturnsTrueForSameInstance()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        // Act
        bool result = userAddress.Equals((object)userAddress);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UserAddress_GetHashCode_ReturnsConsistentHash()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        int expectedHash = userAddress.LinkedId.Hash256() ^ userAddress.LinkType.Hash256();
        // Act
        int actualHash = userAddress.GetHashCode();
        // Assert
        Assert.Equal(expectedHash, actualHash);
    }

    [Fact]
    public void UserAddress_TextProperty_FormatsUserName()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", 123, "John-Doe_123");
        string expectedText = IFfnAddress.FormatText(userAddress.UserName);
        // Act
        string actualText = userAddress.Text;
        // Assert
        Assert.Equal(expectedText, actualText);
    }

    [Fact]
    public void UserAddress_ToString_ReturnsFormattedString()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", 123, "John Doe");
        string expectedString = $"User Name: {userAddress.UserName}, Address: {userAddress.Address}, Linked ID: {userAddress.LinkedId}, Text: {userAddress.Text}";
        // Act
        string actualString = userAddress.ToString();
        // Assert
        Assert.Equal(expectedString, actualString);
    }
}