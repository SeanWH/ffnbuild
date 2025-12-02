namespace ffnbuild.cli;

using System.ComponentModel;

using Spectre.Console.Cli;

public class ConvertSettings : CommandSettings
{
    [CommandArgument(0, "<sourcePath>")]
    [Description("The path to the source directory to convert from.")]
    public string SourcePath { get; set; } = string.Empty;
}