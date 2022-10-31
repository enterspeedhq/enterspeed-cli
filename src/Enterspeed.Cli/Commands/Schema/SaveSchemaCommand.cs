using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Exceptions;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.FileService;
using Enterspeed.Cli.Services.FileService.Models;
using MediatR;

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

            public Handler(IMediator mediator, IOutputService outputService, IFileService fileService)
            {
                _mediator = mediator;
                _outputService = outputService;
                _fileService = fileService;
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
                var matchingSchemaFromEnterspeed = schemas.Results.FirstOrDefault(sc => sc.ViewHandle == Alias);

                // Get mapping schema version
                

                // Validate
                // TODO : Management API being prepared for this currently. 

                // Create update schema request
                var updateSchemaResponse = await _mediator.Send(new UpdateMappingSchemaRequest()
                {
                    Format = "string",
                    MappingSchemaId = matchingSchemaFromEnterspeed?.Id.MappingSchemaGuid,
                    Version = 1, // TODO: Get version from a valid place.
                    Schema = JsonSerializer.Serialize(schema)
                });

                // Save to deployment file

                // Send response to CLI
                _outputService.Write("Successfully updated schema : " + Alias);

                return 0;
            }
        }
    }
}