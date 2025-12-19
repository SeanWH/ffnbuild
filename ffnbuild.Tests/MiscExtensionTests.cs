namespace ffnbuild.Tests;

using ffnbuild.ext;

using Xunit;

public class MiscExtensionTests
{
    [Fact]
    public void ReturnsTrueIfStringTargetInArgsList()
    {
        string target = "br";

        Assert.True(target.IsIn("br", "hr"));
    }

    [Fact]
    public void ReturnsFalseIfStringTargetNotInArgsList()
    {
        string target = "html";
        Assert.False(target.IsIn("br", "hr"));
    }

    [Fact]
    public void ArrayTest()
    {
        string[] emptyElements = {"area","base","br","col","embed","hr","img","input","link",
            "meta","param","source","track","wbr" };

        string target = "br";

        Assert.True(target.IsIn(emptyElements));
    }

    [Fact]
    public void ArrayTest2()
    {
        string[] emptyElements = {"area","base","br","col","embed","hr","img","input","link",
            "meta","param","source","track","wbr" };

        string target = "html";

        Assert.False(target.IsIn(emptyElements));
    }

    [Fact]
    public void ArrayTest3()
    {
        string[] emptyElements = {"area","base","br","col","embed","hr","img","input","link",
            "meta","param","source","track","wbr" };

        string target = "param";

        Assert.True(target.IsIn(emptyElements));
    }

    [Fact]
    public void IntReturnsTrueIfTargetInParams()
    {
        int testVal = 0;

        Assert.True(testVal.IsIn(0, 1, 2, 3, 4, 5, 6, 7, 8, 9));
    }

    [Fact]
    public void IntReturnsFalseIfTargetNotInParams()
    {
        int testVal = 10;

        Assert.False(testVal.IsIn(0, 1, 2, 3, 4, 5, 6, 7, 8, 9));
    }

    [Fact]
    public void IntReturnsTrueIfTargetInArray()
    {
        int testVal = 0;
        int[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        Assert.True(testVal.IsIn(testArray));
    }

    [Fact]
    public void IntReturnsFalseIfTargetNotInArray()
    {
        int testVal = 10;
        int[] testArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        Assert.False(testVal.IsIn(testArray));
    }
}