namespace ffnparser.tests.extensions;

using System;
using System.Text.Json;

using ffnparser.data.extensions;

public class MiscExtensionTests
{
    [Fact]
    public void Hash256_ReturnsConsistentHash()
    {
        // Arrange
        string input = "test";
        // Act
        int hash = input.Hash256();

        // Assert
        Assert.Equal(813340237, hash);
    }

    [Fact]
    public void Hash256_ReturnsDifferentHashesForDifferentStrings()
    {
        // Arrange
        string input1 = "test1";
        string input2 = "test2";
        // Act
        int hash1 = input1.Hash256();
        int hash2 = input2.Hash256();
        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Hash256_ThrowsArgumentNullException_ForNullInput()
    {
        // Arrange
        string? input = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => input!.Hash256());
    }

    [Fact]
    public void ToByteArray_ReturnsCorrectByteArray()
    {
        // Arrange
        string input = "test";
        byte[] expectedBytes = JsonSerializer.SerializeToUtf8Bytes(input);
        // Act
        byte[] actualBytes = input.ToByteArray();
        // Assert
        Assert.Equal(expectedBytes, actualBytes);
    }

    [Fact]
    public void ToByteArray_ThrowsArgumentNullException_ForNullInput()
    {
        // Arrange
        string? input = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => input!.ToByteArray());
    }
}