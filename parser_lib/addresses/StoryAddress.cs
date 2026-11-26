namespace parser_lib.addresses;

using parser_lib.extensions;

public sealed class StoryAddress : IFfnAddress
{
    public string Address { get; }

    public string ChapterIndex { get; }

    public string ChapterTitle { get; }

    public int LinkedId { get; }

    public string LinkType => "story";

    public string Site { get; }
    public string StoryTitle { get; }
    public string Text => IFfnAddress.FormatText(StoryTitle);

    public StoryAddress(string address, int linkedId, string storyTitle, string chapterTitle, string chapterIndex, string site)
    {
        Address = address;
        LinkedId = linkedId;
        StoryTitle = storyTitle;
        ChapterTitle = chapterTitle;
        ChapterIndex = chapterIndex;
        Site = site;
    }

    public StoryAddress(string address, string site, string[] parts)
    {
        Address = address;
        Site = site;

        if( "works".Equals(parts[0]) )
        {
            ChapterIndex = "";
        }
        else
        {
            ChapterIndex = parts[2];
        }

        if( int.TryParse(parts[1], out int linkedId) )
        {
            LinkedId = linkedId;
        }
        else
        {
            throw new ArgumentException("Invalid linkedId passed to constructor.", nameof(parts));
        }

        StoryTitle = parts[3];
        ChapterTitle = "";
    }

    public static bool operator !=(StoryAddress left, StoryAddress right) => !(left == right);

    public static bool operator <(StoryAddress left, StoryAddress right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(StoryAddress left, StoryAddress right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator ==(StoryAddress left, StoryAddress right)
    {
        if( left is null )
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator >(StoryAddress left, StoryAddress right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(StoryAddress left, StoryAddress right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public int CompareTo(IFfnAddress? other)
    {
        if( ReferenceEquals(this, other) )
        {
            return 0;
        }

        if( other is StoryAddress otherStory )
        {
            return LinkedId.CompareTo(otherStory.LinkedId);
        }

        return -1;
    }

    public bool Equals(IFfnAddress? other)
    {
        if( other is StoryAddress otherStory )
        {
            return LinkedId.Equals(otherStory.LinkedId);
        }

        return false;
    }

    public override bool Equals(object? obj)
    {
        if( ReferenceEquals(this, obj) )
        {
            return true;
        }

        if( obj is null )
        {
            return false;
        }

        if( obj is StoryAddress otherAddress )
        {
            return Equals(otherAddress);
        }

        return false;
    }

    public override int GetHashCode() =>
        LinkedId.Hash256() ^
        LinkType.Hash256();

    public override string ToString() =>
        $"StoryTitle: {StoryTitle}, ChapterTitle: {ChapterTitle}, ChapterIndex: {ChapterIndex}, Address: {Address}, Linked ID: {LinkedId}, Text: {Text}";
}