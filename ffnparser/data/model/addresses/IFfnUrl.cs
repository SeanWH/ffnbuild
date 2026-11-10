namespace ffnparser.data.model.addresses;

using System;

using ffnparser.data.extensions;

public interface IFfnUrl : IComparable<IFfnUrl>, IEquatable<IFfnUrl>
{
    string Address { get; }
    string LinkedId { get; }
    LinkType LinkType { get; }
    string Text { get; }

    public static string FormatText(string? text)
    {
        if( string.IsNullOrWhiteSpace(text) )
        {
            return string.Empty;
        }

        text = text.Replace("-", " ");
        text = text.Replace("_", " ");
        text = text.TranslateHtmlCode();
        return text;
    }

    abstract int GetHashCode();

    abstract string? ToString();
}