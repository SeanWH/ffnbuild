namespace ffnparser.data.model.pages;

using ffnparser.data.model.addresses;

using HtmlAgilityPack;

public static class PageFactory
{
    private static string CanonicalUrl(HtmlDocument document) => document?.DocumentNode
    ?.SelectSingleNode("//link[@rel='canonical']")
    ?.Attributes["href"]?.Value ?? string.Empty;

    public static PageBase? GetPage(string htmlCode)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlCode);
        var canonical = CanonicalUrl(doc);
        var address = FfnAddressFactory.GetAddress(canonical);

        if( address is UserAddress )
        {
            return new UserPage(htmlCode);
        }

        if( address is StoryAddress )
        {
            return new StoryPage(htmlCode);
        }

        return null;
    }
}