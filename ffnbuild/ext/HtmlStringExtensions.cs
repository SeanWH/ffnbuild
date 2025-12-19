namespace ffnbuild.ext;

public static class HtmlStringExtensions
{
    public static string AddTag(this string target, string? tag)
    {
        string[] emptyElements = {"area","base","br","col","embed","hr","img","input","link",
            "meta","param","source","track","wbr" };

        if (string.IsNullOrWhiteSpace(tag))
        {
            return target;
        }

        string? finalTag = tag;
        if (tag.StartsWith('<') || tag.EndsWith('>'))
        {
            finalTag = finalTag.Replace("<", "");
            finalTag = finalTag.Replace(">", "");
        }

        if (finalTag.IsIn(emptyElements))
        {
            return $"<{finalTag}>{target}";
        }

        return $"<{finalTag}>{target}</{finalTag}>";
    }

    public static string Bold(this string target)
    {
        return target.AddTag("b");
    }

    public static string Italic(this string target)
    {
        return target.AddTag("i");
    }

    public static string Underline(this string target)
    {
        return target.AddTag("u");
    }

    public static string Heading1(this string target)
    {
        return target.AddTag("h1");
    }

    public static string Heading2(this string target)
    {
        return target.AddTag("h2");
    }

    public static string Heading3(this string target)
    {
        return target.AddTag("h3");
    }

    public static string Heading4(this string target)
    {
        return target.AddTag("h4");
    }

    public static string Heading5(this string target)
    {
        return target.AddTag("h5");
    }

    public static string Heading6(this string target)
    {
        return target.AddTag("h6");
    }

    public static string Paragraph(this string target)
    {
        return target.AddTag("p");
    }

    public static string Break(this string target)
    {
        return target.AddTag("br");
    }
}