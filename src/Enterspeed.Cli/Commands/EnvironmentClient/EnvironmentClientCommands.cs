using System.CommandLine;

namespace Enterspeed.Cli.Commands.EnvironmentClient;

public static class EnvironmentClientCommands
{
    public static Command BuildCommands()
    {
        var command = new Command("environment-client", "Environment client")
        {
            new GetEnvironmentClientCommand(),
            new ListEnvironmentClientsCommand(),
            new CreateEnvironmentClientCommand(),
            new UpdateEnvironmentClientCommand(),
            new DeleteEnvironmentClientCommand()
        };
        command.AddAlias("ec");
        return command;
    }
}