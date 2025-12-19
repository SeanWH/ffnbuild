namespace ffnbuild.cli;

using System.ComponentModel;

using Spectre.Console.Cli;

public class ConvertSettings : CommandSettings
{
    [CommandOption("--html")]
    [Description("Switch indicating html file generation.")]
    public bool? CreateHtml { get; set; } = false;

    [CommandArgument(1, "<sourcePath>")]
    [Description("The name of the folder containing the files to convert.  Multiple folders may be comma-separated.")]
    public string SourcePath { get; set; } = string.Empty;
}