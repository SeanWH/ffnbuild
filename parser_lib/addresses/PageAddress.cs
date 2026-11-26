namespace parser_lib.addresses;

using parser_lib.extensions;

public sealed class PageAddress : IFfnAddress
{
    public string Address { get; }

    public int LinkedId { get; }

    public string LinkType => "page";

    public string Site => "";
    public string Text => Address;

    public PageAddress(string address, int linkedId = -1)
    {
        Address = address;
        LinkedId = linkedId;
    }

    public static bool operator !=(PageAddress left, PageAddress right) => !(left == right);

    public static bool operator <(PageAddress left, PageAddress right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(PageAddress left, PageAddress right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator ==(PageAddress left, PageAddress right)
    {
        if( left is null )
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator >(PageAddress left, PageAddress right) => left?.CompareTo(right) > 0;

    public static bool operator >=(PageAddress left, PageAddress right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public int CompareTo(IFfnAddress? other)
    {
        int val = string.CompareOrdinal(LinkType, other?.LinkType);
        return val == 0 ? string.CompareOrdinal(Address, other?.Address) : val;
    }

    public bool Equals(IFfnAddress? other)
    {
        if( other is null )
        {
            return false;
        }

        return LinkType.Equals(other.LinkType) && Address.Equals(other.Address);
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

        if( obj is PageAddress address )
        {
            return Equals(address);
        }

        return false;
    }

    public override int GetHashCode() => Address.Hash256() ^ LinkedId.Hash256() ^ LinkType.Hash256();
}