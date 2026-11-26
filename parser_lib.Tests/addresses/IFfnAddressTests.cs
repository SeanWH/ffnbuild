namespace parser_lib.Tests.addresses;

using Moq;

using parser_lib.addresses;

[TestFixture]
public class IFfnAddressTests
{
    [Test]
    public void DefaultInterfaceMembers_ViaMoq_ExecuteDefaultImplementations()
    {
        var mock = new Mock<IFfnAddress> { CallBase = true };
        mock.SetupGet(x => x.Address).Returns("https://fanfiction.net");
        mock.SetupGet(x => x.LinkedId).Returns(1);
        mock.SetupGet(x => x.LinkType).Returns("story");
        mock.SetupGet(x => x.Text).Returns("Test");

        IFfnAddress instance = mock.Object;

        Assert.That(instance.ToString(), Is.EqualTo("Address: https://fanfiction.net, LinkedId: 1, LinkTarget: story, Text: Test"));
        Assert.That(instance.GetHashCode(), Is.Not.Zero);
    }

    [TestCase("hello-world", "hello world")]
    [TestCase("hello_world", "hello world")]
    [TestCase("a-b_c-d_e", "a b c d e")]
    [TestCase("Tom-&amp;-Jerry_Show", "Tom & Jerry Show")]
    [TestCase("Already Clean Text", "Already Clean Text")]
    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("   ", "")]
    [TestCase("\t\r\n", "")]
    [TestCase("  my-cool_story&amp;more  ", "my cool story&more")]
    [TestCase("no_dashes_or_underscores", "no dashes or underscores")]
    public void FormatText_ValidInput_ReplacesDashesUnderscoresAndDecodesHtml(string? input, string expected)
    {
        string? result = IFfnAddress.FormatText(input);

        Assert.That(result, Is.EqualTo(expected));
    }
}