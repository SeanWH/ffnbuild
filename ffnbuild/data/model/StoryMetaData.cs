namespace ffnbuild.data.model;

public record StoryMetaData
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string StoryUrl { get; set; } = string.Empty;
    public string AuthorUrl { get; set; } = string.Empty;
}