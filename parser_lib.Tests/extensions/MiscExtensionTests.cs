namespace parser_lib.Tests.extensions;

using System;
using System.Text.Json;

using parser_lib.extensions;

public class MiscExtensionTests
{
    [Test]
    public void Hash256_ReturnsConsistentHash()
    {
        // Arrange
        string input = "test";
        // Act
        int hash = input.Hash256();

        // Assert
        Assert.That(813340237, Is.EqualTo(hash));
    }

    [Test]
    public void Hash256_ReturnsDifferentHashesForDifferentStrings()
    {
        // Arrange
        string input1 = "test1";
        string input2 = "test2";
        // Act
        int hash1 = input1.Hash256();
        int hash2 = input2.Hash256();
        // Assert
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void Hash256_ThrowsArgumentNullException_ForNullInput()
    {
        // Arrange
        string? input = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => input!.Hash256());
    }

    [Test]
    public void ToByteArray_ReturnsCorrectByteArray()
    {
        // Arrange
        string input = "test";
        byte[] expectedBytes = JsonSerializer.SerializeToUtf8Bytes(input);
        // Act
        byte[] actualBytes = input.ToByteArray();
        // Assert
        Assert.That(expectedBytes, Is.EqualTo(actualBytes));
    }

    [Test]
    public void ToByteArray_ThrowsArgumentNullException_ForNullInput()
    {
        // Arrange
        string? input = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => input!.ToByteArray());
    }
}