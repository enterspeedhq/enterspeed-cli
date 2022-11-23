using System.CommandLine;

namespace Enterspeed.Cli.Commands.Source;

public static class SourceCommands
{
    public static Command BuildCommands()
    {
        var command = new Command("source", "Source")
        {
            new CreateSourceCommand(),
            new UpdateSourceCommand(),
            new DeleteSourceCommand()
        };
        return command;
    }
}
