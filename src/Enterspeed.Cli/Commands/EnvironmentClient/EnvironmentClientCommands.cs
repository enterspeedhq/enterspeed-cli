using System.CommandLine;

namespace Enterspeed.Cli.Commands.EnvironmentClient;

public static class EnvironmentClientCommands
{
    public static Command[] BuildCommands()
    {
        return new[] { "environment-client", "ec" }.Select(commandName => new Command(commandName, "Environment client")
        {
            new GetEnvironmentClientCommand(),
            new ListEnvironmentClientsCommand(),
            new CreateEnvironmentClientCommand(),
            new UpdateEnvironmentClientCommand(),
            new DeleteEnvironmentClientCommand()
        }).ToArray();
    }
}