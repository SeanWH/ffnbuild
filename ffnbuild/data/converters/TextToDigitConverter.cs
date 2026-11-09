namespace ffnbuild.data.converters;

using System.Collections.Generic;

using ffnbuild.data.extensions;

public static class TextToDigitConverter
{
    public static string Convert(string? input)
    {
        if( string.IsNullOrEmpty(input) )
        {
            return string.Empty;
        }
        var digitMap = new Dictionary<string, int>
        {
            { "zero", 0 },
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "ten", 10 },
            { "eleven", 11 },
            { "twelve", 12 },
            { "thirteen", 13 },
            { "fourteen", 14 },
            { "fifteen", 15 },
            { "sixteen", 16 },
            { "seventeen", 17 },
            { "eighteen", 18 },
            { "nineteen", 19 },
            { "twenty", 20 },
            { "thirty", 30 },
            { "forty", 40 },
            { "fifty", 50 },
            { "sixty", 60 },
            { "seventy", 70 },
            { "eighty", 80 },
            { "ninety", 90 }
        };

        var words = input.Split([' ', '-', ','], StringSplitOptions.RemoveEmptyEntries);

        List<string> numberParts = [];

        foreach( var word in words )
        {
            var lowerWord = word.ToLowerInvariant();
            if( digitMap.ContainsKey(lowerWord) )
            {
                numberParts.Add(lowerWord);
            }
        }

        if( numberParts.Count == 0 )
        {
            return input;
        }

        int total = 0;

        if( numberParts.Count == 1 )
        {
            total = digitMap[numberParts[0]];
        }
        else
        {
            foreach( var word in numberParts )
            {
                total += digitMap[word];
            }
        }

        return total.ToString("00");
    }

    public static bool IsANumber(this string? input)
    {
        if( string.IsNullOrEmpty(input) )
        {
            return false;
        }
        string[] digits = ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"];

        if( input.Contains("-") )
        {
            var parts = input.Split('-');
            bool isANumber = false;
            foreach( var part in parts )
            {
                isANumber |= part.ToLower().IsIn(digits);
            }
            return isANumber;
        }

        return input.ToLower().IsIn(digits);
    }
}