using System.CommandLine;

namespace Enterspeed.Cli.Commands.SourceGroup;

public static class SourceGroupCommands
{
    public static Command[] BuildCommands()
    {
        return new[] { "source-group", "sg" }.Select(commandName => new Command(commandName, "Source group")
        {
            new ListSourceGroupCommand(),
            new CreateSourceGroupCommand(),
            new UpdateSourceGroupCommand(),
            new DeleteSourceGroupCommand()
        }).ToArray();
    }
}