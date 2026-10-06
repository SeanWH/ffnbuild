namespace ffnbuild.data.model.addresses;

using extensions;

public sealed class AuthorAddress : IFfnUrl
{
    public string Address { get; }

    public string AuthorName { get; }

    public string LinkedId { get; }

    public string LinkTarget => "author";

    public string Text => IFfnUrl.FormatText(AuthorName);

    public AuthorAddress(string address, string authorName, string linkedId)
    {
        Address = address;
        AuthorName = authorName;
        LinkedId = linkedId;
    }

    public static bool operator !=(AuthorAddress left, AuthorAddress right) => !(left == right);

    public static bool operator <(AuthorAddress left, AuthorAddress right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(AuthorAddress left, AuthorAddress right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator ==(AuthorAddress left, AuthorAddress right)
    {
        if( left is null )
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator >(AuthorAddress left, AuthorAddress right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(AuthorAddress left, AuthorAddress right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public int CompareTo(IFfnUrl? other)
    {
        if( ReferenceEquals(this, other) )
        {
            return 0;
        }

        if( other is AuthorAddress otherAuthor )
        {
            return string.CompareOrdinal(LinkedId, otherAuthor.LinkedId);
        }

        return -1;
    }

    public bool Equals(IFfnUrl? other)
    {
        if( other is AuthorAddress otherAuthor )
        {
            return LinkedId.Equals(otherAuthor.LinkedId);
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

        if( obj is IFfnUrl address )
        {
            return Equals(address);
        }

        return false;
    }

    public override int GetHashCode() => LinkTarget.StringHash256() ^ LinkedId.StringHash256();

    public override string ToString() => $"AuthorName: {AuthorName}, Address: {Address}, Linked ID: {LinkedId}, Text: {Text}";
}