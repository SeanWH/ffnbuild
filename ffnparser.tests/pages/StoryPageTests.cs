namespace ffnparser.tests.pages;

using ffnparser.data.model.pages;

using Xunit;

public class StoryPageTests
{
    [Fact]
    public void Constructor_WhenMissingAuthorLink_ReturnsNullStoryData()
    {
        // Arrange
        const string html = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net/s/12345/A-Test-Story" />
                </head>
                <body>
                    <div id="profile_top">
                        <span>No author anchor tag present</span>
                    </div>
                    <div class="storytext">
                        <p>Some text.</p>
                    </div>
                </body>
            </html>
            """;

        // Act
        var page = new StoryPage(html);

        // Assert
        Assert.Null(page.StoryData);
    }

    [Fact]
    public void Constructor_WhenMissingChapterSelectDropdown_DefaultsChapterTitleToEmpty()
    {
        // Arrange (One-shot stories often omit the chap_select dropdown)
        const string html = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net/s/12345/A-Test-Story" />
                </head>
                <body>
                    <div id="pre_story_links">
                        <a href="/game/">Games</a>
                        <a href="/game/Zelda/">Zelda</a>
                    </div>
                    <div id="profile_top">
                        <a href="/u/334455/OneShotWriter">OneShotWriter</a>
                    </div>
                    <div class="storytext">
                        <p>Single chapter content.</p>
                    </div>
                </body>
            </html>
            """;

        // Act
        var page = new StoryPage(html);

        // Assert
        Assert.NotNull(page.StoryData);
        var chapter = page.StoryData.Chapters[0];
        Assert.Equal(string.Empty, chapter.ChapterTitle);
        Assert.Single(chapter.Paragraphs);
    }

    [Fact]
    public void Constructor_WhenMissingPreStoryLinks_DefaultsMediaTypeAndGenreToEmpty()
    {
        // Arrange
        const string html = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net/s/12345/A-Test-Story" />
                </head>
                <body>
                    <div id="profile_top">
                        <a href="/u/998877/Author">Author</a>
                    </div>
                    <div class="storytext">
                        <p>Text content.</p>
                    </div>
                </body>
            </html>
            """;

        // Act
        var page = new StoryPage(html);

        // Assert
        Assert.NotNull(page.StoryData);
        Assert.Equal(string.Empty, page.StoryData.MediaType);
        Assert.Equal(string.Empty, page.StoryData.Genre);
    }

    [Fact]
    public void Constructor_WithCrossoverStoryLinks_SetsMediaTypeToCrossover()
    {
        // Arrange Only 1 <a> tag inside pre_story_links triggers the
        // "crossover" branch
        const string html = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net/s/12345/A-Test-Story" />
                </head>
                <body>
                    <div id="pre_story_links">
                        <a href="/crossovers/Star-Wars-and-Marvel/">Star Wars & Marvel</a>
                    </div>
                    <div id="profile_top">
                        <a href="/u/112233/CrossoverWriter">CrossoverWriter</a>
                    </div>
                    <select id="chap_select">
                        <option value="1" selected="">1. Opening</option>
                    </select>
                    <div class="storytext">
                        <p>A long time ago in New York.</p>
                    </div>
                </body>
            </html>
            """;

        // Act
        var page = new StoryPage(html);

        // Assert
        Assert.NotNull(page.StoryData);
        Assert.Equal("crossover", page.StoryData.MediaType);
        Assert.Equal("Star Wars & Marvel", page.StoryData.Genre);
    }

    [Theory]
    [InlineData("")]
    [InlineData("<html><head></head><body><div>Not a story page</div></body></html>")]
    [InlineData("<html><head><link rel='canonical' href='https://fanfiction.net/s/12345/A-Test-Story' /></head><body></body></html>")]
    public void Constructor_WithInvalidOrNonStoryHtml_CatchesExceptionAndReturnsNullStoryData(string html)
    {
        // Act
        var page = new StoryPage(html);

        // Assert
        Assert.Null(page.StoryData);
    }

    [Fact]
    public void Constructor_WithOldStyleBrSeparatedText_CleansQuotesAndLineBreaks()
    {
        // Arrange
        const string html = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net/s/12345/A-Test-Story" />
                </head>
                <body>
                    <div id="pre_story_links">
                        <a href="/anime/">Anime/Manga</a>
                        <a href="/anime/Naruto/">Naruto</a>
                    </div>
                    <div id="profile_top">
                        <a href="/u/445566/RetroAuthor">RetroAuthor</a>
                    </div>
                    <select id="chap_select">
                        <option value="1" selected="">1. Legacy Format</option>
                    </select>
                    <div class="storytext">
                        "First block with outer quotes
                        and an internal newline."<br><br>
                        Second block without quotes.<br><br>

                    </div>
                </body>
            </html>
            """;

        // Act
        var page = new StoryPage(html);

        // Assert
        Assert.NotNull(page.StoryData);
        var chapter = page.StoryData.Chapters[0];
        Assert.Equal(2, chapter.Paragraphs.Count);
        Assert.Equal("First block with outer quotes and an internal newline.", chapter.Paragraphs[0]);
        Assert.Equal("Second block without quotes.", chapter.Paragraphs[1]);
    }

    [Fact]
    public void Constructor_WithValidStandardStoryHtml_ParsesAllStoryAndChapterData()
    {
        // Arrange
        const string html = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net/s/12345/2/A-Test-Story" />
                </head>
                <body>
                    <div id="pre_story_links">
                        <a href="/book/">Books</a>
                        <a href="/book/Harry-Potter/">Harry Potter</a>
                    </div>
                    <div id="profile_top">
                        <a href="/u/987654/AuthorName">AuthorName</a>
                    </div>
                    <select id="chap_select">
                        <option value="1">1. Prologue</option>
                        <option value="2" selected="">2. The Call to Adventure</option>
                    </select>
                    <div class="storytext xcontrast_txt" id="storytext">
                        <p>First paragraph of the chapter.</p>
                        <p>Second paragraph with more details.</p>
                    </div>
                </body>
            </html>
            """;

        // Act
        var page = new StoryPage(html);

        // Assert
        Assert.NotNull(page.StoryData);
        Assert.Equal("A-Test-Story", page.StoryData.StoryTitle);
        Assert.Equal("Books", page.StoryData.MediaType);
        Assert.Equal("Harry Potter", page.StoryData.Genre);
        Assert.NotNull(page.StoryData.StoryAddress);
        Assert.NotNull(page.StoryData.AuthorAddress);

        // Verify Chapter 2 data
        Assert.True(page.StoryData.Chapters.ContainsKey(2));
        var chapter = page.StoryData.Chapters[2];
        Assert.Equal(2, chapter.ChapterId);
        Assert.Equal("The Call to Adventure", chapter.ChapterTitle);
        Assert.Equal("12345", chapter.StoryFanfictionNetId);
        Assert.Equal(
            ["First paragraph of the chapter.", "Second paragraph with more details."],
            chapter.Paragraphs);
    }
}