namespace ffnparser.tests;

using System.Collections.Generic;

using ffnparser.data.extensions;

public class ObjectIsInCollectionTests
{
    [Fact]
    public void ReturnsFalseWhenObjectIsNotInCollection()
    {
        var obj = new object();
        var collection = new List<object>();
        Assert.False(obj.IsIn(collection));
    }

    [Fact]
    public void ReturnsFalseWhenObjectIsNotInParams()
    {
        var obj = new object();
        Assert.False(obj.IsIn(new object(), new object(), new object()));
    }

    [Fact]
    public void ReturnsFalseWhenObjectIsNullAndNotInCollection()
    {
        object? obj = null;
        var collection = new List<object?> { new object() };
        Assert.False(obj.IsIn(collection));
    }

    [Fact]
    public void ReturnsTrueWhenObjectIsInCollection()
    {
        var obj = new object();
        var collection = new List<object> { obj };
        Assert.True(obj.IsIn(collection));
    }

    [Fact]
    public void ReturnsTrueWhenObjectIsInParams()
    {
        var obj = new object();
        Assert.True(obj.IsIn(new object(), obj, new object()));
    }

    [Fact]
    public void ReturnsTrueWhenObjectIsNullAndInCollection()
    {
        object? obj = null;
        var collection = new List<object?> { null };
        Assert.True(obj.IsIn(collection));
    }

    [Fact]
    public void ReturnsTrueWhenObjectIsNullAndInParams()
    {
        object? obj = null;
        Assert.True(obj.IsIn(new object(), null, new object()));
    }
}