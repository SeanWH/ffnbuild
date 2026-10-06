namespace ffnbuild.data.model.addresses;

using ffnbuild.data.extensions;

public sealed class PageAddress : IFfnUrl
{
    public string Address { get; }

    public string LinkedId { get; }

    public string LinkTarget => "ffnpage";

    public string Text => Address;

    public PageAddress(string address, string linkedId = "")
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

    public static bool operator >(PageAddress left, PageAddress right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(PageAddress left, PageAddress right) => left is null ? right is null : left.CompareTo(right) >= 0;

    public int CompareTo(IFfnUrl? other)
    {
        int val = string.CompareOrdinal(LinkTarget, other?.LinkTarget);
        val = val == 0 ? string.CompareOrdinal(Address, other?.Address) : val;
        return val;
    }

    public bool Equals(IFfnUrl? other)
    {
        if( other is null )
        {
            return false;
        }

        return LinkTarget.Equals(other.LinkTarget) && Address.Equals(other.Address);
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

    public override int GetHashCode() => Address.StringHash256() ^ LinkedId.StringHash256() ^ LinkTarget.StringHash256();
}