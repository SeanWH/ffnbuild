namespace ffnbuild;

using System.Collections.Generic;

public class ChapterData(string title, List<string> paras)
{
    public string Title { get; set; } = title;
    public List<string> Paragraphs { get; set; } = paras;
}