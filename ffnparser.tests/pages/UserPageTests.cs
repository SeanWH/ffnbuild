namespace ffnparser.tests.pages;

using ffnparser.data.model.pages;

using Xunit;

public class UserPageTests
{
    private string LoadTestFile(string fileName)
    {
        var filePath = Path.Combine("test_files", fileName);
        return File.ReadAllText(filePath);
    }

    [Fact]
    public void Constructor_EmptyHtml_LeavesAllCollectionsEmpty()
    {
        // Hits early return branches for UserStories, FavoriteStories, and FavoriteAuthors
        var sut = new UserPage(string.Empty);

        Assert.Empty(sut.UserStories);
        Assert.Empty(sut.FavoriteStories);
        Assert.Empty(sut.FavoriteAuthors);
    }

    [Fact]
    public void Constructor_WhenHtmlIsNull_DoesNotThrowAndListsAreEmpty()
    {
        // Arrange & Act
        var page = new UserPage(null);

        // Assert
        Assert.NotNull(page);
        Assert.Empty(page.UserStories);
        Assert.Empty(page.FavoriteStories);
        Assert.Empty(page.FavoriteAuthors);
    }

    [Fact]
    public void Constructor_FavoriteAuthorsDivWithNoUserLinks_LeavesFavoriteAuthorsEmpty()
    {
        // Hits: if (authorNodes == null) return; when div#fa exists but has no
        // /u/ anchors
        string html = LoadTestFile("UserPageTestFileNoFavAuthors.html");

        var sut = new UserPage(html);

        Assert.Empty(sut.FavoriteAuthors);
    }

    [Fact]
    public void Constructor_FavoriteAuthorsSectionWithValidAndInvalidLinks_PopulatesOnlyUserAddresses()
    {
        string html = LoadTestFile("UserPageTestFile.html");

        var sut = new UserPage(html);

        Assert.Equal(143, sut.FavoriteAuthors.Count);
        Assert.Equal(4713765, sut.FavoriteAuthors[0].LinkedId);
        Assert.Equal(5180580, sut.FavoriteAuthors[1].LinkedId);
    }

    [Fact]
    public void Constructor_HtmlWithoutExpectedDivs_LeavesAllCollectionsEmpty()
    {
        string html = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net" />
                </head>
                <body>
                    <div class="unrelated">Nothing here</div>
                </body>
            </html>
            """;

        var sut = new UserPage(html);

        Assert.Empty(sut.UserStories);
        Assert.Empty(sut.FavoriteStories);
        Assert.Empty(sut.FavoriteAuthors);
    }

    [Fact]
    public void Constructor_StoryNodesWithInvalidCanonicalOrStoryAddress_SkipsNonMatchingTypes()
    {
        // Hits: if (storyAddress is StoryAddress sAddress && authorAddress is
        // UserAddress aAddress) -> false branches
        string htmlWithInvalidCanonical = """
            <html>
                <head>
                    <!-- Canonical URL is a story instead of a user -> authorAddress is not UserAddress -->
                    <link rel="canonical" href="https://fanfiction.net" />
                </head>
                <body>
                    <div class="z-list mystories" data-title="Valid Story">
                        <a class="stitle" href="/s/11111/1/Valid-Story">Valid Story</a>
                    </div>
                </body>
            </html>
            """;

        string htmlWithInvalidStoryHref = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net" />
                </head>
                <body>
                    <!-- Link points to a user (/u/) instead of a story (/s/) -> storyAddress is not StoryAddress -->
                    <div class="z-list mystories" data-title="Not A Story">
                        <a class="stitle" href="/u/88888/Not-A-Story">Not A Story</a>
                    </div>
                </body>
            </html>
            """;

        var sutInvalidAuthor = new UserPage(htmlWithInvalidCanonical);
        var sutInvalidStory = new UserPage(htmlWithInvalidStoryHref);

        Assert.Empty(sutInvalidAuthor.UserStories);
        Assert.Empty(sutInvalidStory.UserStories);
    }

