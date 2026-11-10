namespace ffnparser.tests;

using ffnparser.data.extensions;
using ffnparser.data.model;
using ffnparser.data.model.addresses;

public class UserAddressTests
{
    [Fact]
    public void UserAddress_CompareTo_ReturnsNegativeForDifferentLinkedId()
    {
        // Arrange
        var userAddress1 = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        var userAddress2 = new UserAddress("https://www.fanfiction.net/u/456", "456", LinkType.User, "Jane Smith");
        // Act
        int result = userAddress1.CompareTo(userAddress2);
        // Assert
        Assert.True(result < 0);
    }

    [Fact]
    public void UserAddress_CompareTo_ReturnsNegativeForNullOther()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        // Act
        int result = userAddress.CompareTo(null);
        // Assert
        Assert.True(result < 0);
    }

    [Fact]
    public void UserAddress_CompareTo_ReturnsZeroForSameInstance()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
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
        string linkedId = "123";
        LinkType linkType = LinkType.User;
        string userName = "John Doe";

        // Act
        var userAddress = new UserAddress(address, linkedId, linkType, userName);

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
        var userAddress1 = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        var userAddress2 = new UserAddress("https://www.fanfiction.net/u/456", "456", LinkType.User, "Jane Smith");
        // Act
        bool result = userAddress1.Equals(userAddress2);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UserAddress_Equals_ReturnsFalseForNull()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        // Act
        bool result = userAddress.Equals(null);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UserAddress_Equals_ReturnsTrueForSameLinkedId()
    {
        // Arrange
        var userAddress1 = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        var userAddress2 = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        // Act
        bool result = userAddress1.Equals(userAddress2);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UserAddress_EqualsObject_ReturnsFalseForDifferentType()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        var differentTypeObject = new object();
        // Act
        bool result = userAddress.Equals(differentTypeObject);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UserAddress_EqualsObject_ReturnsTrue_ForDifferentInstancesOfSameObject()
    {
        var userAddress1 = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        var userAddress2 = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        // Act
        bool result = userAddress1.Equals((object)userAddress2);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UserAddress_EqualsObject_ReturnsTrueForSameInstance()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        // Act
        bool result = userAddress.Equals((object)userAddress);
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UserAddress_GetHashCode_ReturnsConsistentHash()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
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
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John-Doe_123");
        string expectedText = IFfnUrl.FormatText(userAddress.UserName);
        // Act
        string actualText = userAddress.Text;
        // Assert
        Assert.Equal(expectedText, actualText);
    }

    [Fact]
    public void UserAddress_ToString_ReturnsFormattedString()
    {
        // Arrange
        var userAddress = new UserAddress("https://www.fanfiction.net/u/123", "123", LinkType.User, "John Doe");
        string expectedString = $"User Name: {userAddress.UserName}, Address: {userAddress.Address}, Linked ID: {userAddress.LinkedId}, Text: {userAddress.Text}";
        // Act
        string actualString = userAddress.ToString();
        // Assert
        Assert.Equal(expectedString, actualString);
    }
}