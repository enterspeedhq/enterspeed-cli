using System.CommandLine;

namespace Enterspeed.Cli.Commands.Environment;

public static class EnvironmentCommands
{
    public static Command[] BuildCommands()
    {
        return new[] { "environment", "e" }.Select(commandName => new Command(commandName, "Environment")
        {
            new GetEnvironmentCommand(),
            new ListEnvironmentsCommand(),
            new CreateEnvironmentCommand(),
            new UpdateEnvironmentCommand(),
            new DeleteEnvironmentCommand()
        }).ToArray();
    }
}