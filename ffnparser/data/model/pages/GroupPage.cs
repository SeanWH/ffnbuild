namespace ffnparser.data.model.pages;

using System.Collections.Generic;

using ffnparser.data.model.addresses;

public class GroupPage : PageBase
{
    private void ProcessGroupPage()
    {
        GroupType = PageTitle.Replace("Communities | FanFiction", "").Trim();

        var novNodes = Document.DocumentNode.SelectNodes("//a[@class='novtitle']");
        if( novNodes == null )
        {
            return;
        }
        foreach( var node in novNodes )
        {
            var address = node?.Attributes["href"]?.Value;
            if( string.IsNullOrEmpty(address) )
            {
                continue;
            }
            var groupAddress = FfnAddressFactory.GetAddress(address);
            if( groupAddress is GroupAddress ga )
            {
                GroupAddresses.Add(ga);
            }
        }
    }

    public List<GroupAddress> GroupAddresses { get; private set; } = new List<GroupAddress>();

    public string GroupType { get; private set; } = "General";

    public GroupPage(string htmlCode)
    {
        PageSource = htmlCode;
        ProcessGroupPage();
    }
}