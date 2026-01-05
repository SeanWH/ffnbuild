namespace ffnbuild.cli;

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Spectre.Console;
using Spectre.Console.Cli;

public partial class ConvertCommand : Command<ConvertSettings>
{
    private readonly SortedDictionary<string, ChapterData> _chapterData = new SortedDictionary<string, ChapterData>(new NaturalStringComparer());
    private string _storyName = string.Empty;
    private bool? _createHtmlVersion = false;

    public override int Execute(CommandContext context, ConvertSettings settings, CancellationToken cancellationToken)
    {
        int returnValue = 0;
        _createHtmlVersion = settings.CreateHtml;

        if (string.IsNullOrWhiteSpace(settings.SourcePath))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Source path is invalid or does not exist.");
            return 1;
        }

        if (settings.SourcePath.Contains(','))
        {
            string[] paths = settings.SourcePath.Split(",");
            foreach (var path in paths)
            {
                AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {path}");
                var sourcePath = Path.Combine("data", path.Trim());
                if (ValidatePath(sourcePath))
                {
                    returnValue += ProcessFolder(sourcePath);
                }
                _chapterData.Clear();
            }
        }
        else if (Path.IsPathRooted(settings.SourcePath))
        {
            var sourcePath = settings.SourcePath;
            if (ValidatePath(sourcePath))
            {
                AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {settings.SourcePath}");
                returnValue += ProcessFolder(sourcePath);
            }
        }
        else if (settings.SourcePath == ".")
        {
            foreach (string folder in Directory.EnumerateDirectories("data"))
            {
                AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {folder}");
                //var sourcePath = Path.Combine("data", path.Trim());
                if (ValidatePath(folder))
                {
                    returnValue += ProcessFolder(folder);
                }
            }
        }
        else
        {
            var sourcePath = Path.Combine("data", settings.SourcePath);
            if (ValidatePath(sourcePath))
            {
                AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {sourcePath}");
                returnValue += ProcessFolder(sourcePath);
            }
        }

        if (returnValue > 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Error(s) occurred during conversion process.");
        }

        return returnValue;
    }

    private bool ValidatePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Source path is invalid or does not exist.");
            return false;
        }

        return true;
    }

    private int ProcessFolder(string pathToFolder)
    {
        var returnValue = ConvertDirectory(pathToFolder).GetAwaiter().GetResult();
        if (!string.IsNullOrEmpty(_storyName))
        {
            SaveTextFile(_storyName.Trim());
            AnsiConsole.MarkupLine($"[green]Successfully created text file for story:[/] {_storyName}");

            if (_createHtmlVersion is not null && _createHtmlVersion == true)
            {
                SaveHtmlConversionFile(_storyName.Trim());
                AnsiConsole.MarkupLine($"[green]Successfully created html conversion file for story:[/] {_storyName}");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Story name could not be determined.");
            returnValue = 1;
        }

        return returnValue;
    }

    private void SaveHtmlConversionFile(string storyTitle)
    {
        if (string.IsNullOrWhiteSpace(storyTitle))
        {
            return;
        }

        if (_chapterData is null)
        {
            return;
        }

        HtmlDocBuilder.BuildDocument(storyTitle, _chapterData);
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
                    if (Path.GetExtension(filePath).ToLower().Contains(".htm"))
                    {
                        var doc = new HtmlDocument();
                        doc.Load(filePath);
                        _storyName = GetStoryTitle(doc.DocumentNode.SelectSingleNode("//title").InnerText).Trim();
                        if (string.IsNullOrWhiteSpace(_storyName))
                        {
                            throw new GetTitleException($"Story title could not be determined from file: {filePath}");
                        }

                        ProcessHtmlFile(doc);
                        task.Increment(1);
                    }
                    else
                    {
                        task.Increment(1);
                    }
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
        var finalPath = Path.Combine("output", finalName);

        if (!Directory.Exists("output"))
        {
            Directory.CreateDirectory("output");
        }

        using var outFile = File.CreateText(finalPath);

        foreach (ChapterData chapterData in _chapterData.Values)
        {
            //log.WriteLine(chapterData.Title);
            outFile.WriteLine(chapterData.Title);
            outFile.WriteLine();
            foreach (string? line in chapterData.Paragraphs)
            {
                outFile.WriteLine(line?.Trim());
                outFile.WriteLine();
            }
        }

        outFile.Flush();
    }

    private static string GetStoryTitle(string titleString)
    {
        var index = titleString.IndexOf("Chapter");
        string possibleTitle = string.Empty;
        if (index > 0)
        {
            possibleTitle = titleString[..index].Trim();
            if (!string.IsNullOrWhiteSpace(possibleTitle))
            {
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    possibleTitle = possibleTitle.Replace(c, '_');
                }
            }
        }
        return possibleTitle;
    }

    [GeneratedRegex(@"Chapter[\w\s:]+")]
    private static partial Regex ChapterTitleRegex();

    [GeneratedRegex(@"\d+")]
    private static partial Regex ChapterIndexRegex();
}