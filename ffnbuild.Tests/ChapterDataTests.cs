namespace ffnbuild.Tests;

using Xunit;

public class ChapterDataTests
{
    [Fact]
    public void ChapterData_Initialization_WorksCorrectly()
    {
        var title = "Sample Title";
        var paragraphs = new List<string?> { "Paragraph 1", "Paragraph 2" };
        var chapterData = new ChapterData(title, paragraphs);
        Assert.Equal(title, chapterData.Title);
        Assert.Equal(paragraphs, chapterData.Paragraphs);
    }

    [Fact]
    public void ChapterData_Setters_WorksCorrectly()
    {
        var chapterData = new ChapterData("Initial Title", [])
        {
            Title = "Updated Title",
            Paragraphs = ["New Paragraph"]
        };
        Assert.Equal("Updated Title", chapterData.Title);
        Assert.Equal(["New Paragraph"], chapterData.Paragraphs);
    }

    [Fact]
    public void ChapterData_EmptyParagraphs_WorksCorrectly()
    {
        var chapterData = new ChapterData("Title with No Paragraphs", []);
        Assert.Equal("Title with No Paragraphs", chapterData.Title);
        Assert.Empty(chapterData.Paragraphs);
    }

    [Fact]
    public void ChapterData_LongTitle_WorksCorrectly()
    {
        var longTitle = new string('A', 1000);
        var chapterData = new ChapterData(longTitle, []);
        Assert.Equal(longTitle, chapterData.Title);
    }

    [Fact]
    public void ChapterData_MultipleParagraphs_WorksCorrectly()
    {
        var paragraphs = new List<string?>
        {
            "First paragraph.",
            "Second paragraph.",
            "Third paragraph."
        };
        var chapterData = new ChapterData("Multi-Paragraph Title", paragraphs);
        Assert.Equal(paragraphs, chapterData.Paragraphs);
    }

    [Fact]
    public void ChapterData_WhitespaceTitle_WorksCorrectly()
    {
        var chapterData = new ChapterData("   ", []);
        Assert.Equal("   ", chapterData.Title);
    }

    [Fact]
    public void ChapterData_SpecialCharactersInTitle_WorksCorrectly()
    {
        var title = "Title! @# $%^ &*()";
        var chapterData = new ChapterData(title, []);
        Assert.Equal(title, chapterData.Title);
    }

    [Fact]
    public void ChapterData_ParagraphsWithSpecialCharacters_WorksCorrectly()
    {
        var paragraphs = new List<string?>
        {
            "Paragraph with special characters! @# $%^ &*()",
            "Another paragraph with numbers 12345"
        };
        var chapterData = new ChapterData("Title", paragraphs);
        Assert.Equal(paragraphs, chapterData.Paragraphs);
    }
}