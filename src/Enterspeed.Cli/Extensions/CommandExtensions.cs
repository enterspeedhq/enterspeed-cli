using System.CommandLine;

namespace Enterspeed.Cli.Extensions;

public static class CommandExtensions
{
    public static void AddCommands(this Command command, IEnumerable<Command> subCommands)
    {
        foreach (var subCommand in subCommands)
        {
            command.AddCommand(subCommand);
        }
    }
}