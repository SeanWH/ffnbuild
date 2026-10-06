namespace ffnbuild.data.extensions;

using System.Security.Cryptography;
using System.Text;
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

    public static int StringHash256(this string val)
    {
        using var algo = SHA256.Create();
        algo.ComputeHash(Encoding.UTF8.GetBytes(val));
        var result = algo.Hash;
        return BitConverter.ToInt32(result, 0);
    }

    public static string TranslateHtmlCode(this string text)
    {
        Regex regex = new(@"&\w+;");
        var symbol = regex.Match(text);

        while( symbol.Success )
        {
            switch( symbol.Value )
            {
                case "&amp;":
                    text = text.Replace(symbol.Value, "&");
                    break;

                case "&lt;":
                    text = text.Replace(symbol.Value, "<");
                    break;

                case "&gt;":
                    text = text.Replace(symbol.Value, ">");
                    break;

                case "&quot;":
                    text = text.Replace(symbol.Value, "\"");
                    break;

                case "&apos;":
                    text = text.Replace(symbol.Value, "'");
                    break;

                default:
                    text = text.Replace(symbol.Value, "-");
                    break;
            }
            symbol = regex.Match(text);
        }

        return text;
    }
}