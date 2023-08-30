using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Exceptions;
using Enterspeed.Cli.Extensions;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.FileService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.Schema
{
    internal class SaveSchemaCommand : Command
    {
        public SaveSchemaCommand() : base(name: "save", "Saves schema")
        {
            AddArgument(new Argument<string>("alias", "Alias of the schema"));
            AddOption(new Option<string>(new[] { "--file", "-f" }, "E.g. mySchemaAlias.json"));
        }

        public new class Handler : BaseCommandHandler, ICommandHandler
        {
            private readonly IMediator _mediator;
            private readonly IOutputService _outputService;
            private readonly ISchemaFileService _schemaFileService;
            private readonly ILogger<SaveSchemaCommand> _logger;

            public Handler(
                IMediator mediator,
                IOutputService outputService,
                ISchemaFileService schemaFileService,
                ILogger<SaveSchemaCommand> logger)
            {
                _mediator = mediator;
                _outputService = outputService;
                _schemaFileService = schemaFileService;
                _logger = logger;
            }

            public string Alias { get; set; }
            public string File { get; set; }

            public async Task<int> InvokeAsync(InvocationContext context)
            {
                if (string.IsNullOrEmpty(Alias))
                {
                    throw new ConsoleArgumentException("Please specify an alias for your schema");
                }

                var schema = _schemaFileService.GetSchema(Alias, File);
                if (schema.Content == null)
                {
                    _logger.LogError("Schema file not found!");
                    return 1;
                }

                // Get mapping schema guid 
                var schemas = await _mediator.Send(new QueryMappingSchemasRequest());
                var matchingSchema = schemas.FirstOrDefault(sc => sc.ViewHandle == Alias);

                // Get mapping schema version
                GetMappingSchemaResponse existingSchema = null;
                if (matchingSchema != null)
                {
                    existingSchema = await _mediator.Send(
                        new GetMappingSchemaRequest
                        {
                            MappingSchemaId = matchingSchema.Id.MappingSchemaGuid,
                        }
                    );
                }

                if (existingSchema == null)
                {
                    _logger.LogError("Schema not found!");
                    return 1;
                }

                // Create update schema request
                var updateSchemaResponse = await _mediator.Send(new UpdateMappingSchemaRequest
                {
                    Format = "json",
                    MappingSchemaId = existingSchema.Version.Id.MappingSchemaGuid,
                    Version = existingSchema.LatestVersion,
                    Schema = schema.GetSchemaContent()
                });

                var updatedSchema = await _mediator.Send(
                    new GetMappingSchemaRequest
                    {
                        MappingSchemaId = matchingSchema.Id.MappingSchemaGuid,
                    }
                );

                // Send response to CLI
                _outputService.Write($"Successfully updated schema : {Alias} Version: {updatedSchema.LatestVersion}");

                return 0;
            }
        }
    }
}