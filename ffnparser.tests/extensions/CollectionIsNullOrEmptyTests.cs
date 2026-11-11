namespace ffnparser.tests.extensions;

using ffnparser.data.extensions;

public class CollectionIsNullOrEmptyTests
{
    [Fact]
    public void ReturnsFalseForCollectionWithNonEmptyString()
    {
        IEnumerable<string?> collection = new List<string?> { null, "", "   ", "non-empty" };
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalseForCollectionWithNonStringObjects()
    {
        IEnumerable<object?> collection = new List<object?> { null, "", "   ", 42 };
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.False(result);
    }

    [Fact]
    public void ReturnsTrueForCollectionWithOnlyNullOrEmptyStrings()
    {
        IEnumerable<string?> collection = new List<string?> { null, "", "   " };
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.True(result);
    }

    [Fact]
    public void ReturnsTrueForEmptyCollection()
    {
        IEnumerable<string?> collection = new List<string?>();
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.True(result);
    }

    [Fact]
    public void ReturnsTrueForNullCollection()
    {
        IEnumerable<string?> collection = null;
        bool result = collection.CollectionIsNullOrEmpty();
        Assert.True(result);
    }
}