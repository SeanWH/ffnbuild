namespace ffnparser.data.model.pages;

using HtmlAgilityPack;

public abstract class PageBase
{
    public string CanonicalUrl => Document?.DocumentNode?.SelectSingleNode("//link[@rel='canonical']")?.Attributes["href"]?.Value ?? string.Empty;

    public HtmlDocument Document { get; } = new HtmlDocument();

    public bool IsAnErrorPage
    {
        get
        {
            bool result = false;

            var warningNode1 = Document.DocumentNode.SelectNodes("//span[@class='gui_warning']");
            var warningNode2 = Document.DocumentNode.SelectNodes("//span[@class='gui_normal']");

            string warningText = string.Empty;
            if( warningNode2 != null )
            {
                warningText = warningNode2[0].InnerText;
            }

            if( warningNode1 != null || (!string.IsNullOrEmpty(warningText) && warningText.Equals("fanfiction.net message type 1", StringComparison.InvariantCultureIgnoreCase)) )
            {
                result = true;
            }

            return result;
        }
    }

    public string? PageSource
    {
        get;
        set
        {
            field = value;
            Document.LoadHtml(value);
        }
    }

    public string PageTitle => Document?.DocumentNode?.SelectSingleNode("//title")?.InnerText ?? string.Empty;
}