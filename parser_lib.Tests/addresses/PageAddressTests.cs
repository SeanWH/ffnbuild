namespace parser_lib.Tests.addresses;

using Moq;

using parser_lib.addresses;

[TestFixture]
public class PageAddressTests
{
    [Test]
    public void PropertiesAndOperators_WorkAsExpected()
    {
        var p1 = new PageAddress("https://example.com");
        var p1Dup = new PageAddress("https://example.com", 99);
        var p2 = new PageAddress("https://example2.com");
        PageAddress? pNull = null;

        var otherMock = new Mock<IFfnAddress>();
        otherMock.SetupGet(x => x.LinkType).Returns("story");
        otherMock.SetupGet(x => x.Address).Returns("https://example.com");

        Assert.Multiple(() =>
        {
            Assert.That(p1.LinkedId, Is.EqualTo(-1));
            Assert.That(p1.LinkType, Is.EqualTo("page"));
            Assert.That(p1.Site, Is.EqualTo(""));
            Assert.That(p1.Text, Is.EqualTo("https://example.com"));

            Assert.That(p1 == p1Dup, Is.True);
            Assert.That(p1 != p2, Is.True);
            Assert.That(p1 < p2, Is.True);
            Assert.That(p1 <= p1Dup, Is.True);
            Assert.That(p2 > p1, Is.True);
            Assert.That(p2 >= p1, Is.True);
            Assert.That(pNull < p1, Is.True);
            Assert.That(p1.Equals(otherMock.Object), Is.False);
            Assert.That(p1.CompareTo(otherMock.Object), Is.Not.Zero);
            Assert.That(p1.GetHashCode(), Is.Not.EqualTo(p1Dup.GetHashCode()));
        });
    }
}