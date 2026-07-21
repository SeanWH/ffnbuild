namespace ffnbuild.cli;

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ffnbuild.data.comparers;
using ffnbuild.data.exceptions;
using ffnbuild.data.extensions;
using ffnbuild.data.model;

using HtmlAgilityPack;

using Serilog;

using Spectre.Console;
using Spectre.Console.Cli;

public partial class ConvertCommand : Command<ConvertSettings>
{
    private readonly SortedDictionary<string, ChapterData> _chapterData = new SortedDictionary<string, ChapterData>(new NaturalStringComparer());
    private string _storyName = string.Empty;

    private static List<string?> ParseChapterText(HtmlNodeCollection nodes)
    {
        List<string?> lines = [];
        Regex regex = new Regex(@"\r\n|\n|\r");
        Regex r2 = new Regex(@"\s{2,}");
        foreach( var para in nodes )
        {
            var line = regex.Replace(para.InnerText, " ").Trim();
            line = r2.Replace(line, " ");
            lines.Add(line);
        }
        return lines;
    }

    private static bool ValidatePath(string path)
    {
        if( string.IsNullOrWhiteSpace(path) || !Directory.Exists(path) )
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Source path is invalid or does not exist.");
            return false;
        }

        if( !Directory.EnumerateFiles(path).Any() )
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Source path does not contain any files to process.");
            return false;
        }

        return true;
    }

    private void AppendChapterData(string chapterName, List<string?> lines)
    {
        if( lines.Count == 0 || lines == null )
        {
            Log.Debug("Count of lines in chapter is null or 0.");
            return;
        }

        _chapterData.Add(chapterName, new ChapterData(chapterName, lines));
    }

    private void AppendStoryData(string chapterName, List<string?> lines)
    {
        ChapterData existing = _chapterData[chapterName];
        List<string?> existingLines = existing.Paragraphs;
        foreach( var line in lines )
        {
            existingLines.Add(line);
        }

        _ = _chapterData.Remove(chapterName);
        _chapterData.Add(chapterName, new ChapterData(chapterName, existingLines));
    }

    private async Task<int> ConvertDirectory(string sourcePath)
    {
        int returnValue = 0;

        await AnsiConsole.Progress()
            .AutoRefresh(false)
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(

                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn()
            )
            .StartAsync(async ctx =>
        {
            var task = ctx.AddTask("Processing HTML files...", autoStart: true);
            task.MaxValue = Directory.EnumerateFiles(sourcePath).Count();
            try
            {
                foreach( var filePath in Directory.EnumerateFiles(sourcePath) )
                {
                    if( Path.GetExtension(filePath).ToLower().Contains(".htm", StringComparison.OrdinalIgnoreCase) )
                    {
                        Log.Information("Processing file: {FilePath}", filePath);
                        var doc = new HtmlDocument();
                        doc.Load(filePath);
                        _storyName = doc.DocumentNode.SelectSingleNode("//title").InnerText.GetStoryTitle().Trim();
                        Log.Information("StoryName: {StoryName}", _storyName);
                        if( string.IsNullOrWhiteSpace(_storyName) )
                        {
                            Log.Error("Could not determine storytitle from file: {FilePath}", filePath);
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
            catch( Exception ex )
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] An exception occurred while processing files: [yellow]{ex.Message}[/]");
                returnValue = 1;
            }
        });

        return returnValue;
    }

    private int ProcessFolder(string pathToFolder)
    {
        var returnValue = ConvertDirectory(pathToFolder).GetAwaiter().GetResult();
        if( !string.IsNullOrEmpty(_storyName) )
        {
            SaveTextFile(_storyName.Trim());
            AnsiConsole.MarkupLine($"[green]Successfully created text file for story:[/] {_storyName}");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Story name could not be determined.");
            returnValue = 1;
        }

        return returnValue;
    }

    private void ProcessHtmlFile(HtmlDocument doc)
    {
        var chapterName = doc.DocumentNode.SelectSingleNode("//title").InnerText.GetChapterTitle();
        Log.Information("Chapter name: {ChapterName}", chapterName);

        var storyText = doc.DocumentNode.SelectSingleNode("//div[contains(concat(' ', normalize-space(@class), ' '),' storytext ')]");
        var paras = storyText.SelectNodes("//p");
        List<string?> lines = ParseChapterText(paras);

        if( lines.Count > 0 || lines != null )
        {
            if( _chapterData.ContainsKey(chapterName) )
            {
                AppendStoryData(chapterName, lines);
            }
            else
            {
                AppendChapterData(chapterName, lines);
            }
        }
    }

    private Task<int> ProcessMultipleFolders(string folderPaths)
    {
        int returnValue = 0;
        string[] paths = folderPaths == "." ? Directory.EnumerateDirectories("data").ToArray() : folderPaths.Split(",");
        foreach( var path in paths )
        {
            Log.Information("Processing Folder: {FolderPath}", path);
            AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {path}");
            var sourcePath = path.Contains("data") ? path : Path.Combine("data", path.Trim());
            if( ValidatePath(sourcePath) )
            {
                returnValue += ProcessFolder(sourcePath);
            }
            _chapterData.Clear();
        }
        return Task.FromResult(returnValue);
    }

    private void SaveTextFile(string fileName)
    {
        var finalName = fileName + ".txt";
        var finalPath = Path.Combine("output", finalName);

        if( !Directory.Exists("output") )
        {
            Directory.CreateDirectory("output");
        }

        using var outFile = File.CreateText(finalPath);

        foreach( ChapterData chapterData in _chapterData.Values )
        {
            //log.WriteLine(chapterData.Title);
            outFile.WriteLine(chapterData.Title);
            outFile.WriteLine();
            foreach( string? line in chapterData.Paragraphs )
            {
                outFile.WriteLine(line?.Trim());
                outFile.WriteLine();
            }
        }

        outFile.Flush();
    }

    protected override int Execute(CommandContext context, ConvertSettings settings, CancellationToken cancellationToken)
    {
        int returnValue = 0;

        if( string.IsNullOrWhiteSpace(settings.SourcePath) )
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Source path is invalid or does not exist.");
            return 1;
        }

        Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Verbose()
           .WriteTo.File("ffnbuild.log", rollingInterval: RollingInterval.Minute)
           .CreateLogger();
        Serilog.Debugging.SelfLog.Enable(Console.Error);

        if( settings.SourcePath.Contains(',') || settings.SourcePath == "." )
        {
            Log.Information("Processing multiple files:");
            returnValue = ProcessMultipleFolders(settings.SourcePath).GetAwaiter().GetResult();
        }
        else if( Path.IsPathRooted(settings.SourcePath) )
        {
            var sourcePath = settings.SourcePath;
            if( ValidatePath(sourcePath) )
            {
                Log.Information("Processing from rooted source path: {SourcePath}", settings.SourcePath);
                AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {settings.SourcePath}");
                returnValue += ProcessFolder(sourcePath);
            }
        }
        else
        {
            var sourcePath = Path.Combine("data", settings.SourcePath);
            if( ValidatePath(sourcePath) )
            {
                Log.Information("Processing from non-rooted source path: {SourcePath}", settings.SourcePath);
                AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {sourcePath}");
                returnValue += ProcessFolder(sourcePath);
            }
        }

        if( returnValue > 0 )
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Error(s) occurred during conversion process.");
        }

        return returnValue;
    }
}