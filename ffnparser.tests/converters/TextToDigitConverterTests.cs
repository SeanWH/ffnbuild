namespace ffnparser.tests.converters;

using ffnparser.data.converters;

using Xunit;

public class TextToDigitConverterTests
{
    [Theory]
    // 1. Null or Empty Guard
    [InlineData(null, "")]
    [InlineData("", "")]

    // 2. No Valid Number Words (numberParts.Count == 0 -> returns original input)
    [InlineData("   ", "   ")]
    [InlineData("hello world", "hello world")]
    [InlineData("hundred", "hundred")] // Not in digitMap

    // 3. Single Number Word (numberParts.Count == 1, formatted as "00")
    [InlineData("zero", "00")]
    [InlineData("five", "05")]
    [InlineData("TEN", "10")]           // Verifies ToLowerInvariant()
    [InlineData("nineteen", "19")]
    [InlineData("ninety", "90")]

    // 4. Multiple Number Words & Delimiters (numberParts.Count > 1)
    [InlineData("twenty-one", "21")]
    [InlineData("thirty four", "34")]
    [InlineData("forty, five", "45")]
    [InlineData("FIFTY-SIX", "56")]

    // 5. Mixed Text and Number Words (ignores non-number words)
    [InlineData("Chapter twenty-two", "22")]
    [InlineData("Item seven, please", "07")]

    // 6. Edge Case: Unintended Summation Behavior
    [InlineData("one two three", "06")] // Current logic sums all matched words
    [InlineData("ninety nine", "99")]
    [InlineData("ninety ninety", "180")] // Exceeds 2 digits, ToString("00") outputs "180"
    public void Convert_CoversAllBranchesAndEdgeCases(string? input, string expected)
    {
        // Act
        string result = TextToDigitConverter.Convert(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_CoversAllDictionaryEntries()
    {
        var expectedMap = new Dictionary<string, string>
        {
            { "zero", "00" }, { "one", "01" }, { "two", "02" }, { "three", "03" },
            { "four", "04" }, { "five", "05" }, { "six", "06" }, { "seven", "07" },
            { "eight", "08" }, { "nine", "09" }, { "ten", "10" }, { "eleven", "11" },
            { "twelve", "12" }, { "thirteen", "13" }, { "fourteen", "14" }, { "fifteen", "15" },
            { "sixteen", "16" }, { "seventeen", "17" }, { "eighteen", "18" }, { "nineteen", "19" },
            { "twenty", "20" }, { "thirty", "30" }, { "forty", "40" }, { "fifty", "50" },
            { "sixty", "60" }, { "seventy", "70" }, { "eighty", "80" }, { "ninety", "90" }
        };

        foreach( var kvp in expectedMap )
        {
            Assert.Equal(kvp.Value, TextToDigitConverter.Convert(kvp.Key));
        }
    }

    [Theory]
    // 1. Null or Empty Guard
    [InlineData(null, false)]
    [InlineData("", false)]

    // 2. Single Words (No Hyphen)
    [InlineData("zero", true)]
    [InlineData("TWENTY", true)]       // Case-insensitivity
    [InlineData("ninety", true)]
    [InlineData("apple", false)]
    [InlineData("twenty one", false)]  // Space-separated returns false in IsANumber

    // 3. Hyphenated Strings (input.Contains("-"))
    [InlineData("twenty-one", true)]   // Both parts valid
    [InlineData("FORTY-TWO", true)]    // Case-insensitivity with hyphen
    [InlineData("twenty-apple", true)] // Partial match (because of |= operator)
    [InlineData("apple-five", true)]   // Partial match on second part
    [InlineData("foo-bar", false)]     // Neither part valid
    [InlineData("-", false)]           // Only hyphen
    public void IsANumber_CoversAllBranchesAndEdgeCases(string? input, bool expected)
    {
        // Act
        bool result = input.IsANumber();

        // Assert
        Assert.Equal(expected, result);
    }
}