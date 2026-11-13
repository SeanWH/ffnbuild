namespace ffnparser.data.model.pages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ffnparser.data.model.addresses;
using ffnparser.data.model.story;

using HtmlAgilityPack;

using Serilog;

public class StoryPage : PageBase
{
    private static List<string> ParseChapterText(HtmlNodeCollection nodes)
    {
        List<string> lines = [];
        foreach( var para in nodes )
        {
            lines.Add(para.InnerText);
        }
        return lines;
    }

    private static List<string> ParseOldStyleStoryText(string oldStyleText)
    {
        List<string> lines = [];

        Regex quotes = new(@"^""|""$");
        Regex lineBreaks = new(@"\r\n|\r|\n");

        string[] splitLines = oldStyleText.Split(@"<br><br>", StringSplitOptions.RemoveEmptyEntries);

        foreach( var line in splitLines )
        {
            string cleanedLine = quotes.Replace(line.Trim(), string.Empty).Trim();
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

    private string GetChapterTitle()
    {
        if( Document is not null )
        {
            var selectNode = Document.DocumentNode
                .SelectNodes("//select[@id='chap_select']/option[@selected='']")?[
                0];
            if( selectNode is not null )
            {
                return selectNode.InnerText.Split('.')[1].Trim();
            }
            else
            {
                return string.Empty;
            }
        }
        else
        {
            throw new ArgumentNullException(nameof(Document), "Critical Error: Document is null. Cannot get chapter title.");
        }
    }

    private (string mediaType, string genre) GetMediaTypeAndGenre()
    {
        string mediatype = string.Empty;
        string genre = string.Empty;
        if( Document is not null )
        {
            var genreNodes = Document.DocumentNode
                .SelectNodes("//div[@id='pre_story_links']//a");
            if( genreNodes is null || genreNodes.Count == 0 )
            {
                return (mediatype, genre);
            }
            else
            {
                if( genreNodes.Count == 2 )
                {
                    mediatype = genreNodes[0].InnerText;
                    genre = genreNodes[1].InnerText;
                    return (mediatype, genre);
                }
                else
                {
                    genre = genreNodes[0].InnerText;
                    return ("crossover", genre);
                }
            }
        }

        throw new ArgumentNullException(nameof(Document), "Critical Error: Document is null. Cannot get media type and genre.");
    }

    private StoryData? GetPageStoryData()
    {
        try
        {
            IFfnAddress? storyAddress = FfnAddressFactory.GetAddress(CanonicalUrl);
            (string storyTitle, string storyId, int chapterIdx) = GetStoryInfo();
            (string mediaType, string genre) = GetMediaTypeAndGenre();
            string chapterTitle = GetChapterTitle();

            var storyText = ProcessChapterHtml();

            var chapterData = new ChapterData(
                chapterIdx,
                chapterTitle,
                storyText,
                storyId);

            var authorAddress = GetStoryAuthor();

            if( storyAddress is StoryAddress sAddress && authorAddress is UserAddress aAddress )
            {
                var storyData = new StoryData(
                storyTitle,
                sAddress,
                aAddress);
                storyData.AddChapter(chapterIdx, chapterData);
                storyData.MediaType = mediaType;
                storyData.Genre = genre;

                return storyData;
            }

            return null;
        }
        catch( Exception ex )
        {
            Log.Error(
                ex,
                "Error getting Story Page data from: {CanonicalUrl}.\nError Was: {Message}\nStack Trace: {Trace}",
                CanonicalUrl,
                ex.Message,
                ex.StackTrace);
            return null;
        }
    }

    private IFfnAddress? GetStoryAuthor()
    {
        string aNode = Document?.DocumentNode?.SelectNodes("//div[@id='profile_top']/a[contains(@href,'/u/')]/@href")?[0]?.Attributes["href"]?.Value ?? string.Empty;

        if( string.IsNullOrEmpty(aNode) )
        {
            return null;
        }

        return FfnAddressFactory.GetAddress(aNode)!;
    }

    private (string storyTitle, string storyId, int chapterIdx) GetStoryInfo()
    {
        string storyTitle = string.Empty;
        string storyId = string.Empty;
        int chapterIdx = 0;

        if( !string.IsNullOrEmpty(CanonicalUrl) )
        {
            var temp = CanonicalUrl.
                Split("/s/", System.StringSplitOptions.None)[1];
            var idAndName = temp.Split('/', StringSplitOptions.TrimEntries);
            if( idAndName.Length == 2 )
            {
                storyId = idAndName[0];
                storyTitle = idAndName[1];
            }
            else if( idAndName.Length == 3 )
            {
                storyId = idAndName[0];
                chapterIdx = Convert.ToInt32(idAndName[1]);
                storyTitle = idAndName[2];
            }
        }
        return (storyTitle, storyId, chapterIdx);
    }

    private List<string> ProcessChapterHtml()
    {
        var storyText = Document?.DocumentNode
            ?.SelectSingleNode(
                "//div[contains(concat(' ', normalize-space(@class), ' '),' storytext ')]");
        var paras = storyText?.SelectNodes("//p");
        List<string> lines;
        if( storyText is not null && (paras == null || paras.Count == 0) )
        {
            lines = ParseOldStyleStoryText(storyText.InnerHtml);
        }
        else if( paras != null && paras.Count > 0 )
        {
            lines = ParseChapterText(paras);
        }
        else
        {
            lines = new List<string>();
        }

        return lines ?? new List<string>();
    }

    public StoryData? StoryData
    {
        get; private set;
    }

    public StoryPage(string pageHtml)
    {
        PageSource = pageHtml;
        StoryData = GetPageStoryData();
    }
}