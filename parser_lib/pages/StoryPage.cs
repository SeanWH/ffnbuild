namespace parser_lib.pages;

using System.Text;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using parser_lib.addresses;
using parser_lib.extensions;

public class StoryPage : PageBase
{
    public record struct ChapterData(int chapterId, string chapterTitle, List<string> paras, int storyId);
    public record struct Category(string mediaType, string genre);
    public record struct StoryInfo(string storyTitle, int storyId, int chapterIndex);

    private readonly HtmlDocument _document = new HtmlDocument();
    private Dictionary<int, ChapterData> _chapters;

    private static List<string> ParseOldStyleFfnStoryText(string oldStyleText)
    {
        List<string> lines = [];

        Regex quotes = new(@"^""|""$");
        Regex lineBreaks = new(@"\r\n|\r|\n");

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

    private void GetPageData()
    {
        try
        {
            var storyTitle = GetStoryTitle();
        }
        catch( Exception ex )
        {
            //TODO: Log error
        }
    }

    private string GetStoryTitle()
    {
        var title = _document?.DocumentNode?.SelectSingleNode("//title")?.InnerText ?? string.Empty;
        if( string.IsNullOrWhiteSpace(title) )
        {
            return title;
        }
        switch( StoryAddress.Site )
        {
            case "ffn":
                return StoryAddress.StoryTitle;

            case "a03":
                string[] parts = title.Split(" - ");
                return parts[0].Trim();

            default:
                return title;
        }
    }

    public IReadOnlyDictionary<int, ChapterData> Chapters { get => _chapters; }
    public StoryAddress StoryAddress { get; init; }
    public string? StoryTitle { get; set; }
    public UserAddress? UserAddress { get; set; }

    public StoryPage(StoryAddress storyAddress, string pageHtml)
    {
        StoryAddress = storyAddress;
        PageSource = pageHtml;
        _chapters = new Dictionary<int, ChapterData>();
        _document.LoadHtml(pageHtml);
        GetPageData();
    }

    public void AddChapter(int chapterIndex, ChapterData chapterData)
    {
        if( Chapters.ContainsKey(chapterIndex) )
        {
            // Is the chapter identical or updated?
            byte[] current = Chapters[chapterIndex].GetCrc32();
            if( chapterData.GetCrc32() != current )
            {
                _chapters[chapterIndex] = chapterData;
            }
        }
    }
}