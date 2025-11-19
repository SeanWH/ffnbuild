using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ffnbuild;

using HtmlAgilityPack;



SortedDictionary<string, ChapterData> _chapterData = [];

string _storyName = string.Empty;

if (args.Length == 0)
{
    Console.WriteLine("Please provide the path to the folder containg the HTML files.");
    return;
}

if (!Directory.Exists(args[0]))
{
    Console.WriteLine("Specified directory does not exist.");
    return;
}


using StreamWriter log = File.CreateText($"{args[0]}_convert_log.log");

foreach (var filePath in Directory.EnumerateFiles(args[0]))
{
    var doc = new HtmlDocument();
    doc.Load(filePath);
    _storyName = GetStoryTitle(doc.DocumentNode.SelectSingleNode("//title").InnerText).Trim();

    ProcessHtmlFile(doc);
}

SaveTextFile(_storyName);

void ProcessHtmlFile(HtmlDocument doc)
{
    

    var chapterName = GetChapterTitle(doc.DocumentNode.SelectSingleNode("//title").InnerText);

    var storyText = doc.DocumentNode.SelectSingleNode("//div[contains(concat(' ', normalize-space(@class), ' '),' storytext ')]");
    var paras = storyText.SelectNodes("//p");
    List<string> lines =  ParseChapterText(paras);

    if (_chapterData.ContainsKey(chapterName))
    {
        AppendStoryData(chapterName, lines);
    }
    else
    {
        _chapterData.Add(chapterName, new ChapterData(chapterName, lines));
    }
    
    
}

void AppendStoryData(string chapterName, List<string> lines)
{
    ChapterData existing = _chapterData[chapterName];
    List<string> existingLines = existing.Paragraphs;
    foreach(var line in lines)
    {
        existingLines.Add(line);
    }

    _ = _chapterData.Remove(chapterName);
    _chapterData.Add(chapterName, new ChapterData(chapterName, existingLines));
}

List<string> ParseChapterText(HtmlNodeCollection nodes)
{
    List<string> lines = [];
    foreach (var para in nodes)
    {
        lines.Add(para.InnerText);
    }
    return lines;
}

static string GetChapterTitle(string titleString)
{
    if (!String.IsNullOrWhiteSpace(titleString))
    {
        var regex = ChapterTitleRegex();
        var match = regex.Match(titleString);
        if (match.Success)
        {
            regex = ChapterIndexRegex();
            var newMatch = regex.Match(match.Value);
            string value = "";

            if (newMatch.Success)
            {
                if(newMatch.Value.Length == 1)
                {
                    value = "0" + newMatch.Value;
                }
                else
                {
                    value = newMatch.Value;
                }

            }
            else
            {
                value = TextToDigitConverter.Convert(match.Value);
                if(value == match.Value)
                {
                    return value;
                }
            }

            return $"Chapter {value}";

        }
    }

    throw new ArgumentException("Invalid chapter title passed to GetChapterTitle.", nameof(titleString));
}

void SaveTextFile(string fileName)
{
    var finalName = fileName + ".txt";
    using var outFile = File.CreateText(finalName);
    foreach(ChapterData chapterData in _chapterData.Values)
    {
        log.WriteLine(chapterData.Title);
        outFile.WriteLine(chapterData.Title);
        outFile.WriteLine();
        foreach(string line in chapterData.Paragraphs)
        {
            outFile.WriteLine(line);
            outFile.WriteLine();
        }
    }

    outFile.Flush();
    
}

static string GetStoryTitle(string titleString)
{
    var index = titleString.IndexOf("Chapter");
    return titleString[..index];
}

partial class Program
{
    [GeneratedRegex(@"\d+")]
    private static partial Regex ChapterIndexRegex();

    [GeneratedRegex(@"Chapter[\w\s:]+")]
    private static partial Regex ChapterTitleRegex();
}