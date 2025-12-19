namespace ffnbuild;

using System.Text;

using ffnbuild.ext;

using HtmlAgilityPack;

public static class HtmlDocBuilder
{
    private static readonly string _htmlTemplate = @"<!DOCTYPE html>
<html>
<head>
    <title>Html Template String</title>
</head>
<body>
</body>
</html>";

    public static void BuildDocument(string title, SortedDictionary<string, ChapterData> story)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(_htmlTemplate);

        var rootNode = htmlDocument.DocumentNode;

        var headNode = rootNode.SelectSingleNode("//head");
        if (headNode != null)
        {
            var titleNode = headNode.SelectSingleNode("//title");
            titleNode.InnerHtml = title;
        }

        var bodyNode = rootNode.SelectSingleNode("//body");
        if (bodyNode != null)
        {
            StringBuilder bodyHtml = new StringBuilder();

            bodyHtml.AppendLine(title.Heading1());
            bodyHtml.AppendLine("".Break());

            foreach (ChapterData chapterData in story.Values)
            {
                if (chapterData is not null)
                {
                    bodyHtml.AppendLine(chapterData.Title?.AddTag("strong"));
                    bodyHtml.AppendLine("".Break());
                    foreach (string? line in chapterData.Paragraphs)
                    {
                        bodyHtml.AppendLine(line?.ToString().Trim().Paragraph());
                    }
                }
            }

            bodyNode.InnerHtml = bodyHtml.ToString();
        }

        htmlDocument.Save(GetSavePath(title));
    }

    private static string GetSavePath(string title)
    {
        var finalName = title + ".html";
        var finalPath = Path.Combine("output", finalName);

        if (!Directory.Exists("output"))
        {
            Directory.CreateDirectory("output");
        }

        return finalPath;
    }
}