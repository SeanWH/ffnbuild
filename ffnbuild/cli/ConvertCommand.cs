namespace ffnbuild.cli;

using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Spectre.Console;
using Spectre.Console.Cli;

public partial class ConvertCommand : Command<ConvertSettings>
{
    private readonly SortedDictionary<string, ChapterData> _chapterData = [];
    private string _storyName = string.Empty;

    public override int Execute(CommandContext context, ConvertSettings settings, CancellationToken cancellationToken)
    {
        int returnValue;
        if (string.IsNullOrEmpty(settings.SourcePath) || !Directory.Exists(settings.SourcePath))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Source path is invalid or does not exist.");
            returnValue = 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {settings.SourcePath}");
            returnValue = ConvertDirectory(settings.SourcePath).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(_storyName))
            {
                SaveTextFile(_storyName.Trim());
                AnsiConsole.MarkupLine($"[green]Successfully created text file for story:[/] {_storyName}");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error:[/] Story name could not be determined.");
                returnValue = 1;
            }
        }
        return returnValue;
    }

    private async Task<int> ConvertDirectory(string sourcePath)
    {
        int returnValue = 0;

        await AnsiConsole.Progress()
            .AutoRefresh(false)
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
            ])
            .StartAsync(async ctx =>
        {
            var task = ctx.AddTask("Processing HTML files...", autoStart: true);
            task.MaxValue = Directory.EnumerateFiles(sourcePath).Count();
            try
            {
                foreach (var filePath in Directory.EnumerateFiles(sourcePath))
                {
                    var doc = new HtmlDocument();
                    doc.Load(filePath);
                    _storyName = GetStoryTitle(doc.DocumentNode.SelectSingleNode("//title").InnerText).Trim();
                    ProcessHtmlFile(doc);
                    task.Increment(1);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] An exception occurred while processing files: [yellow]{ex.Message}[/]");
                returnValue = 1;
            }
        });

        return returnValue;
    }

    private void ProcessHtmlFile(HtmlDocument doc)
    {
        var chapterName = GetChapterTitle(doc.DocumentNode.SelectSingleNode("//title").InnerText);

        var storyText = doc.DocumentNode.SelectSingleNode("//div[contains(concat(' ', normalize-space(@class), ' '),' storytext ')]");
        var paras = storyText.SelectNodes("//p");
        List<string?> lines = ParseChapterText(paras);

        if (lines.Count > 0 || lines != null)
        {
            if (_chapterData.ContainsKey(chapterName))
            {
                AppendStoryData(chapterName, lines);
            }
            else
            {
                AppendChapterData(chapterName, lines);
            }
        }
    }

    private void AppendChapterData(string chapterName, List<string?> lines)
    {
        if (lines.Count == 0 || lines == null)
        {
            return;
        }

        _chapterData.Add(chapterName, new ChapterData(chapterName, lines));
    }

    private void AppendStoryData(string chapterName, List<string?> lines)
    {
        ChapterData existing = _chapterData[chapterName];
        List<string?> existingLines = existing.Paragraphs;
        foreach (var line in lines)
        {
            existingLines.Add(line);
        }

        _ = _chapterData.Remove(chapterName);
        _chapterData.Add(chapterName, new ChapterData(chapterName, existingLines));
    }

    private static List<string?> ParseChapterText(HtmlNodeCollection nodes)
    {
        List<string?> lines = [];
        foreach (var para in nodes)
        {
            lines.Add(para.InnerText);
        }
        return lines;
    }

    private static string GetChapterTitle(string titleString)
    {
        if (!String.IsNullOrWhiteSpace(titleString))
        {
            var regex = ChapterTitleRegex();
            var match = regex.Match(titleString);
            if (match.Success)
            {
                regex = ChapterIndexRegex();
                var newMatch = regex.Match(match.Value);
                string value;

                if (newMatch.Success)
                {
                    if (newMatch.Value.Length == 1)
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
                    if (value == match.Value)
                    {
                        return value;
                    }
                }

                return $"Chapter {value}";
            }
        }

        throw new ArgumentException("Invalid chapter title passed to GetChapterTitle.", nameof(titleString));
    }

    private void SaveTextFile(string fileName)
    {
        var finalName = fileName + ".txt";
        using var outFile = File.CreateText(finalName);
        foreach (ChapterData chapterData in _chapterData.Values)
        {
            //log.WriteLine(chapterData.Title);
            outFile.WriteLine(chapterData.Title);
            outFile.WriteLine();
            foreach (string? line in chapterData.Paragraphs)
            {
                outFile.WriteLine(line);
                outFile.WriteLine();
            }
        }

        outFile.Flush();
    }

    private static string GetStoryTitle(string titleString)
    {
        var index = titleString.IndexOf("Chapter");
        return titleString[..index];
    }

    [GeneratedRegex(@"Chapter[\w\s:]+")]
    private static partial Regex ChapterTitleRegex();

    [GeneratedRegex(@"\d+")]
    private static partial Regex ChapterIndexRegex();
}