    [Fact]
    public void Constructor_StoryNodesWithMissingUrlOrTitle_SkipsInvalidNodes()
    {
        // Hits: if (string.IsNullOrEmpty(storyUrl) ||
        // string.IsNullOrEmpty(storyTitle)) continue;
        string html = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net" />
                </head>
                <body>
                    <!-- Missing data-title -->
                    <div class="z-list mystories">
                        <a class="stitle" href="/s/11111/1/No-Title">No Title</a>
                    </div>
                    <!-- Empty data-title -->
                    <div class="z-list mystories" data-title="">
                        <a class="stitle" href="/s/22222/1/Empty-Title">Empty Title</a>
                    </div>
                    <!-- Missing stitle link -->
                    <div class="z-list mystories" data-title="No Link">
                        <a class="other" href="/s/33333/1/No-Link">No Link</a>
                    </div>
                    <!-- Empty href on stitle -->
                    <div class="z-list mystories" data-title="Empty Href">
                        <a class="stitle" href="">Empty Href</a>
                    </div>
                </body>
            </html>
            """;

        var sut = new UserPage(html);

        Assert.Empty(sut.UserStories);
    }

    [Fact]
    public void Constructor_ValidUserStoriesAndFavoriteStories_PopulatesBothLists()
    {
        string html = LoadTestFile("UserPageTestFile.html");

        var sut = new UserPage(html);

        Assert.Single(sut.UserStories);
        Assert.Equal("A Life Resumed", sut.UserStories[0].StoryTitle);
        Assert.Equal(1904279, sut.UserStories[0].StoryAddress.LinkedId);
        Assert.Equal(596419, sut?.UserStories[0]?.AuthorAddress?.LinkedId);

        Assert.Equal(500, sut?.FavoriteStories.Count);
        Assert.Equal("Worldwar: Establishing A Balance", sut?.FavoriteStories[0].StoryTitle);
        Assert.Equal(11827512, sut?.FavoriteStories[0].StoryAddress.LinkedId);
    }

    [Fact]
    public void ProcessStoryNodes_MissingSTitleNodeAndMissingDataTitle_ReturnsEmptyStrings()
    {
        string html = LoadTestFile("UserPageTestFileMissingTagsForUserStory.html");

        var sut = new UserPage(html);

        Assert.Single(sut.UserStories);
    }

    [Fact]
    public void ProcessUserStories_ValidNodeWithLeadingSlashUrl_PopulatesUserStories()
    {
        // Arrange
        string html = @"
                <html>
                  <head>
                    <link rel='canonical' href='https://www.fanfiction.net/u/123456/AuthorName' />
                  </head>
                  <body>
                    <div class='z-list mystories' data-title='Test Story Title'>
                      <a class='stitle' href='/s/987654/1/Story-Name'>Story Name</a>
                    </div>
                  </body>
                </html>";

        // Act
        var page = new UserPage(html);

        // Assert
        Assert.Single(page.UserStories);
        var story = page.UserStories[0];
        Assert.Equal("Test Story Title", story.StoryTitle);
        Assert.Equal("https://www.fanfiction.net/s/987654/1/Story-Name", story.StoryAddress.Address);
    }

    [Fact]
    public void ProcessUserStories_ValidNodeWithRelativeUrl_PopulatesUserStories()
    {
        // Arrange
        string html = @"
                <html>
                  <head>
                    <link rel='canonical' href='https://www.fanfiction.net/u/123456/AuthorName' />
                  </head>
                  <body>
                    <div class='z-list mystories' data-title='Relative URL Story'>
                      <a class='stitle' href='s/111222/1/Relative-Story'>Story</a>
                    </div>
                  </body>
                </html>";

        // Act
        var page = new UserPage(html);

        // Assert
        Assert.Single(page.UserStories);
        var story = page.UserStories[0];
        Assert.Equal("Relative URL Story", story.StoryTitle);
        Assert.Equal("https://www.fanfiction.net/s/111222/1/Relative-Story", story.StoryAddress.Address);
    }

    [Fact]
    public void ProcessFavoriteStories_ValidNode_PopulatesFavoriteStories()
    {
        // Arrange
        string html = @"
                <html>
                  <head>
                    <link rel='canonical' href='https://www.fanfiction.net/u/123456/AuthorName' />
                  </head>
                  <body>
                    <div class='z-list favstories' data-title='Fav Story Title'>
                      <a class='stitle' href='/s/555444/1/Fav-Story'>Fav Story</a>
                    </div>
                  </body>
                </html>";

        // Act
        var page = new UserPage(html);

        // Assert
        Assert.Single(page.FavoriteStories);
        var story = page.FavoriteStories[0];
        Assert.Equal("Fav Story Title", story.StoryTitle);
        Assert.Equal("https://www.fanfiction.net/s/555444/1/Fav-Story", story.StoryAddress.Address);
    }

    [Fact]
    public void ProcessFavoriteAuthors_ValidAuthorNode_PopulatesFavoriteAuthors()
    {
        // Arrange
        string html = @"
                <html>
                  <body>
                    <div id='fa'>
                      <a href='/u/789012/FavoriteAuthor'>Favorite Author</a>
                    </div>
                  </body>
                </html>";

        // Act
        var page = new UserPage(html);

        // Assert
        Assert.Single(page.FavoriteAuthors);
        // Assumes UserAddress properly parses/exposes the path or identifier
        Assert.Contains("789012", page.FavoriteAuthors[0].Address);
    }

    [Fact]
    public void ProcessPage_WithMissingDataNodes_DoesNotThrowAndLeavesListsEmpty()
    {
        // Arrange - HTML structure lacks mystories, favstories, and fav authors div
        string html = @"
                <html>
                  <body>
                    <div>Some unrelated content</div>
                  </body>
                </html>";

        // Act
        var page = new UserPage(html);

        // Assert
        Assert.Empty(page.UserStories);
        Assert.Empty(page.FavoriteStories);
        Assert.Empty(page.FavoriteAuthors);
    }
}