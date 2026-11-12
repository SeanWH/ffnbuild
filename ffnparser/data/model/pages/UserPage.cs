namespace ffnparser.data.model.pages;

using ffnparser.data.model.addresses;
using ffnparser.data.model.story;

using HtmlAgilityPack;

using System.Collections.Generic;

public class UserPage : PageBase
{
    private void ProcessFavoriteAuthors()
    {
        var favAuthors = Document?.DocumentNode?.SelectNodes("//div[@id='fa']")?[0] ?? null;
        var authorNodes = favAuthors?.SelectNodes(".//a[contains(@href,'/u/')]/@href");
        if (authorNodes == null)
        {
            return;
        }
        foreach (HtmlNode author in authorNodes)
        {
            if (author == null)
            {
                continue;
            }

            if (author.HasAttributes && author?.Attributes["href"] != null)
            {
                var userAddress = author?.Attributes["href"]?.Value ?? string.Empty;
                var address = FfnAddressFactory.GetAddress(userAddress);
                if (address is UserAddress authorAddress)
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
        if (storyNodes == null)
        {
            return;
        }

        var storyData = ProcessStoryNodes(storyNodes);
        FavoriteStories.AddRange(storyData);
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

        foreach (HtmlNode storyNode in storyNodes)
        {
            var storyUrl = storyNode?.SelectNodes(".//a[@class='stitle']")?[0]?.Attributes["href"]?.Value ?? string.Empty;

            if (storyUrl.StartsWith('/'))
            {
                storyUrl = $"https://www.fanfiction.net{storyUrl}";
            }
            else if (storyUrl.StartsWith('s'))
            {
                storyUrl = $"https://www.fanfiction.net/{storyUrl}";
            }


            var storyTitle = storyNode?.Attributes["data-title"]?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(storyUrl) || string.IsNullOrEmpty(storyTitle))
            {
                continue;
            }

            var storyAddress = FfnAddressFactory.GetAddress(storyUrl);
            var authorAddress = FfnAddressFactory.GetAddress(CanonicalUrl);

            if (storyAddress is StoryAddress sAddress && authorAddress is UserAddress aAddress)
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

        if (storyNodes == null)
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