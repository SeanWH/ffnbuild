namespace ffnbuild.Tests;

using System.Collections.Generic;

using Xunit;

public class NaturalStringComparerTests
{
    [Fact]
    public void ShouldSortInNaturalOrder()
    {
        List<string> testData = new List<string>() {
            "Chapter 1", "Chapter 10", "Chapter 11",
            "Chapter 12", "Chapter 13", "Chapter 14",
            "Chapter 2", "Chapter 20", "Chapter 3" };

        List<string> expected = new List<string>() {
            "Chapter 1","Chapter 2","Chapter 3",
            "Chapter 10", "Chapter 11", "Chapter 12",
            "Chapter 13", "Chapter 14","Chapter 20" };

        testData.Sort(new NaturalStringComparer());

        Assert.Equal(expected, testData);
    }

    [Fact]
    public void ComparingNullsReturnsAsEqual()
    {
        string? test1 = null;
        string? test2 = null;

        var comparer = new NaturalStringComparer();

        Assert.Equal(0, comparer.Compare(test1, test2));
    }

    [Fact]
    public void ComparingNullAndStringReturnsAsNegativeOne()
    {
        string? test1 = null;
        string? test2 = "null";

        var comparer = new NaturalStringComparer();

        Assert.Equal(-1, comparer.Compare(test1, test2));
    }

    [Fact]
    public void ComparingNullsReturnsAsPositiveOne()
    {
        string? test1 = "null";
        string? test2 = null;

        var comparer = new NaturalStringComparer();

        Assert.Equal(1, comparer.Compare(test1, test2));
    }

    [Fact]
    public void ComparingNumericStringsReturnsAsEqual()
    {
        string? test1 = "01";
        string? test2 = "01";

        var comparer = new NaturalStringComparer();

        Assert.Equal(0, comparer.Compare(test1, test2));
    }

    [Fact]
    public void ComparingRightNumericStringLessThanLeftReturnsAsNegativeOne()
    {
        string? test1 = "01";
        string? test2 = "02";

        var comparer = new NaturalStringComparer();

        Assert.Equal(-1, comparer.Compare(test1, test2));
    }

    [Fact]
    public void ComparingRightNumericStringGreaterThanLeftReturnsAsPositiveOne()
    {
        string? test1 = "02";
        string? test2 = "01";

        var comparer = new NaturalStringComparer();

        Assert.Equal(1, comparer.Compare(test1, test2));
    }
}