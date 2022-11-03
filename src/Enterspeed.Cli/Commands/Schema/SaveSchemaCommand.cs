using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Exceptions;
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
            AddArgument(new Argument<string>("alias", "Alias of the schema") { });
            AddOption(new Option<string>(new[] { "--file", "-f" }, "E.g. mySchemaAlias.json"));
        }

        public new class Handler : BaseCommandHandler, ICommandHandler
        {
            private readonly IMediator _mediator;
            private readonly IOutputService _outputService;
            private readonly IFileService _fileService;
            private readonly ILogger<SaveSchemaCommand> _logger;

            public Handler(IMediator mediator, IOutputService outputService, IFileService fileService, ILogger<SaveSchemaCommand> logger)
            {
                _mediator = mediator;
                _outputService = outputService;
                _fileService = fileService;
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

                var schema = _fileService.GetSchema(Alias, File);

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

                // Validate
                // TODO : Management API being prepared for this currently. 

                // Create update schema request
                var updateSchemaResponse = await _mediator.Send(new UpdateMappingSchemaRequest()
                {
                    Format = "json",
                    MappingSchemaId = existingSchema.Version.Id.MappingSchemaGuid,
                    Version = existingSchema.LatestVersion,
                    Schema = schema
                });

                // Save to deployment file



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