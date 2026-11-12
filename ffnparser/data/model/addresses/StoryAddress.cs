namespace ffnparser.data.model.addresses;

public sealed class StoryAddress : IFfnAddress
{
    public string Address { get; private set; }
    public string ChapterId { get; private set; }
    public string ChapterTitle { get; private set; }
    public int LinkedId { get; private set; }
    public string LinkType => "story";
    public string StoryTitle { get; private set; }
    public string Text => IFfnAddress.FormatText(StoryTitle);

    public StoryAddress(string address, int linkedId, string storyTitle, string chapterId, string chapterTitle)
    {
        Address = address;
        LinkedId = linkedId;

        StoryTitle = storyTitle;
        ChapterId = chapterId;
        ChapterTitle = chapterTitle;
    }

    public static bool operator !=(StoryAddress left, StoryAddress right)
    {
        return !(left == right);
    }

    public static bool operator <(StoryAddress left, StoryAddress right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(StoryAddress left, StoryAddress right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator ==(StoryAddress left, StoryAddress right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator >(StoryAddress left, StoryAddress right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(StoryAddress left, StoryAddress right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }

    public int CompareTo(IFfnAddress? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }
        if (other is StoryAddress otherStory)
        {
            return LinkedId.CompareTo(otherStory.LinkedId);
        }
        return 1;
    }

    public bool Equals(IFfnAddress? other)
    {
        if (other is StoryAddress otherStory)
        {
            return LinkedId == otherStory.LinkedId;
        }
        return false;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj is StoryAddress otherStory)
        {
            return Equals(otherStory);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return LinkedId.GetHashCode() ^ LinkType.GetHashCode();
    }

    public override string ToString() =>
             $"StoryTitle: {StoryTitle}, ChapterTitle: {ChapterTitle}, ChapterId: {ChapterId}, Address: {Address}, Linked ID: {LinkedId}, Text: {Text}";
}