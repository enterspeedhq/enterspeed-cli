using System.CommandLine;

namespace Enterspeed.Cli.Commands.Environment;

public static class EnvironmentCommands
{
    public static Command BuildCommands()
    {
        var command = new Command("environment", "Environment")
        {
            new GetEnvironmentCommand(),
            new ListEnvironmentsCommand(),
            new CreateEnvironmentCommand(),
            new UpdateEnvironmentCommand(),
            new DeleteEnvironmentCommand()
        };
        command.AddAlias("e");
        return command;
    }
}