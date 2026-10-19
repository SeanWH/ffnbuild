namespace ffnbuild.data.extensions;

using System.Net;
using System.Security.Cryptography;
using System.Text;

#nullable disable

public static class StringExtensions
{
    public static int StringHash256(this string val)
    {
        using var algo = SHA256.Create();
        _ = algo.ComputeHash(Encoding.UTF8.GetBytes(val));
        var result = algo.Hash;
        return BitConverter.ToInt32(result, 0);
    }

    public static string TranslateHtmlCode(this string text)
    {
        return WebUtility.HtmlDecode(text);
    }
}