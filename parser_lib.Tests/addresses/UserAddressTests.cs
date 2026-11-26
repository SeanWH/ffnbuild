namespace parser_lib.Tests.addresses;

using Moq;

using parser_lib.addresses;

[TestFixture]
public class UserAddressTests
{
    [Test]
    public void Constructor_FromParts_Ao3User_ParsesCorrectly()
    {
        var user = new UserAddress("/users/Writer/pseuds/PenName", "a03", new[] { "users", "Writer", "pseuds", "PenName" });

        Assert.Multiple(() =>
        {
            Assert.That(user.LinkedId, Is.EqualTo(-1));
            Assert.That(user.UserName, Is.EqualTo("Writer"));
            Assert.That(user.A03Pseud, Is.EqualTo("PenName"));
        });
    }

    [Test]
    public void Constructor_FromParts_FfnUser_ParsesCorrectly()
    {
        var user = new UserAddress("/u/789/Author_One", "ffn", new[] { "u", "789", "Author_One" });

        Assert.Multiple(() =>
        {
            Assert.That(user.LinkedId, Is.EqualTo(789));
            Assert.That(user.UserName, Is.EqualTo("Author_One"));
            Assert.That(user.Text, Is.EqualTo("Author One"));
            Assert.That(user.LinkType, Is.EqualTo("user"));
        });
    }

    [Test]
    public void Constructor_FromParts_InvalidSiteOrId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new UserAddress("/u/bad/Name", "ffn", new[] { "u", "bad", "Name" }));
    }

    [Test]
    public void Equals_WithMockIFfnAddress_HandlesPolymorphismCorrectly()
    {
        var u1 = new UserAddress("/u/1/A", "A", 1, "ffn");
        var u2 = new UserAddress("/u/2/B", "B", 2, "ffn");

        var mockAddress = new Mock<IFfnAddress>();

        Assert.Multiple(() =>
        {
            Assert.That(u1.Equals((object)mockAddress.Object), Is.False);
            Assert.That(u1 < u2, Is.True);
            Assert.That(u2 > u1, Is.True);
            Assert.That(u1.ToString(), Does.Contain("AuthorName: A"));
        });
    }
}