namespace ffnbuild.data.model;

using System.Collections.Generic;

public record ChapterData(string? title, List<string?> paras)
{
    public string? Title { get; set; } = title;
    public List<string?> Paragraphs { get; set; } = paras;
}