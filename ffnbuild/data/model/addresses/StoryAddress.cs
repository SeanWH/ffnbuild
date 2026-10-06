namespace ffnbuild.data.model.addresses;

using ffnbuild.data.extensions;

public sealed class StoryAddress : IFfnUrl
{
    public string Address { get; }

    public string ChapterIndex { get; }

    public string ChapterTitle { get; }

    public string LinkedId { get; }

    public string LinkTarget => "story";

    public string StoryTitle { get; }

    public string Text => IFfnUrl.FormatText(StoryTitle);

    public StoryAddress(string address, string linkedId, string storyTitle, string chapterTitle, string chapterIndex)
    {
        Address = address;
        LinkedId = linkedId;
        StoryTitle = storyTitle;
        ChapterTitle = chapterTitle;
        ChapterIndex = chapterIndex;
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

    public int CompareTo(IFfnUrl? other)
    {
        if( ReferenceEquals(this, other) )
        {
            return 0;
        }

        if( other is StoryAddress otherStory )
        {
            return string.CompareOrdinal(LinkedId, otherStory.LinkedId);
        }

        return -1;
    }

    public bool Equals(IFfnUrl? other)
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
        LinkedId.StringHash256() ^
        LinkTarget.StringHash256();

    public override string ToString() =>
        $"StoryTitle: {StoryTitle}, ChapterTitle: {ChapterTitle}, ChapterIndex: {ChapterIndex}, Address: {Address}, Linked ID: {LinkedId}, Text: {Text}";
}