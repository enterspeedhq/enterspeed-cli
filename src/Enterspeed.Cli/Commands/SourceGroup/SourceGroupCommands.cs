using System.CommandLine;

namespace Enterspeed.Cli.Commands.SourceGroup;

public static class SourceGroupCommands
{
    public static Command BuildCommands()
    {
        var command = new Command("source-group", "Source group")
        {
            new ListSourceGroupCommand(),
            new CreateSourceGroupCommand(),
            new UpdateSourceGroupCommand(),
            new DeleteSourceGroupCommand()
        };
        command.AddAlias("sg");
        return command;
    }
}