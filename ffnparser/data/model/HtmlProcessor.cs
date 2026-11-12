namespace ffnparser.data.model;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using ffnparser.data.model.addresses;

using HtmlAgilityPack;

public class HtmlProcessor
{
    private readonly HtmlDocument _htmlDocument;

    private static List<string?> ParseChapterText(HtmlNodeCollection nodes)
    {
        List<string?> lines = [];
        Regex lineEndingRegex = new Regex(@"\r\n|\n|\r");
        Regex excessiveSpacingRegex = new Regex(@"\s{2,}");
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

        Regex quotes = new Regex(@"^""|""$");
        Regex lineBreaks = new Regex(@"\r\n|\n|\r");

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

    public HtmlProcessor(HtmlDocument doc)
    {
        _htmlDocument = doc;
    }

    public HtmlProcessor(string filePath)
    {
        _htmlDocument = new HtmlDocument();
        _htmlDocument.Load(filePath);
    }

    public List<string?> GetChapterText()
    {
        var storyText = _htmlDocument.DocumentNode.SelectSingleNode("//div[contains(concat(' ', normalize-space(@class), ' '),' storytext ')]");
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
        var selected_chapter = _htmlDocument.DocumentNode.SelectNodes("//select[@id='chap_select']/option[@selected]");
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
        var href = _htmlDocument.DocumentNode.SelectNodes("//link[@rel='canonical']")[0].Attributes["href"].Value;
        if( href != null )
        {
            var temp = href.Split("/s/", StringSplitOptions.None)[1];
            return temp.Split('/', StringSplitOptions.TrimEntries)[2];
        }
        return string.Empty;
    }

    public string GetUrl()
    {
        if( _htmlDocument is not null && _htmlDocument.DocumentNode is not null )
        {
            var canonicalLink = _htmlDocument.DocumentNode.SelectNodes("//link[@rel='canonical']")?[0];
            if( canonicalLink is not null )
            {
                var hrefAttribute = canonicalLink.Attributes["href"];
                if( hrefAttribute is not null )
                {
                    return hrefAttribute.Value ?? string.Empty;
                }
            }
        }
        return string.Empty;
    }

    public UserAddress? GetUserAddress()
    {
        var a_s = _htmlDocument.DocumentNode.SelectNodes("//a[@class='xcontrast_txt']");
        if( a_s != null && a_s.Count > 0 )
        {
            foreach( var a in a_s )
            {
                var href = a.GetAttributeValue("href", string.Empty);
                if( !string.IsNullOrWhiteSpace(href) && href.Contains("/u/") )
                {
                    return FfnAddressFactory.GetAddress(href) as UserAddress;
                }
            }
        }
        return null;
    }

    public string? GetUserName()
    {
        var userAddress = GetUserAddress();
        if( userAddress != null )
        {
            return userAddress.UserName;
        }
        return string.Empty;
    }
}