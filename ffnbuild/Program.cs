using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ffnbuild;

using HtmlAgilityPack;

SortedDictionary<string, ChapterData> _chapterData = new SortedDictionary<string, ChapterData>();
using StreamWriter log = File.CreateText("convert_log.log");
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



foreach(var filePath in Directory.EnumerateFiles(args[0]))
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

    _chapterData.Add(chapterName, new ChapterData(chapterName, lines));

    
    
}

List<string> ParseChapterText(HtmlNodeCollection nodes)
{
    List<string> lines = new List<string>();
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
        var regex = new Regex(@"Chapter[\w\s:]+");
        var match = regex.Match(titleString);
        if (match.Success)
        {
            regex = new Regex(@"\d+");
            var newMatch = regex.Match(match.Value);

            if (newMatch.Success)
            {
                string value = "";
                if(newMatch.Value.Length == 1)
                {
                    value = "0" + newMatch.Value;
                }
                else
                {
                    value = newMatch.Value;
                }

                return $"Chapter {value}";
            }
        }
    }

    return "Chapter X";
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
    return titleString.Substring(0, index);
}