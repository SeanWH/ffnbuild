namespace ffnparser.data.model.pages;

using System.Collections.Generic;

using ffnparser.data.model.addresses;
using ffnparser.data.model.story;

using HtmlAgilityPack;

public class UserPage : PageBase
{
    private UserAddress? GetAuthorAddressFromStoryNode(HtmlNode storyNode)
    {
        var hrefNodes = storyNode?.SelectNodes(".//a[@href]");
        if( hrefNodes == null || hrefNodes.Count == 0 )
        {
            return null;
        }
        foreach( HtmlNode hrefNode in hrefNodes )
        {
            var hrefValue = hrefNode?.Attributes["href"]?.Value ?? string.Empty;
            if( hrefValue.StartsWith("/u/") )
            {
                hrefValue = NormalizeUrl(hrefValue);
                var authorAddress = FfnAddressFactory.GetAddress(hrefValue);
                if( authorAddress is UserAddress userAddress )
                {
                    return userAddress;
                }
            }
        }
        return null;
    }

    private string NormalizeUrl(string url)
    {
        if( url.StartsWith("//www.fanfiction.net") )
        {
            return $"https:{url}";
        }
        else if( url.StartsWith('/') )
        {
            return $"https://www.fanfiction.net{url}";
        }
        else if( url.StartsWith('s') || url.StartsWith('u') )
        {
            return $"https://www.fanfiction.net/{url}";
        }
        else
        {
            return url;
        }
    }

    private void ProcessFavoriteAuthors()
    {
        var favAuthors = Document?.DocumentNode?.SelectNodes("//div[@id='fa']")?[0] ?? null;
        var authorNodes = favAuthors?.SelectNodes(".//a[contains(@href,'/u/')]/@href");
        if( authorNodes == null )
        {
            return;
        }
        foreach( HtmlNode author in authorNodes )
        {
            if( author == null )
            {
                continue;
            }

            if( author.HasAttributes && author?.Attributes["href"] != null )
            {
                var userAddress = author?.Attributes["href"]?.Value ?? string.Empty;
                var address = FfnAddressFactory.GetAddress(userAddress);
                if( address is UserAddress authorAddress )
                {
                    FavoriteAuthors.Add(authorAddress);
                }
            }
        }
    }

    private void ProcessFavoriteStories()
    {
        var storyNodes = Document.DocumentNode
           .SelectNodes("//div[@class='z-list favstories']");
        if( storyNodes == null )
        {
            return;
        }

        var storyData = ProcessFavoriteStoryNodes(storyNodes);
        FavoriteStories.AddRange(storyData);
    }

    private List<StoryData> ProcessFavoriteStoryNodes(HtmlNodeCollection storyNodes)
    {
        List<StoryData> storyDataList = new List<StoryData>();
        foreach( HtmlNode storyNode in storyNodes )
        {
            var storyUrl = storyNode?.SelectNodes(".//a[@class='stitle']")?[0]?.Attributes["href"]?.Value ?? string.Empty;
            storyUrl = NormalizeUrl(storyUrl);
            var storyTitle = storyNode?.Attributes["data-title"]?.Value ?? string.Empty;
            if( string.IsNullOrEmpty(storyUrl) || string.IsNullOrEmpty(storyTitle) )
            {
                continue;
            }
            var storyAddress = FfnAddressFactory.GetAddress(storyUrl);
            var authorAddress = GetAuthorAddressFromStoryNode(storyNode);
            if( storyAddress is StoryAddress sAddress && authorAddress is UserAddress aAddress )
            {
                storyDataList.Add(
                    new StoryData(
                        storyTitle,
                        sAddress,
                        aAddress));
            }
        }
        return storyDataList;
    }

    private void ProcessPage()
    {
        ProcessUserStories();
        ProcessFavoriteStories();
        ProcessFavoriteAuthors();
    }

    private List<StoryData> ProcessStoryNodes(HtmlNodeCollection storyNodes)
    {
        List<StoryData> storyDataList = new List<StoryData>();

        foreach( HtmlNode storyNode in storyNodes )
        {
            var storyUrl = storyNode?.SelectNodes(".//a[@class='stitle']")?[0]?.Attributes["href"]?.Value ?? string.Empty;

            storyUrl = NormalizeUrl(storyUrl);

            var storyTitle = storyNode?.Attributes["data-title"]?.Value ?? string.Empty;

            if( string.IsNullOrEmpty(storyUrl) || string.IsNullOrEmpty(storyTitle) )
            {
                continue;
            }

            var storyAddress = FfnAddressFactory.GetAddress(storyUrl);
            var canonicalUrl = NormalizeUrl(CanonicalUrl);
            var authorAddress = FfnAddressFactory.GetAddress(canonicalUrl);

            if( storyAddress is StoryAddress sAddress && authorAddress is UserAddress aAddress )
            {
                storyDataList.Add(
                    new StoryData(
                        storyTitle,
                        sAddress,
                        aAddress));
            }
        }

        return storyDataList;
    }

    private void ProcessUserStories()
    {
        var storyNodes = Document.DocumentNode
            .SelectNodes("//div[@class='z-list mystories']");

        if( storyNodes == null )
        {
            return;
        }

        var storyData = ProcessStoryNodes(storyNodes);
        UserStories.AddRange(storyData);
    }

    public List<UserAddress> FavoriteAuthors
    {
        get;
        set;
    } = new List<UserAddress>();

    public List<StoryData> FavoriteStories { get; set; } = new();

    public List<StoryData> UserStories { get; set; } = new();

    public UserPage(string pageHtml)
    {
        PageSource = pageHtml;
        ProcessPage();
    }
}