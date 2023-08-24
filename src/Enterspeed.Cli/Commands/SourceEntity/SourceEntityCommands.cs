using System.CommandLine;

namespace Enterspeed.Cli.Commands.SourceEntity;

public static class SourceEntityCommands
{
    public static Command[] BuildCommands()
    {
        return new[] { "source-entity", "se" }.Select(commandName => new Command(commandName, "Source entities")
        {
            new ListSourceEntitiesCommand(), 
            new IngestSourceEntitiesCommand(),
            new DeleteSourceEntitiesCommand()
        }).ToArray();
    }
}