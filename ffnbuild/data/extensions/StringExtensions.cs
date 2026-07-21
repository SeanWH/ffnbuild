namespace ffnbuild.data.extensions;

using System.Text.RegularExpressions;

using ffnbuild.data.converters;

public static class StringExtensions
{
    public static string GetChapterIndex(this string chapterTitle)
    {
        // Extract the numeric part of the chapter title
        var regex = new Regex(@"\d+");
        var match = regex.Match(chapterTitle);
        if( match.Success )
        {
            if( match.Value.Length == 1 )
            {
                return "0" + match.Value;
            }
            else
            {
                var value = TextToDigitConverter.Convert(match.Value);
                if( value == match.Value )
                {
                    return value;
                }
            }
        }
        // If no numeric part is found, return the original title
        return chapterTitle;
    }

    public static string GetChapterTitle(this string chapterTitle)
    {
        //if chapter title contains Prologue, it's automatically chapter 00
        if( chapterTitle.Contains("Prologue,") )
        {
            return "Chapter 00";
        }

        if( chapterTitle.Contains("Chapter ") )
        {
            var chapRegex = new Regex(@"Chapter.+,");
            var chapMatch = chapRegex.Match(chapterTitle);
            if( chapMatch.Success )
            {
                var cleaned = chapMatch.Value.Trim(',');
                var parts = cleaned.Split(" ");
                if( parts[1].Trim(',').IsANumber() )
                {
                    return parts[0] + " " + TextToDigitConverter.Convert(parts[1]);
                }
            }
        }

        // Extract the non-numeric part of the chapter title
        var regex = new Regex(@"Chapter[\w\s:]+");
        var match = regex.Match(chapterTitle);
        if( match.Success )
        {
            return $"Chapter {match.Value.GetChapterIndex()}";
        }
        // If no non-numeric part is found, return the original title
        return chapterTitle;
    }

    public static string GetStoryTitle(this string chapterTitle)
    {
        var index = chapterTitle.IndexOf("Chapter");
        string possibleTitle = string.Empty;
        if( index > 0 )
        {
            possibleTitle = chapterTitle[..index].Trim();
            if( !string.IsNullOrWhiteSpace(possibleTitle) )
            {
                foreach( char c in Path.GetInvalidFileNameChars() )
                {
                    possibleTitle = possibleTitle.Replace(c, '_');
                }
            }
        }
        if( index == -1 || string.IsNullOrWhiteSpace(possibleTitle) )
        {
            possibleTitle = chapterTitle[..(chapterTitle.IndexOf(',') >= 0 ? chapterTitle.IndexOf(',') : chapterTitle.Length)].Trim();
        }
        return possibleTitle;
    }
}