namespace parser_lib.Tests.extensions;

using System.Collections.Generic;

using parser_lib.extensions;

public class CollectionIsNullOrEmptyTests
{
    [Test]
    public void ReturnsFalseForCollectionWithNonEmptyString()
    {
        IEnumerable<string?> collection = new List<string?> { null, "", "    ", "non-empty" };
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.False(result);
    }

    [Test]
    public void ReturnsFalseForCollectionWithNonStringObjects()
    {
        IEnumerable<object?> collection = new List<object?> { null, "", "   ", 42 };
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.False(result);
    }

    [Test]
    public void ReturnsTrueForCollectionWithOnlyNullOrEmptyStrings()
    {
        IEnumerable<string?> collection = new List<string?> { null, "", "   " };
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.True(result);
    }

    [Test]
    public void ReturnsTrueForEmptyCollection()
    {
        IEnumerable<string?> collection = new List<string?>();
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.True(result);
    }

    [Test]
    public void ReturnsTrueForNullCollection()
    {
        IEnumerable<string?> collection = null;
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.True(result);
    }
}