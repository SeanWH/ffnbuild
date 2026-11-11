namespace ffnparser.tests.comparers;

using System;
using System.Collections.Generic;

using ffnparser.data.comparers;

using Xunit;

public class NaturalStringComparerTests
{
    private readonly NaturalStringComparer _comparer = new();

    [Theory]
    // 1. Null & Empty String Guards
    [InlineData(null, null, 0)]
    [InlineData(null, "a", -1)]
    [InlineData("a", null, 1)]
    [InlineData("", "", 0)]
    [InlineData("", "a", -1)]      // 0 parts vs 1 part -> hits (i >= xParts.Count)
    [InlineData("a", "", 1)]       // 1 part vs 0 parts -> hits (i >= yParts.Count)

    // 2. Part Count Differences (Prefix matches, different lengths)
    [InlineData("file1", "file1a", -1)]   // 2 parts vs 3 parts -> hits (i >= xParts.Count)
    [InlineData("file1a", "file1", 1)]    // 3 parts vs 2 parts -> hits (i >= yParts.Count)

    // 3. Numeric Comparisons (Both parts parse as int)
    [InlineData("item2", "item10", -1)]   // Numeric: 2 < 10 (numCompare < 0)
    [InlineData("item10", "item2", 1)]    // Numeric: 10 > 2 (numCompare > 0)
    [InlineData("item05", "item5", 0)]    // Numeric equal, continues loop -> returns 0 at end

    // 4. Text Comparisons (Case-insensitivity & fall-through)
    [InlineData("apple", "Banana", -1)]   // Text: 'a' < 'B' ignoring case (textCompare < 0)
    [InlineData("Banana", "apple", 1)]    // Text: 'B' > 'a' ignoring case (textCompare > 0)
    [InlineData("File", "file", 0)]       // Text equal ignoring case, continues loop -> returns 0 at end

    // 5. Mixed Type at Same Part Index (Short-circuiting && on TryParse)
    [InlineData("10", "abc", -1)]         // xPart is int (true), yPart is text (false) -> else branch
    [InlineData("abc", "10", 1)]          // xPart is text (false, short-circuits &&) -> else branch

    // 6. Multi-part Fall-throughs (First parts match, later parts decide)
    [InlineData("v1.2.10", "v1.2.2", 1)]  // Parts 0..3 equal, part 4 (10 vs 2) decides
    [InlineData("v01-a", "v1-B", -1)]     // Part 1 equal numerically (01 == 1), part 2 decides ('-a' < '-B')

    // 7. Edge Case: Integer Overflow (> int.MaxValue falls back to string compare)
    [InlineData("99999999999999999999", "2", 1)] // Fails int.TryParse, compares as text ('9' > '2')
    public void Compare_CoversAllBranchesAndEdgeCases(string? x, string? y, int expectedSign)
    {
        // Act
        int result = _comparer.Compare(x, y);

        // Assert
        Assert.Equal(expectedSign, Math.Sign(result));
    }

    [Fact]
    public void Sort_OrdersCollectionNaturally()
    {
        // Arrange
        var unsorted = new List<string?>
        {
            "file10.txt",
            "file2.txt",
            null,
            "file1.txt",
            "File20.txt",
            "file01.txt"
        };

        // Act
        unsorted.Sort(_comparer);

        // Assert
        var expected = new List<string?>
        {
            null,
            "file1.txt",
            "file01.txt",
            "file2.txt",
            "file10.txt",
            "File20.txt"
        };

        Assert.Equal(expected, unsorted);
    }
}