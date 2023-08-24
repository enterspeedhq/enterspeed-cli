using System.CommandLine;

namespace Enterspeed.Cli.Commands.Schema
{
    public class SchemaCommands
    {
        public static Command[] BuildCommands()
        {
            return new[] { "schema", "sc" }.Select(commandName => new Command(commandName, "Schemas")
            {
                new CreateSchemaCommand(),
                new SaveSchemaCommand(),
                new DeploySchemaCommand(),
                new CloneSchemaCommand(),
                new ImportSchemaCommand(),
                new ListDeployedSchemasCommand()
            }).ToArray();
        }
    }
}