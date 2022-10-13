using System.CommandLine;

namespace Enterspeed.Cli.Commands.View;

public static class ViewCommands
{
    public static Command BuildCommands()
    {
        var command = new Command("views", "Generated views")
        {
            new ListViewsCommand()
        };
        return command;
    }
}