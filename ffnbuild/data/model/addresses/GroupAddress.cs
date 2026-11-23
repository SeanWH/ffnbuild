namespace ffnbuild.data.model.addresses;

using ffnbuild.data.extensions;

public class GroupAddress : IFfnUrl
{
    public string Address { get; }

    public string Category { get; set; } = "General";

    public string GroupName { get; }

    public string LinkedId { get; }

    public string LinkTarget => "group";
    public string Site { get; }

    public string Text => IFfnUrl.FormatText(GroupName);

    public GroupAddress(string address, string linkedId, string groupName, string site)
    {
        Address = address;
        LinkedId = linkedId;
        GroupName = groupName;
        Site = site;
    }

    public GroupAddress(string address, string site, string[] parts) : this(address, parts[2], parts[1], site)
    {
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

    public int CompareTo(IFfnUrl? other)
    {
        if( ReferenceEquals(this, other) )
        {
            return 0;
        }

        if( other is GroupAddress otherGroup )
        {
            return string.CompareOrdinal(LinkedId, otherGroup.LinkedId);
        }

        return -1;
    }

    public bool Equals(IFfnUrl? other)
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

    public override int GetHashCode() => LinkTarget.StringHash256() ^ LinkedId.StringHash256();

    public override string ToString() => $"Category: {Category} Group Name: {GroupName}, Address: {Address}, Linked ID: {LinkedId}, Text: {Text}";
}