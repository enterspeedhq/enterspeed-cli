﻿using System.CommandLine;

namespace Enterspeed.Cli.Commands.Schema
{
    public class SchemaCommands
    {
        public static Command BuildCommands()
        {
            var command = new Command("schema", "Schemas")
            {
                new CreateSchemaCommand(),
                new SaveSchemaCommand(),
                new DeploySchemaCommand(),
                new CloneSchemaCommand()
            };

            return command;
        }
    }
}
