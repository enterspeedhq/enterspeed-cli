using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Extensions;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.FileService;
using Enterspeed.Cli.Services.FileService.Models;
using Enterspeed.Cli.Services.SchemaService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.Schema;

internal class ImportSchemaCommand : Command
{
    public ImportSchemaCommand() : base(name: "import",
        "Imports all schemas from the /schemas folder on the disk. Will create new schemas and update existing schemas if --override is enabled.")
    {
        AddOption(new Option<string>(new[] { "--schema-alias", "-a" }, "Provide a schema alias to only import a single schema"));
        AddOption(new Option<bool>(new[] { "--override", "-o" }, "Override existing schemas"));
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        private readonly ISchemaFileService _schemaFileService;
        private readonly ISchemaNameService _schemaNameService;
        private readonly ILogger<CreateSchemaCommand> _logger;

        public Handler(IMediator mediator,
            IOutputService outputService,
            ISchemaFileService schemaFileService,
            ILogger<CreateSchemaCommand> logger,
            ISchemaNameService schemaNameService)
        {
            _mediator = mediator;
            _outputService = outputService;
            _schemaFileService = schemaFileService;
            _logger = logger;
            _schemaNameService = schemaNameService;
        }

        public bool Override { get; set; }
        public string SchemaAlias { get; set; }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var existingSchemas = await _mediator.Send(new QueryMappingSchemasRequest());

            var schemaFiles = string.IsNullOrWhiteSpace(SchemaAlias)
                ? _schemaFileService.GetAllSchemas()
                : new List<SchemaFile> { _schemaFileService.GetSchema(SchemaAlias) };

            var successfulImports = 0;
            var failedImports = 0;
            foreach (var schemaFile in schemaFiles)
            {
                var matchingSchema = existingSchemas.FirstOrDefault(sc => sc.ViewHandle == schemaFile.Alias);
                var schemaExists = matchingSchema is not null;

                if (schemaExists && Override)
                {
                    if (await UpdateExistingSchema(schemaFile, matchingSchema))
                        successfulImports += 1;
                    else
                        failedImports += 1;
                }
                else if (!schemaExists)
                {
                    if (await CreateNewSchema(schemaFile))
                        successfulImports += 1;
                    else
                        failedImports += 1;
                }
            }

            _outputService.Write($"Successfully imported {successfulImports} schema(s).");

            if (failedImports > 0)
            {
                _outputService.Write($"Failed to imported {failedImports} schema(s).");
                return 1;
            }

            return 0;
        }

        private async Task<bool> UpdateExistingSchema(SchemaFile schemaFile, QueryMappingSchemaResponse matchingSchema)
        {
            var existingSchema = await _mediator.Send(
                new GetMappingSchemaRequest
                {
                    MappingSchemaId = matchingSchema.Id.MappingSchemaGuid
                }
            );

            if (existingSchema == null)
            {
                _logger.LogError("Schema not found!");
                return false;
            }

            var relativeDirectoryPathInEnterspeed = Path.GetDirectoryName(existingSchema.Name.TrimEnd(Path.DirectorySeparatorChar));
            var relativeDirectoryPathOnDisk = schemaFile.RelativeSchemaDirectory;

            if (!relativeDirectoryPathOnDisk.Equals(relativeDirectoryPathInEnterspeed))
            {
                await UpdateSchemaName(existingSchema, relativeDirectoryPathOnDisk);
            }

            var updateMappingSchemaVersionResponse = await _mediator.Send(new UpdateMappingSchemaVersionRequest
            {
                Format = schemaFile.Format,
                MappingSchemaId = matchingSchema.Id.MappingSchemaGuid,
                Version = existingSchema.LatestVersion,
                Schema = schemaFile.GetSchemaContent()
            });

            _outputService.Write($"Successfully updated schema: {schemaFile.Alias} Version: {updateMappingSchemaVersionResponse.Version}");

            return true;
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

        /// <summary>
        /// As the management API does not support upserts we first creates the empty schema and then updates the schema with content
        /// </summary>
        private async Task<bool> CreateNewSchema(SchemaFile schemaFile)
        {
            var name = _schemaNameService.GetSchemaName(schemaFile);
            var createSchemaResponse = await _mediator.Send(new CreateMappingSchemaRequest
            {
                Name = name,
                ViewHandle = schemaFile.Alias,
                Type = schemaFile.SchemaType.ToApiString(),
                Format = schemaFile.Format
            });

            if (createSchemaResponse?.IdValue is null || string.IsNullOrEmpty(createSchemaResponse.MappingSchemaGuid))
            {
                _logger.LogError($"Could not create schema: {schemaFile.Alias}");
                return false;
            }

            _outputService.Write("Successfully created new schema: " + schemaFile.Alias);

            var updateSchemaResponse = await _mediator.Send(new UpdateMappingSchemaVersionRequest
            {
                Format = schemaFile.Format,
                MappingSchemaId = createSchemaResponse.MappingSchemaGuid,
                Version = createSchemaResponse.Version,
                Schema = schemaFile.GetSchemaContent()
            });

            _outputService.Write($"Successfully updated schema: {schemaFile.Alias} Version: {updateSchemaResponse.Version}");

            return true;
        }
    }
}