namespace ffnparser.data.model.addresses;

using ffnparser.data.extensions;

public static class FfnAddressFactory
{
    private const int AUTHOR_ID = 2;
    private const int AUTHOR_NAME = 3;
    private const int CHAPTER_IDX = 3;
    private const int STORY_ID = 2;
    private const int STORY_NAME = 4;

    private static string RemoveProtocol(string address)
    {
        if (address.StartsWith("https://"))
        {
            //removes https://www.
            //Used to be 12
            address = address.Substring(8);
        }

        if (address.StartsWith("http://"))
        {
            //removes http://www.
            //Used to be 11
            address = address.Substring(7);
        }

        return address;
    }

    private static string[] SplitAddress(string address)
    {
        address = RemoveProtocol(address);

        return address.Split('/');
    }

    public static IFfnAddress? GetAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return null;
        }

        string[] parts = SplitAddress(address);

        if (parts.Length == 1)
        {
            //This type of author address does not contain all the required parts.
            return new PageAddress(address);
        }

        if (parts.Length < 4 && parts[0] == "u")
        {
            address = $"https://www.fanfiction.net/{address}";
            var linkedId = Convert.ToInt32(parts[AUTHOR_NAME - 2]);
            var userName = parts[AUTHOR_NAME - 1];
            return new UserAddress(address, linkedId, userName);
        }

        if (parts[1].IsIn("s", "u"))
        {
            if (parts[1] == "s")
            {
                var storyId = Convert.ToInt32(parts[STORY_ID]);
                return new StoryAddress(address, storyId, parts[STORY_NAME], parts[CHAPTER_IDX], "");
            }

            if (parts[1] == "u")
            {
                if (string.IsNullOrWhiteSpace(parts[0]))
                {
                    return new PageAddress(address);
                }
                var linkedId = Convert.ToInt32(parts[AUTHOR_NAME - 1]);
                return new UserAddress(address, linkedId, parts[AUTHOR_NAME]);
            }
        }

        return new PageAddress(address);
    }
}