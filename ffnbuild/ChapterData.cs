namespace ffnbuild;

using System;
using System.Collections.Generic;
using System.Text;

public class ChapterData
{
    public string Title { get; set; } = string.Empty;
    public List<string> Paragraphs { get; set; } = new List<string>();

    public ChapterData(string title, List<string> paras)
    {
        Title = title;
        Paragraphs = paras;
    }
}
