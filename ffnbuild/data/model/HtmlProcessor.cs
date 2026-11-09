namespace ffnbuild.data.model;

using System.Text;
using System.Text.RegularExpressions;

using ffnbuild.data.model.addresses;

using HtmlAgilityPack;

internal partial class HtmlProcessor
{
    private readonly HtmlDocument _doc;

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex ExcessiveSpacingRegex();

    [GeneratedRegex(@"\r\n|\n|\r")]
    private static partial Regex LineEndingRegex();

    private static List<string?> ParseChapterText(HtmlNodeCollection nodes)
    {
        List<string?> lines = [];
        Regex lineEndingRegex = LineEndingRegex();
        Regex excessiveSpacingRegex = ExcessiveSpacingRegex();
        foreach( var para in nodes )
        {
            var line = lineEndingRegex.Replace(para.InnerText, " ").Trim();
            line = excessiveSpacingRegex.Replace(line, " ");
            lines.Add(line);
        }
        return lines;
    }

    private static List<string?> ParseOldStyleStoryText(string oldStyleText)
    {
        List<string?> lines = [];

        Regex quotes = QuoteRegex();
        Regex lineBreaks = LineEndingRegex();

        string[] splitLines = oldStyleText.Split(@"<br><br>", StringSplitOptions.RemoveEmptyEntries);

        foreach( var line in splitLines )
        {
            string cleanedLine = quotes.Replace(line, string.Empty).Trim();
            if( !string.IsNullOrWhiteSpace(cleanedLine) )
            {
                string[] subLines = lineBreaks.Split(cleanedLine);
                StringBuilder sb = new();
                foreach( var subLine in subLines.Where(subLine => !string.IsNullOrWhiteSpace(subLine)) )
                {
                    sb.Append($"{subLine.Trim()} ");
                }

                lines.Add(sb.ToString().Trim());
            }
        }

        return lines;
    }

    [GeneratedRegex(@"^""|""$")]
    private static partial Regex QuoteRegex();

    public HtmlProcessor(HtmlDocument doc)
    {
        _doc = doc;
    }

    public HtmlProcessor(string filePath)
    {
        _doc = new HtmlDocument();
        _doc.Load(filePath);
    }

    public AuthorAddress? GetAuthorAddress()
    {
        var a_s = _doc.DocumentNode.SelectNodes("//a[@class='xcontrast_txt']");
        if( a_s != null && a_s.Count > 0 )
        {
            foreach( var a in a_s )
            {
                var href = a.Attributes["href"].Value;
                if( href != null && href.Contains("/u/") )
                {
                    return FfnUrlFactory.GetAddress(href) as AuthorAddress;
                }
            }
        }
        return null;
    }

    public string GetAuthorName()
    {
        var authorAddress = GetAuthorAddress();
        if( authorAddress is not null )
        {
            return authorAddress.AuthorName;
        }
        return string.Empty;
    }

    public List<string?> GetChapterText()
    {
        var storyText = _doc.DocumentNode.SelectSingleNode("//div[contains(concat(' ', normalize-space(@class), ' '),' storytext ')]");
        var paras = storyText.SelectNodes("//p");
        List<string?> lines;
        if( paras == null || paras.Count == 0 )
        {
            lines = ParseOldStyleStoryText(storyText.InnerHtml);
        }
        else
        {
            lines = ParseChapterText(paras);
        }
        return lines;
    }

    public string GetChapterTitle()
    {
        var selected_chapter = _doc.DocumentNode.SelectNodes("//select[@id='chap_select']/option[@selected]");
        if( selected_chapter is not null )
        {
            var tmp = selected_chapter[0].InnerText;

            Regex parts = new Regex(@"^(\d+)\.(.*)$", RegexOptions.IgnoreCase);
            if( parts.IsMatch(tmp) )
            {
                var match = parts.Match(tmp);
                return match.Groups[1].Value.PadLeft(3, '0') + ": " + match.Groups[2].Value.Trim();
            }
            else
            {
                var match = parts.Match(tmp);
                return match.Groups[2].Value.Trim();
            }
        }

        return string.Empty;
    }

    public string GetStoryTitle()
    {
        var href = _doc.DocumentNode.SelectNodes("//link[@rel='canonical']")[0].Attributes["href"].Value;
        if( href != null )
        {
            var temp = href.Split("/s/", StringSplitOptions.None)[1];
            return temp.Split('/', StringSplitOptions.TrimEntries)[2];
        }
        return string.Empty;
    }

    public string GetUrl()
    {
        return _doc.DocumentNode.SelectNodes("//link[@rel='canonical']")[0].Attributes["href"].Value;
    }
}