using System.CommandLine;

namespace Enterspeed.Cli.Commands.View;

public static class ViewCommands
{
    public static Command[] BuildCommands()
    {
        return new[] { "views", "v" }.Select(commandName => new Command(commandName, "Generated views")
        {
            new ListViewsCommand()
        }).ToArray();
    }
}