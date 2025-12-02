using Spectre.Console;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;
Console.Title = "FFN Build Tool";

AnsiConsole.Write(new FigletText("FFN Build Tool").Centered().Color(Color.Orange1));

var app = new Spectre.Console.Cli.CommandApp<ffnbuild.cli.ConvertCommand>();
return app.Run(args);