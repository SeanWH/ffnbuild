namespace ffnparser.data.extensions;

using System;

public static class StringExtensions
{
    public static string TranslateHtmlCode(this string text)
    {
        if( text == null )
        {
            throw new ArgumentNullException(nameof(text));
        }
        return System.Net.WebUtility.HtmlDecode(text);
    }
}