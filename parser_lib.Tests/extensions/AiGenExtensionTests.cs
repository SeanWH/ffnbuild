namespace parser_lib.Tests.extensions;

using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text.Json;

using NUnit.Framework;

using parser_lib.extensions;

[TestFixture]
public class GenericExtensionsTests
{
    [Test]
    public void GetCrc32_NullObject_ThrowsArgumentNullException()
    {
        object? obj = null;
        Assert.Throws<ArgumentNullException>(() => obj.GetCrc32());
    }

    [Test]
    public void GetCrc32_ValidObject_ReturnsExpectedChecksum()
    {
        var input = 12345;
        var expected = Crc32.Hash(JsonSerializer.SerializeToUtf8Bytes(input));

        Assert.That(input.GetCrc32(), Is.EqualTo(expected));
    }

    [Test]
    public void Hash256_NullObject_ThrowsArgumentNullException()
    {
        object? obj = null;
        Assert.Throws<ArgumentNullException>(() => obj.Hash256());
    }

    [Test]
    public void Hash256_ValidObject_ReturnsDeterministicInt32()
    {
        var input = "sample";
        var expectedBytes = SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(input));
        var expected = BitConverter.ToInt32(expectedBytes, 0);

        Assert.That(input.Hash256(), Is.EqualTo(expected));
    }

    [Test]
    public void IsIn_IEnumerable_ItemAbsent_ReturnsFalse()
    {
        IEnumerable<string> list = new List<string> { "a", "b", "c" };
        Assert.That("z".IsIn(list), Is.False);
    }

    [Test]
    public void IsIn_IEnumerable_ItemPresent_ReturnsTrue()
    {
        IEnumerable<string> list = new List<string> { "a", "b", "c" };
        Assert.That("b".IsIn(list), Is.True);
    }

    [Test]
    public void IsIn_IEnumerable_NullCollection_ReturnsFalse()
    {
        IEnumerable<string>? list = null;
        Assert.That("a".IsIn(list!), Is.False);
    }

    [Test]
    public void IsIn_IEnumerable_NullItemAndNullInCollection_ReturnsTrue()
    {
        IEnumerable<string?> list = new List<string?> { "a", null, "c" };
        string? target = null;
        Assert.That(target.IsIn(list!), Is.True);
    }

    [Test]
    public void IsIn_Params_EmptyArgs_ReturnsFalse()
    {
        Assert.That(20.IsIn(), Is.False);
    }

    [Test]
    public void IsIn_Params_ItemPresent_ReturnsTrue()
    {
        Assert.That(20.IsIn(10, 20, 30), Is.True);
    }

    [Test]
    public void IsIn_Params_NullMatch_ReturnsTrue()
    {
        string? target = null;
        Assert.That(target.IsIn("x", null!, "y"), Is.True);
    }

    [Test]
    public void ToByteArray_NullObject_ThrowsArgumentNullException()
    {
        string? obj = null;
        Assert.Throws<ArgumentNullException>(() => obj.ToByteArray());
    }

    [Test]
    public void ToByteArray_ValidObject_ReturnsUtf8JsonBytes()
    {
        var input = "test";
        var expected = JsonSerializer.SerializeToUtf8Bytes(input);
        Assert.That(input.ToByteArray(), Is.EqualTo(expected));
    }
}

[TestFixture]
public class StringExtensionsTests
{
    [Test]
    public void TranslateHtmlCode_NullInput_ThrowsArgumentNullException()
    {
        string? input = null;
        Assert.Throws<ArgumentNullException>(() => input!.TranslateHtmlCode());
    }

    [TestCase("", "")]
    [TestCase("Plain text", "Plain text")]
    [TestCase("Tom &amp; Jerry", "Tom & Jerry")]
    [TestCase("&lt;div&gt;Hello&lt;/div&gt;", "<div>Hello</div>")]
    [TestCase("&quot;Quoted&quot; &#39;Single&#39;", "\"Quoted\" 'Single'")]
    public void TranslateHtmlCode_ValidInput_DecodesHtmlEntities(string input, string expected)
    {
        var result = input.TranslateHtmlCode();
        Assert.That(result, Is.EqualTo(expected));
    }
}

[TestFixture]
public class CollectionExtensionsTests
{
    [Test]
    public void CollectionIsNullOrEmpty_ContainsNonEmptyString_ReturnsFalse()
    {
        var collection = new List<string?> { null, "   ", "valid" };
        Assert.That(collection.CollectionIsNullOrEmpty(), Is.False);
    }

    [Test]
    public void CollectionIsNullOrEmpty_ContainsNonNullValueType_ReturnsFalse()
    {
        var collection = new List<int?> { null, 0 };
        Assert.That(collection.CollectionIsNullOrEmpty(), Is.False);
    }

    [Test]
    public void CollectionIsNullOrEmpty_EmptyCollection_ReturnsTrue()
    {
        var collection = Array.Empty<string>();
        Assert.That(collection.CollectionIsNullOrEmpty(), Is.True);
    }

    [Test]
    public void CollectionIsNullOrEmpty_NullCollection_ReturnsTrue()
    {
        IEnumerable<string>? collection = null;
        Assert.That(collection.CollectionIsNullOrEmpty(), Is.True);
    }

    [Test]
    public void CollectionIsNullOrEmpty_NullsAndWhitespaceStrings_ReturnsTrue()
    {
        var collection = new List<string?> { null, "", "   ", "\t\n" };
        Assert.That(collection.CollectionIsNullOrEmpty(), Is.True);
    }

    [Test]
    public void CollectionIsNullOrEmpty_OnlyNulls_ReturnsTrue()
    {
        var collection = new List<object?> { null, null };
        Assert.That(collection.CollectionIsNullOrEmpty(), Is.True);
    }
}