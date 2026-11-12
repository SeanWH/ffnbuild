namespace ffnparser.tests.pages;

using ffnparser.data.model.pages;

using Xunit;

public class UserPageTests
{
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
    public void Constructor_FavoriteAuthorsDivWithNoUserLinks_LeavesFavoriteAuthorsEmpty()
    {
        // Hits: if (authorNodes == null) return; when div#fa exists but has no
        // /u/ anchors
        string html = """
            <html>
                <body>
                    <div id="fa">
                        <p>This user has no favorite authors.</p>
                    </div>
                </body>
            </html>
            """;

        var sut = new UserPage(html);

        Assert.Empty(sut.FavoriteAuthors);
    }

    [Fact]
    public void Constructor_FavoriteAuthorsSectionWithValidAndInvalidLinks_PopulatesOnlyUserAddresses()
    {
        string html = """
            <html>
                <body>
                    <div id="fa">
                        <a href="https://fanfiction.net/u/44444/Fav-Author-One">Fav Author One</a>
                        <a href="https://fanfiction.net/u/55555/Fav-Author-Two">Fav Author Two</a>
                        <a href="https://fanfiction.net/s/66666/1/Not-An-Author">Story Link</a>
                        <a>Anchor Without Href</a>
                    </div>
                </body>
            </html>
            """;

        var sut = new UserPage(html);

        Assert.Equal(2, sut.FavoriteAuthors.Count);
        Assert.Equal(44444, sut.FavoriteAuthors[0].LinkedId);
        Assert.Equal(55555, sut.FavoriteAuthors[1].LinkedId);
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
        string html = """
            <html>
                <head>
                    <link rel="canonical" href="https://fanfiction.net/u/12345/JohnDoe" />
                </head>
                <body>
                    <div class="z-list mystories" data-title="My First Story">
                        <a class="stitle" href="/s/11111/1/My-First-Story">My First Story</a>
                    </div>
                    <div class="z-list mystories" data-title="My Second Story">
                        <a class="stitle" href="/s/22222/1/My-Second-Story">My Second Story</a>
                    </div>

                    <div class="z-list favstories" data-title="Favorite Story One">
                        <a class="stitle" href="/s/33333/1/Favorite-Story-One">Favorite Story One</a>
                    </div>
                </body>
            </html>
            """;

        var sut = new UserPage(html);

        Assert.Equal(2, sut.UserStories.Count);
        Assert.Equal("My First Story", sut.UserStories[0].StoryTitle);
        Assert.Equal(11111, sut.UserStories[0].StoryAddress.LinkedId);
        Assert.Equal(12345, sut?.UserStories[0]?.AuthorAddress?.LinkedId);

        Assert.Single(sut.FavoriteStories);
        Assert.Equal("Favorite Story One", sut.FavoriteStories[0].StoryTitle);
        Assert.Equal(33333, sut.FavoriteStories[0].StoryAddress.LinkedId);
    }
}