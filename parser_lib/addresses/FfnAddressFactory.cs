namespace parser_lib.addresses;

using parser_lib.extensions;

public static class FfnAddressFactory
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

        if( address.StartsWith("//www.fanfiction.net") || address.StartsWith("//fanfiction.net") )
        {
            address = address.Replace("//", string.Empty);
            address = address.Replace("www.", string.Empty);
            address = address.Replace("fanfiction.net", string.Empty);
            ;
            site = "ffn";
        }

        if( address.StartsWith("//archiveofourown.org") || address.StartsWith("//www.archiveofourown.org") )
        {
            address = address.Replace("//", string.Empty);
            address = address.Replace("www.", string.Empty);
            address = address.Replace("archiveofourown.org", string.Empty);
            site = "a03";
        }

        return new NormalizedAddress(site, address);
    }

    private static AddressParts SplitAddress(string address)
    {
        var normalized = NormalizeAddress(address);
        var parts = normalized.address.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var site = normalized.site;
        if( string.IsNullOrWhiteSpace(site) )
        {
            switch( parts[0] )
            {
                case "s":
                case "u":
                case "community":
                    site = "ffn";
                    break;

                case "works":
                case "users":
                    site = "a03";
                    break;

                default:
                    break;
            }
        }

        return new AddressParts(site, parts);
    }

    public static IFfnAddress? GetAddress(string address)
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
            if( parts[0] == "community" )
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