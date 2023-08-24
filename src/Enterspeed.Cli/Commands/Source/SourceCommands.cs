using System.CommandLine;

namespace Enterspeed.Cli.Commands.Source;

public static class SourceCommands
{
    public static Command[] BuildCommands()
    {
        return new[] { "source", "s" }.Select(commandName => new Command(commandName, "Source")
        {
            new CreateSourceCommand(),
            new UpdateSourceCommand(),
            new DeleteSourceCommand()
        }).ToArray();
    }
}