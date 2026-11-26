namespace parser_lib.Tests.addresses;

using Moq;

using NUnit.Framework;

using parser_lib.addresses;

[TestFixture]
public class GroupAddressTests
{
    [Test]
    public void Constructor_DirectProperties_SetsAllFields()
    {
        var group = new GroupAddress("/community/My_Group/123/", 123, "My_Group&amp;Co", "ffn");

        Assert.Multiple(() =>
        {
            Assert.That(group.Address, Is.EqualTo("/community/My_Group/123/"));
            Assert.That(group.LinkedId, Is.EqualTo(123));
            Assert.That(group.GroupName, Is.EqualTo("My_Group&amp;Co"));
            Assert.That(group.Site, Is.EqualTo("ffn"));
            Assert.That(group.Category, Is.EqualTo("General"));
            Assert.That(group.LinkType, Is.EqualTo("group"));
            Assert.That(group.Text, Is.EqualTo("My Group&Co"));
        });
    }

    [Test]
    public void Constructor_FromParts_InvalidId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new GroupAddress("addr", "ffn", new[] { "community", "Sci-Fi", "not-an-int" }));
    }

    [Test]
    public void Constructor_FromParts_ValidId_ParsesCorrectly()
    {
        var group = new GroupAddress("addr", "ffn", new[] { "community", "Sci-Fi", "456" });

        Assert.That(group.LinkedId, Is.EqualTo(456));
        Assert.That(group.GroupName, Is.EqualTo("Sci-Fi"));
        Assert.That(group.Text, Is.EqualTo("Sci Fi"));
    }

    [Test]
    public void EqualityAndComparisonOperators_WorkAsExpected()
    {
        var g1 = new GroupAddress("a1", 10, "A", "ffn");
        var g1SameId = new GroupAddress("a2", 10, "B", "ffn");
        var g2 = new GroupAddress("a3", 20, "C", "ffn");
        GroupAddress? gNull = null;

        var otherMock = new Mock<IFfnAddress>();

        Assert.Multiple(() =>
        {
            Assert.That(g1 == g1SameId, Is.True);
            Assert.That(g1 != g2, Is.True);
            Assert.That(g1 < g2, Is.True);
            Assert.That(g1 <= g1SameId, Is.True);
            Assert.That(g2 > g1, Is.True);
            Assert.That(g2 >= g1, Is.True);
            Assert.That(gNull == null, Is.True);
            Assert.That(gNull < g1, Is.True);
            Assert.That(gNull <= g1, Is.True);
            Assert.That(gNull >= null, Is.True);
            Assert.That(g1.Equals((object)g1SameId), Is.True);
            Assert.That(g1.Equals((object?)null), Is.False);
            Assert.That(g1.Equals(otherMock.Object), Is.False);
            Assert.That(g1.CompareTo(otherMock.Object), Is.EqualTo(-1));
            Assert.That(g1.GetHashCode(), Is.EqualTo(g1SameId.GetHashCode()));
            Assert.That(g1.ToString(), Does.Contain("Linked ID: 10"));
        });
    }
}