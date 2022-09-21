using System.CommandLine;

namespace Enterspeed.Cli.Commands.SourceGroup;

public static class SourceGroupCommands
{
    public static Command BuildCommands()
    {
        var command = new Command("source-groups", "Source groups")
        {
            new ListSourceGroupsCommand()
        };
        return command;
    }
}