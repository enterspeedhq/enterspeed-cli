using System.CommandLine;

namespace Enterspeed.Cli.Commands.SourceEntity;

public static class SourceEntityCommands
{
    public static Command BuildCommands()
    {
        var command = new Command("source-entity", "Source entities")
        {
            new ListSourceEntitiesCommand()
        };
        return command;
    }
}
