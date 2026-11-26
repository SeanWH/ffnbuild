namespace ffnbuild.data.model.addresses;

using ffnbuild.data.extensions;

public static class FfnUrlFactory
{
    private sealed record NormalizedAddress(string site, string address);
    private sealed record AddressParts(string site, string[] parts);

    private static NormalizedAddress NormalizeAddress(string address)
    {
        string site = "";

        if( address.StartsWith("https:") )
        {
            address = address.Replace("https:", string.Empty);
        }

        if( address.StartsWith("http:") )
        {
            address = address.Replace("http:", string.Empty);
        }

        if( address.StartsWith("//www.fanfiction.net") )
        {
            address = address.Replace("//www.fanfiction.net/", string.Empty);
            site = "ffn";
        }

        if( address.StartsWith("//archiveofourown.org") )
        {
            address = address.Replace("//archiveofourown.org/", string.Empty);
            site = "a03";
        }

        return new NormalizedAddress(site, address);
    }

    private static AddressParts SplitAddress(string address)
    {
        var normalized = NormalizeAddress(address);
        var parts = normalized.address.Split('/', StringSplitOptions.RemoveEmptyEntries);

        return new AddressParts(normalized.site, parts);
    }

    public static IFfnUrl? GetAddress(string address)
    {
        if( string.IsNullOrWhiteSpace(address) )
        {
            return null;
        }

        var addressParts = SplitAddress(address);

        var parts = addressParts.parts;
        var site = addressParts.site;

        if( parts.Length == 1 )
        {
            //This type of author address does not contain all the required parts
            // or this isn't a recognized fanfiction page.
            return new PageAddress(address);
        }

        if( parts.Length == 2 && parts[0] == "works" )
        {
            // A03 Story
            return new StoryAddress(address, site, parts);
        }

        if( parts.Length == 3 && parts[0].IsIn("community", "u") )
        {
            if( parts[0] == "u" )
            {
                return new UserAddress(address, site, parts);
            }
            if( parts[1] == "community" )
            {
                return new GroupAddress(address, site, parts);
            }
        }

        if( parts.Length == 4 )
        {
            if( parts[0] == "s" )
            {
                return new StoryAddress(address, site, parts);
            }
            if( parts[0] == "users" )
            {
                // A03 User
                return new UserAddress(address, site, parts);
            }
        }

        return new PageAddress(address);
    }
}