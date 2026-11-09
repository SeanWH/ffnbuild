namespace ffnbuild.cli;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ffnbuild.data.comparers;
using ffnbuild.data.exceptions;
using ffnbuild.data.model;

using Serilog;

using Spectre.Console;
using Spectre.Console.Cli;

public partial class ConvertCommand : Command<ConvertSettings>
{
    private readonly SortedDictionary<string, ChapterData> _chapterData = new SortedDictionary<string, ChapterData>(new NaturalStringComparer());
    private StoryMetaData _storyMetaData = new StoryMetaData();
    private string _storyName = string.Empty;

    private static bool ValidatePath(string path, string dateToProcess)
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

        if( !string.IsNullOrWhiteSpace(dateToProcess) )
        {
            var folderDate = Directory.GetCreationTime(path).ToString("yyyy-MM-dd");
            var touchDate = Directory.GetLastWriteTime(path).ToString("yyyy-MM-dd");
            if( folderDate != dateToProcess && touchDate != dateToProcess )
            {
                return false;
            }
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
        if( lines == null || lines.Count == 0 )
        {
            Log.Debug("Count of lines in chapter is null or 0.");
            return;
        }

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
            .AutoRefresh(true)
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(

                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn()
            )
            .StartAsync(async ctx =>
        {
            var files = Directory.EnumerateFiles(sourcePath).Where(f => Path.GetExtension(f).ToLower().Contains(".htm", StringComparison.OrdinalIgnoreCase)).ToList();

            var task = ctx.AddTask("Processing HTML files...", autoStart: true);
            task.MaxValue = files.Count;
            try
            {
                foreach( var filePath in files )
                {
                    Log.Debug("Currently processing file: {FilePath}", filePath);
                    if( Path.GetExtension(filePath).ToLower().Contains(".htm", StringComparison.OrdinalIgnoreCase) )
                    {
                        ProcessFile(filePath);
                        task.Increment(1);
                    }
                    else
                    {
                        Log.Debug("Skipping file: {FilePath}", filePath);
                        task.Increment(1);
                    }

                    await Task.Delay(1);
                }
            }
            catch( Exception ex )
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] An exception occurred while processing files: [yellow]{ex.Message}[/]");
                Log.Error(ex, "An exception occurred while processing files.");
                returnValue = 1;
            }
        });

        return returnValue;
    }

    private void ProcessFile(string filePath)
    {
        try
        {
            HtmlProcessor htmlProcessor = new HtmlProcessor(filePath);
            _storyName = htmlProcessor.GetStoryTitle().Trim();
            if( string.IsNullOrWhiteSpace(_storyName) )
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] Story title could not be determined from file: [yellow]{filePath}[/]");
                throw new GetTitleException($"Story title could not be determined from file: {filePath}");
            }
            _storyMetaData = new StoryMetaData
            {
                AuthorUrl = htmlProcessor.GetAuthorAddress()!.Address,
                Author = htmlProcessor.GetAuthorName(),
                StoryUrl = htmlProcessor.GetUrl(),
                Title = _storyName
            };

            var chapterName = htmlProcessor.GetChapterTitle();
            Log.Information("Chapter name: {ChapterName}", chapterName);

            var lines = htmlProcessor.GetChapterText();

            if( lines != null && lines.Count > 0 )
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
        catch( GetTitleException ex )
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] {ex.Message}");
            Log.Error(ex, "GetTitleException occurred while processing file: {FilePath}", filePath);
        }
        catch( Exception ex )
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] An exception occurred while processing file: [yellow]{filePath}[/] - [yellow]{ex.Message}[/]");
            Log.Error(ex, "An exception occurred while processing file: {FilePath}", filePath);
        }
    }

    private int ProcessFolder(string pathToFolder)
    {
        var returnValue = ConvertDirectory(pathToFolder).GetAwaiter().GetResult();
        if( !string.IsNullOrEmpty(_storyName) )
        {
            SaveTextFile(_storyName.Trim(), _storyMetaData);
            AnsiConsole.MarkupLine($"[green]Successfully created text file for story:[/] {_storyName}");
            AnsiConsole.MarkupLine("[green]================================================================================[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Story name could not be determined.");
            returnValue = 1;
        }

        return returnValue;
    }

    private Task<int> ProcessMultipleFolders(string[] folderPaths, string dateToProcess)
    {
        int returnValue = 0;
        string[] paths = folderPaths[0] == "." ? Directory.EnumerateDirectories("data").ToArray() : folderPaths;

        foreach( var path in paths )
        {
            var sourcePath = path.Contains("data") ? path : Path.Combine("data", path.Trim());
            if( ValidatePath(sourcePath, dateToProcess) )
            {
                Log.Information("Processing Folder: {FolderPath}", path);
                AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {path}\n");

                returnValue += ProcessFolder(sourcePath);
            }

            _chapterData.Clear();
        }
        return Task.FromResult(returnValue);
    }

    private void SaveTextFile(string fileName, StoryMetaData storyMetaData)
    {
        try
        {
            var finalName = fileName + ".txt";
            var finalPath = Path.Combine("output", finalName);

            if( !Directory.Exists("output") )
            {
                Directory.CreateDirectory("output");
            }

            using var outFile = File.CreateText(finalPath);

            outFile.WriteLine($"Title: {storyMetaData.Title}");
            outFile.WriteLine($"Author: {storyMetaData.Author} ({storyMetaData.AuthorUrl})");
            outFile.WriteLine($"Story URL: {storyMetaData.StoryUrl}");
            outFile.WriteLine();

            foreach( ChapterData chapterData in _chapterData.Values )
            {
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
        catch( Exception ex )
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] An exception occurred while saving text file: [yellow]{ex.Message}[/]");
            Log.Error(ex, "An exception occurred while saving text file.");
        }
    }

    protected override int Execute(CommandContext context, ConvertSettings settings, CancellationToken cancellationToken)
    {
        Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Verbose()
           .WriteTo.File("ffnbuild.log", rollingInterval: RollingInterval.Hour)
           .CreateLogger();
        Serilog.Debugging.SelfLog.Enable(Console.Error);

        int returnValue = 0;
        var dateToProcess = string.Empty;
        if( !string.IsNullOrWhiteSpace(settings.Date) )
        {
            if( settings.Date == "!" )
            {
                dateToProcess = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                dateToProcess = settings.Date;
            }
        }

        if( settings.SourcePath.Length == 0 )
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Source path(s) are invalid or do not exist.");
            Log.Debug("Source path(s) are invalid or do not exist.");

            return 1;
        }

        if( settings.SourcePath.Length > 1 || settings.SourcePath[0] == "." )
        {
            Log.Information("Processing multiple files:");
            returnValue = ProcessMultipleFolders(settings.SourcePath, dateToProcess).GetAwaiter().GetResult();
        }
        else if( Path.IsPathRooted(settings.SourcePath[0]) )
        {
            var sourcePath = settings.SourcePath[0];
            if( ValidatePath(sourcePath, dateToProcess) )
            {
                Log.Information("Processing from rooted source path: {SourcePath}", settings.SourcePath[0]);
                AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {settings.SourcePath[0]}\n");
                returnValue += ProcessFolder(sourcePath);
            }
        }
        else
        {
            var sourcePath = Path.Combine("data", settings.SourcePath[0]);
            if( ValidatePath(sourcePath, dateToProcess) )
            {
                Log.Information("Processing from non-rooted source path: {SourcePath}", settings.SourcePath);
                AnsiConsole.MarkupLine($"[green]Building project from source path:[/] {sourcePath}\n");
                returnValue += ProcessFolder(sourcePath);
            }
        }

        if( returnValue > 0 )
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Error(s) occurred during conversion process.\n");
            Log.Error("Error(s) occurred during conversion process.");
        }

        return returnValue;
    }
}