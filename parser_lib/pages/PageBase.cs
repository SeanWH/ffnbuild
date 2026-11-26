namespace parser_lib.pages;

using HtmlAgilityPack;

public abstract class PageBase
{
    public HtmlDocument Document { get; } = new HtmlDocument();

    public bool IsAnErrorPage
    {
        get
        {
            // constant equals variable avoids null problem. constant is never null.
            if( "ffn".Equals(Site) )
            {
                bool result = false;

                var warningNode1 = Document.DocumentNode.SelectNodes("//span[@class='gui_warning']");
                var warningNode2 = Document.DocumentNode.SelectNodes("//span[@class='gui_normal']");

                string warningText = string.Empty;
                if( warningNode2 != null )
                {
                    warningText = warningNode2[0].InnerText;
                }

                if( warningNode1 != null || (
                    !string.IsNullOrEmpty(warningText) &&
                    warningText.Equals("fanfiction.net message type 1", StringComparison.InvariantCultureIgnoreCase)
                    )
                  )
                {
                    result = true;
                }

                return result;
            }
            else
            {
                //TODO: A03 Error Page handling
                return false;
            }
        }
    }

    public string PageSource
    {
        get
        {
            return field ?? string.Empty;
        }
        set
        {
            value = field!;
            if( value is not null )
            {
                Document.LoadHtml(value);
            }
        }
    }

    public string PageTitle => Document?.DocumentNode?.SelectSingleNode("//title")?.InnerText ?? string.Empty;
    public string? Site { get; set; }
}