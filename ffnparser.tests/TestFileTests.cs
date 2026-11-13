namespace ffnparser.tests;

public class TestFileTests
{
    [Fact]
    public void ReadTestFile_Succeeds()
    {
        var filePath = Path.Combine("test_files", "UserPageTestFile.html");
        string fileContent = File.ReadAllText(filePath);
        Assert.False(string.IsNullOrWhiteSpace(fileContent), $"Test file '{filePath}' is empty or whitespace.");
    }

    [Fact]
    public void TestFileExists()
    {
        string userTestFilePath = "test_files/UserPageTestFile.html";
        string oneShotTestFilePath = "test_files/OneShotSingleChapterTestFile.html";
        string multiChapterTestFilePath = "test_files/MultiChapterStoryTestFile.html";
        Assert.True(File.Exists(userTestFilePath), $"Test file '{userTestFilePath}' does not exist.");
        Assert.True(File.Exists(oneShotTestFilePath), $"Test file '{oneShotTestFilePath}' does not exist.");
        Assert.True(File.Exists(multiChapterTestFilePath), $"Test file '{multiChapterTestFilePath}' does not exist.");
    }
}