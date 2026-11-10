namespace ffnparser.data.model.addresses;

using ffnparser.data.extensions;

public sealed class UserAddress : IFfnUrl
{
    public string Address { get; private set; }
    public string LinkedId { get; private set; }
    public LinkType LinkType { get; private set; }
    public string Text => IFfnUrl.FormatText(UserName);
    public string UserName { get; private set; }

    public UserAddress(string address, string linkedId, LinkType linkType, string userName)
    {
        Address = address;
        LinkedId = linkedId;
        LinkType = linkType;
        UserName = userName;
    }

    public int CompareTo(IFfnUrl? other)
    {
        if( ReferenceEquals(this, other) )
        {
            return 0;
        }

        if( other is UserAddress otherUser )
        {
            return string.CompareOrdinal(LinkedId, otherUser.LinkedId);
        }

        return -1;
    }

    public bool Equals(IFfnUrl? other)
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