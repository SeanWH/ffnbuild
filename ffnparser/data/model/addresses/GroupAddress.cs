namespace ffnparser.data.model.addresses;

using ffnparser.data.extensions;

public sealed class GroupAddress : IFfnAddress
{
    public string Address { get; private set; }
    public string GroupName { get; private set; }
    public int LinkedId { get; private set; }
    public string LinkType => "group";

    public string Text => IFfnAddress.FormatText(GroupName);

    public GroupAddress(string address, string groupName, int linkedId)
    {
        Address = address;
        GroupName = groupName;
        LinkedId = linkedId;
    }

    public static bool operator !=(GroupAddress left, GroupAddress right) => !(left == right);

    public static bool operator <(GroupAddress left, GroupAddress right) => ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;

    public static bool operator <=(GroupAddress left, GroupAddress right) => ReferenceEquals(left, null) || left.CompareTo(right) <= 0;

    public static bool operator ==(GroupAddress left, GroupAddress right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator >(GroupAddress left, GroupAddress right) => !ReferenceEquals(left, null) && left.CompareTo(right) > 0;

    public static bool operator >=(GroupAddress left, GroupAddress right) => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;

    public int CompareTo(IFfnAddress? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is GroupAddress otherGroup)
        {
            int val = string.CompareOrdinal(GroupName, otherGroup.GroupName);
            val = val == 0 ? string.CompareOrdinal(Address, otherGroup.Address) : val;
            return val == 0 ? LinkedId.CompareTo(otherGroup.LinkedId) : val;
        }

        return string.CompareOrdinal(LinkType, other.LinkType);
    }

    public bool Equals(IFfnAddress? other)
    {
        if (other is GroupAddress otherGroup)
        {
            return GroupName.Equals(otherGroup.GroupName) && Address.Equals(otherGroup.Address) && LinkedId.Equals(otherGroup.LinkedId);
        }

        return false;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (ReferenceEquals(obj, null))
        {
            return false;
        }

        if (obj is GroupAddress address)
        {
            return Equals(address);
        }

        return false;
    }

    public override int GetHashCode()
    {
        var gNameHash = GroupName.Hash256();
        var addrHash = Address.Hash256();
        var linkTypeHash = LinkType.Hash256();
        var linkedIdHash = LinkedId.Hash256();

        var hashCode = gNameHash ^ addrHash ^ linkTypeHash ^ linkedIdHash;

        return hashCode;
    }
}