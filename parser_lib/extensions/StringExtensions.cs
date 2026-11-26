namespace parser_lib.extensions;

using System.Net;

#nullable disable

public static class StringExtensions
{
    public static string TranslateHtmlCode(this string text)
    {
        if( text is null )
        {
            throw new ArgumentNullException(nameof(text));
        }
        return WebUtility.HtmlDecode(text);
    }
}