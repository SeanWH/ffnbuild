namespace parser_lib.addresses;

using System;

using extensions;

public interface IFfnAddress : IComparable<IFfnAddress>, IEquatable<IFfnAddress>
{
    string Address { get; }
    int LinkedId { get; }
    string LinkType { get; }
    string Site { get; }
    string Text { get; }

    public static string FormatText(string? textToFormat)
    {
        if( textToFormat is not null )
        {
            textToFormat = textToFormat.Replace("-", " ");
            textToFormat = textToFormat.Replace("_", " ");
            textToFormat = textToFormat.TranslateHtmlCode();
            textToFormat = textToFormat.Trim();
        }
        else
        {
            textToFormat = string.Empty;
        }

        return textToFormat;
    }

    virtual int GetHashCode()
    {
        return Address.Hash256() ^ LinkedId.Hash256() ^ LinkType.Hash256();
    }

    virtual string? ToString()
    {
        return $"Address: {Address}, LinkedId: {LinkedId}, LinkTarget: {LinkType}, Text: {Text}";
    }
}