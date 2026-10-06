namespace ffnbuild.data.model.addresses;

using System;

using extensions;

public interface IFfnUrl : IComparable<IFfnUrl>, IEquatable<IFfnUrl>
{
    string Address { get; }
    string LinkedId { get; }
    string LinkTarget { get; }
    string Text { get; }

    public static string FormatText(string textToFormat)
    {
        textToFormat = textToFormat.Replace("-", " ");
        textToFormat = textToFormat.TranslateHtmlCode();

        return textToFormat;
    }

    virtual int GetHashCode()
    {
        return Address.StringHash256() ^ LinkedId.StringHash256() ^ LinkTarget.StringHash256();
    }

    virtual string ToString()
    {
        return $"Address: {Address}, LinkedId: {LinkedId}, LinkTarget: {LinkTarget}, Text: {Text}";
    }
}