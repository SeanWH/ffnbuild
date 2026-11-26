namespace parser_lib.Tests.extensions;

using System.Collections.Generic;

using parser_lib.extensions;

public class ObjectIsInCollectionTests
{
    [Test]
    public void ReturnsFalseWhenObjectIsNotInCollection()
    {
        var obj = new object();
        var collection = new List<object>();
        Assert.False(obj.IsIn(collection));
    }

    [Test]
    public void ReturnsFalseWhenObjectIsNotInParams()
    {
        var obj = new object();
        Assert.False(obj.IsIn(new object(), new object(), new object()));
    }

    [Test]
    public void ReturnsFalseWhenObjectIsNullAndNotInCollection()
    {
        object? obj = null;
        var collection = new List<object?> { new object() };
        Assert.False(obj.IsIn(collection));
    }

    [Test]
    public void ReturnsTrueWhenObjectIsInCollection()
    {
        var obj = new object();
        var collection = new List<object> { obj };
        Assert.True(obj.IsIn(collection));
    }

    [Test]
    public void ReturnsTrueWhenObjectIsInParams()
    {
        var obj = new object();
        Assert.True(obj.IsIn(new object(), obj, new object()));
    }

    [Test]
    public void ReturnsTrueWhenObjectIsNullAndInCollection()
    {
        object? obj = null;
        var collection = new List<object?> { null };
        Assert.True(obj.IsIn(collection));
    }

    [Test]
    public void ReturnsTrueWhenObjectIsNullAndInParams()
    {
        object? obj = null;
        Assert.True(obj.IsIn(new object(), null, new object()));
    }
}