namespace ffnparser.data.model.addresses;

using ffnparser.data.extensions;

public sealed class PageAddress : IFfnAddress
{
    public string Address { get; private set; }
    public int LinkedId { get; private set; }
    public string LinkType => "page";
    public string Text => Address;

    public PageAddress(string address, int linkedId = 0)
    {
        Address = address;
        LinkedId = linkedId;
    }

    public static bool operator !=(PageAddress left, PageAddress right)
    {
        return !(left == right);
    }

    public static bool operator <(PageAddress left, PageAddress right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(PageAddress left, PageAddress right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator ==(PageAddress left, PageAddress right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator >(PageAddress left, PageAddress right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(PageAddress left, PageAddress right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }

    public int CompareTo(IFfnAddress? other)
    {
        if (other is null)
        {
            return 1;
        }

        int val = string.CompareOrdinal(LinkType, other.LinkType);
        return val == 0 ? string.CompareOrdinal(Address, other.Address) : val;
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

        if (obj is PageAddress pageAddress)
        {
            return Equals(pageAddress);
        }

        return false;
    }

    public bool Equals(IFfnAddress? other)
    {
        if (other is null)
        {
            return false;
        }

        return LinkType.Equals(other.LinkType) && Address.Equals(other.Address) && LinkedId.Equals(other.LinkedId);
    }

    public override int GetHashCode()
    {
        var addrHash = Address.Hash256();
        var linkedIdHash = LinkedId.Hash256();
        var linkTypeHash = LinkType.Hash256();

        var hashCode = addrHash ^ linkedIdHash ^ linkTypeHash;

        return hashCode;
    }
}