namespace parser_lib.Tests.addresses;

using parser_lib.addresses;

[TestFixture]
public class StoryAddressTests
{
    [Test]
    public void Constructor_DirectProperties_SetsAllFields()
    {
        var story = new StoryAddress("/s/100/1/My-Story", 100, "My-Story", "Ch 1", "1", "ffn");

        Assert.Multiple(() =>
        {
            Assert.That(story.LinkedId, Is.EqualTo(100));
            Assert.That(story.ChapterIndex, Is.EqualTo("1"));
            Assert.That(story.ChapterTitle, Is.EqualTo("Ch 1"));
            Assert.That(story.LinkType, Is.EqualTo("story"));
            Assert.That(story.Text, Is.EqualTo("My Story"));
            Assert.That(story.ToString(), Does.Contain("StoryTitle: My-Story"));
        });
    }

    [Test]
    public void Constructor_FromParts_Ao3TwoElementArray_ThrowsIndexOutOfRangeException()
    {
        // Documents current behavior: constructor accesses parts[3] unconditionally
        Assert.Throws<IndexOutOfRangeException>(() =>
            new StoryAddress("/works/12345", "a03", new[] { "works", "12345" }));
    }

    [Test]
    public void Constructor_FromParts_FfnFormat_ParsesCorrectly()
    {
        var story = new StoryAddress("/s/555/3/Title_Here", "ffn", new[] { "s", "555", "3", "Title_Here" });

        Assert.Multiple(() =>
        {
            Assert.That(story.LinkedId, Is.EqualTo(555));
            Assert.That(story.ChapterIndex, Is.EqualTo("3"));
            Assert.That(story.StoryTitle, Is.EqualTo("Title_Here"));
            Assert.That(story.Text, Is.EqualTo("Title Here"));
        });
    }

    [Test]
    public void Constructor_FromParts_InvalidId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoryAddress("addr", "ffn", new[] { "s", "abc", "1", "Title" }));
    }

    [Test]
    public void Operators_CompareByLinkedId()
    {
        var s1 = new StoryAddress("a", 1, "T1", "C1", "1", "ffn");
        var s2 = new StoryAddress("b", 2, "T2", "C1", "1", "ffn");
        StoryAddress? sNull = null;

        Assert.Multiple(() =>
        {
            Assert.That(s1 < s2, Is.True);
            Assert.That(s2 > s1, Is.True);
            Assert.That(s1 != s2, Is.True);
            Assert.That(sNull < s1, Is.True);
            Assert.That(sNull > s1, Is.False);
        });
    }
}