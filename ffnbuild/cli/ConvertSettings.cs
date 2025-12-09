namespace ffnbuild.cli;

using System.ComponentModel;

using Spectre.Console.Cli;

public class ConvertSettings : CommandSettings
{
    [CommandArgument(0, "<sourcePath>")]
    [Description("The name of the folder containing the files to convert.  Multiple folders may be comma-separated.")]
    public string SourcePath { get; set; } = string.Empty;
}