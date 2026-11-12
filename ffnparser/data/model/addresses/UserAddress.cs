namespace ffnparser.data.model.addresses;

using ffnparser.data.extensions;

public sealed class UserAddress : IFfnAddress
{
    public string Address { get; private set; }
    public int LinkedId { get; private set; }
    public string LinkType => "user";
    public string Text => IFfnAddress.FormatText(UserName);
    public string UserName { get; private set; }

    public UserAddress(string address, int linkedId, string userName)
    {
        Address = address;
        LinkedId = linkedId;

        UserName = userName;
    }

    public static bool operator !=(UserAddress? left, UserAddress? right)
    {
        return !(left == right);
    }

    public static bool operator <(UserAddress? left, UserAddress? right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(UserAddress? left, UserAddress? right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator ==(UserAddress? left, UserAddress? right)
    {
        if( ReferenceEquals(left, null) )
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator >(UserAddress? left, UserAddress? right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(UserAddress? left, UserAddress? right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }

    public int CompareTo(IFfnAddress? other)
    {
        if( ReferenceEquals(this, other) )
        {
            return 0;
        }

        if( other is UserAddress otherUser )
        {
            return LinkedId.CompareTo(otherUser.LinkedId);
        }

        return -1;
    }

    public bool Equals(IFfnAddress? other)
    {
        if( other is UserAddress otherUser )
        {
            return LinkedId == otherUser.LinkedId;
        }

        return false;
    }

    public override bool Equals(object? obj)
    {
        if( ReferenceEquals(this, obj) )
        {
            return true;
        }

        if( obj is UserAddress otherUser )
        {
            return Equals(otherUser);
        }

        return false;
    }

    public override int GetHashCode() => LinkedId.Hash256() ^ LinkType.Hash256();

    public override string ToString()
    {
        return $"User Name: {UserName}, Address: {Address}, Linked ID: {LinkedId}, Text: {Text}";
    }
}