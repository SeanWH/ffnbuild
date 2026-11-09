namespace ffnbuild.data.model.addresses;

using ffnbuild.data.extensions;

public static class FfnUrlFactory
{
    private const int AUTHOR_ID = 2;
    private const int AUTHOR_NAME = 3;
    private const int CHAPTER_IDX = 3;
    private const int STORY_ID = 2;
    private const int STORY_NAME = 4;

    private static string RemoveProtocol(string address)
    {
        if( address.StartsWith("https://") )
        {
            //removes https://www.
            address = address.Substring(12);
        }

        if( address.StartsWith("http://") )
        {
            //removes http://www.
            address = address.Substring(11);
        }

        return address;
    }

    private static string[] SplitAddress(string address)
    {
        address = RemoveProtocol(address);

        return address.Split('/');
    }

    public static IFfnUrl? GetAddress(string address)
    {
        if( string.IsNullOrWhiteSpace(address) )
        {
            return null;
        }

        string[] parts = SplitAddress(address);

        if( parts.Length == 1 )
        {
            //This type of author address does not contain all the required parts.
            return new PageAddress(address);
        }

        if( parts.Length < 4 && parts[0] == "u" )
        {
            address = $"https://www.fanfiction.net{address}";
            return new AuthorAddress(address, parts[AUTHOR_NAME - 1], parts[AUTHOR_ID - 1]);
        }

        if( parts[1].IsIn("s", "u") )
        {
            if( parts[1] == "s" )
            {
                return new StoryAddress(address, parts[STORY_ID], parts[STORY_NAME], "", parts[CHAPTER_IDX]);
            }

            if( parts[1] == "u" )
            {
                return new AuthorAddress(address, parts[AUTHOR_NAME], parts[AUTHOR_ID]);
            }
        }

        return new PageAddress(address);
    }
}