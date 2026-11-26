namespace parser_lib.addresses;

using extensions;

public sealed class UserAddress : IFfnAddress
{
    public string A03Pseud { get; } = "";
    public string Address { get; }

    public int LinkedId { get; }
    public string LinkType => "user";
    public string Site { get; }
    public string Text => IFfnAddress.FormatText(UserName);
    public string UserName { get; }

    public UserAddress(string address, string userName, int linkedId, string site, string a03Pseud = "")
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

        if( int.TryParse(parts[1], out int linkedId) && "ffn".Equals(site) )
        {
            UserName = parts[2];
            LinkedId = linkedId;
        }
        else if( "a03".Equals(site) )
        {
            UserName = parts[1];
            A03Pseud = parts[3];
            LinkedId = -1;
        }
        else
        {
            throw new ArgumentException("Invalid linkedId passed to constructor.", nameof(parts));
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

    public int CompareTo(IFfnAddress? other)
    {
        if( ReferenceEquals(this, other) )
        {
            return 0;
        }

        if( other is UserAddress otherUser )
        {
            return Address.CompareTo(otherUser.Address);
        }

        return -1;
    }

    public bool Equals(IFfnAddress? other)
    {
        if( other is UserAddress otherUser )
        {
            return Address.Equals(otherUser.Address);
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

        if( obj is IFfnAddress address )
        {
            return Equals(address);
        }

        return false;
    }

    public override int GetHashCode() => LinkType.Hash256() ^ LinkedId.Hash256() ^ A03Pseud.Hash256();

    public override string ToString() => $"AuthorName: {UserName}, Address: {Address}, Linked ID: {LinkedId}, Text: {Text}";
}