namespace parser_lib.addresses;

using parser_lib.extensions;

public sealed class GroupAddress : IFfnAddress
{
    public string Address { get; }

    public string Category { get; set; } = "General";

    public string GroupName { get; }

    public int LinkedId { get; }

    public string LinkType => "group";
    public string Site { get; }

    public string Text => IFfnAddress.FormatText(GroupName);

    public GroupAddress(string address, int linkedId, string groupName, string site)
    {
        Address = address;
        LinkedId = linkedId;
        GroupName = groupName;
        Site = site;
    }

    public GroupAddress(string address, string site, string[] parts)
    {
        Address = address;
        Site = site;
        if( int.TryParse(parts[2], out int linkedId) )
        {
            LinkedId = linkedId;
        }
        else
        {
            throw new ArgumentException("Invalid linkedId passed to constructor.", nameof(parts));
        }
        GroupName = parts[1];
    }

    public static bool operator !=(GroupAddress left, GroupAddress right) => !(left == right);

    public static bool operator <(GroupAddress left, GroupAddress right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(GroupAddress left, GroupAddress right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator ==(GroupAddress left, GroupAddress right)
    {
        if( left is null )
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator >(GroupAddress left, GroupAddress right) => left?.CompareTo(right) > 0;

    public static bool operator >=(GroupAddress left, GroupAddress right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public int CompareTo(IFfnAddress? other)
    {
        if( ReferenceEquals(this, other) )
        {
            return 0;
        }

        if( other is GroupAddress otherGroup )
        {
            return LinkedId.CompareTo(otherGroup.LinkedId);
        }

        return -1;
    }

    public bool Equals(IFfnAddress? other)
    {
        if( other is GroupAddress otherGroup )
        {
            return LinkedId.Equals(otherGroup.LinkedId);
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

        if( obj is GroupAddress groupAddress )
        {
            return Equals(groupAddress);
        }

        return false;
    }

    public override int GetHashCode() => LinkType.Hash256() ^ LinkedId.Hash256();

    public override string ToString() => $"Category: {Category} Group Name: {GroupName}, Address: {Address}, Linked ID: {LinkedId}, Text: {Text}";
}