using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Exceptions;
using Enterspeed.Cli.Extensions;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.FileService;
using Enterspeed.Cli.Services.SchemaService;
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
            private readonly ISchemaNameService _schemaNameService;
            private readonly ILogger<SaveSchemaCommand> _logger;

            public Handler(
                IMediator mediator,
                IOutputService outputService,
                ISchemaFileService schemaFileService,
                ILogger<SaveSchemaCommand> logger,
                ISchemaNameService schemaNameService)
            {
                _mediator = mediator;
                _outputService = outputService;
                _schemaFileService = schemaFileService;
                _logger = logger;
                _schemaNameService = schemaNameService;
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

                var relativeDirectoryPathInEnterspeed = Path.GetDirectoryName(existingSchema.Name.TrimEnd('/'));
                var relativeDirectoryPathOnDisk = schema.RelativeSchemaDirectory;

                if (!relativeDirectoryPathOnDisk.Equals(relativeDirectoryPathInEnterspeed))
                {
                    await UpdateSchemaName(existingSchema, relativeDirectoryPathOnDisk);
                }

                var updateMappingSchemaVersionRequest = new UpdateMappingSchemaVersionRequest
                {
                    Format = existingSchema.Version.Format,
                    MappingSchemaId = existingSchema.Version.Id.MappingSchemaGuid,
                    Version = existingSchema.LatestVersion,
                    Schema = schema.GetSchemaContent()
                };

                // Create update schema request
                var updateMappingSchemaVersionResponse = await _mediator.Send(updateMappingSchemaVersionRequest);

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

            private async Task UpdateSchemaName(GetMappingSchemaResponse existingSchema, string relativeDirectoryPathOnDisk)
            {
                var name = _schemaNameService.BuildNewSchemaName(existingSchema.Name, relativeDirectoryPathOnDisk);
                var updateSchemaResponse = await _mediator.Send(new UpdateMappingSchemaRequest()
                {
                    Name = name,
                    MappingSchemaId = existingSchema.Version.Id.MappingSchemaGuid,
                });

                _outputService.Write($"Successfully updated name to: {name}");
            }
        }
    }
}