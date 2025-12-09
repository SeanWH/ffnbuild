namespace ffnbuild;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NaturalStringComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        // Handle nulls safely
        if (x == null && y == null)
        {
            return 0;
        }

        if (x == null)
        {
            return -1;
        }

        if (y == null)
        {
            return 1;
        }

        // Split strings into alternating numeric and non-numeric parts
        var regex = new Regex(@"\d+|\D+");
        var xParts = regex.Matches(x);
        var yParts = regex.Matches(y);

        int maxParts = Math.Max(xParts.Count, yParts.Count);

        for (int i = 0; i < maxParts; i++)
        {
            // If one string has fewer parts, it comes first
            if (i >= xParts.Count)
            {
                return -1;
            }

            if (i >= yParts.Count)
            {
                return 1;
            }

            string xPart = xParts[i].Value;
            string yPart = yParts[i].Value;

            // If both parts are numbers, compare numerically
            if (int.TryParse(xPart, out int xNum) && int.TryParse(yPart, out int yNum))
            {
                int numCompare = xNum.CompareTo(yNum);
                if (numCompare != 0)
                {
                    return numCompare;
                }
            }
            else
            {
                // Compare text parts case-insensitively
                int textCompare = string.Compare(xPart, yPart, StringComparison.OrdinalIgnoreCase);
                if (textCompare != 0)
                {
                    return textCompare;
                }
            }
        }

        return 0; // Strings are equal
    }
}