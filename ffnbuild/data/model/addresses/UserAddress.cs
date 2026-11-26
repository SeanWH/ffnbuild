namespace ffnbuild.data.model.addresses;

using extensions;

public sealed class UserAddress : IFfnUrl
{
    public string A03Pseud { get; } = "";
    public string Address { get; }

    public string LinkedId { get; }
    public string LinkTarget => "author";
    public string Site { get; }
    public string Text => IFfnUrl.FormatText(UserName);
    public string UserName { get; }

    public UserAddress(string address, string userName, string linkedId, string site, string a03Pseud = "")
    {
        Address = address;
        UserName = userName;
        LinkedId = linkedId;
        Site = site;
        A03Pseud = a03Pseud;
    }

    public UserAddress(string address, string site, string[] parts)
    {
        Address = address;
        Site = site;
        switch( site )
        {
            case "a03":
                UserName = parts[1];
                A03Pseud = parts[3];
                LinkedId = "";
                break;

            default:
                //FFN User
                UserName = parts[2];
                LinkedId = parts[1];
                break;
        }
    }

    public static bool operator !=(UserAddress left, UserAddress right) => !(left == right);

    public static bool operator <(UserAddress left, UserAddress right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(UserAddress left, UserAddress right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator ==(UserAddress left, UserAddress right)
    {
        if( left is null )
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator >(UserAddress left, UserAddress right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(UserAddress left, UserAddress right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public int CompareTo(IFfnUrl? other)
    {
        if( ReferenceEquals(this, other) )
        {
            return 0;
        }

        if( other is UserAddress otherAuthor )
        {
            return string.CompareOrdinal(LinkedId, otherAuthor.LinkedId);
        }

        return -1;
    }

    public bool Equals(IFfnUrl? other)
    {
        if( other is UserAddress otherAuthor )
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

    public override string ToString() => $"AuthorName: {UserName}, Address: {Address}, Linked ID: {LinkedId}, Text: {Text}";
}