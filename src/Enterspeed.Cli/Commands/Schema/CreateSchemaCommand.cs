using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Constants;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Exceptions;
using Enterspeed.Cli.Extensions;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.FileService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.Schema
{
    internal class CreateSchemaCommand : Command
    {
        public CreateSchemaCommand() : base(name: "create", "Creates schema")
        {
            AddArgument(new Argument<string>("alias", "Alias of the schema"));
            AddOption(new Option<string>(new[] { "--type", "-t" }, "Type of the schema (full or partial). Default value is full"));
            AddOption(new Option<string>(new[] { "--name", "-n" }, "Name of the schema"));
            AddOption(new Option<string>(new[] { "--format", "-f" }, "Format of the schema (json or javascript). Default is json"));
        }

        public new class Handler : BaseCommandHandler, ICommandHandler
        {
            private readonly IMediator _mediator;
            private readonly IOutputService _outputService;
            private readonly ISchemaFileService _schemaFileService;
            private readonly ILogger<CreateSchemaCommand> _logger;

            public Handler(IMediator mediator,
                IOutputService outputService,
                ISchemaFileService schemaFileService,
                ILogger<CreateSchemaCommand> logger)
            {
                _mediator = mediator;
                _outputService = outputService;
                _schemaFileService = schemaFileService;
                _logger = logger;
            }

            public string Alias { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Format { get; set; }

            public async Task<int> InvokeAsync(InvocationContext context)
            {
                if (string.IsNullOrEmpty(Alias))
                {
                    throw new ConsoleArgumentException("Please specify an alias for your schema");
                }

                var schemaType = GetSchemaType();
                if (!schemaType.HasValue)
                {
                    throw new ConsoleArgumentException("Please specify a valid value for type");
                }

                if (string.IsNullOrEmpty(Format))
                {
                    Format = SchemaConstants.JsonFormat;
                }

                var createSchemaResponse = await _mediator.Send(new CreateMappingSchemaRequest
                {
                    Name = Name ?? Alias,
                    ViewHandle = Alias,
                    Type = schemaType.Value.ToApiString(),
                    Format = Format
                });


                if (createSchemaResponse?.IdValue != null && !string.IsNullOrEmpty(createSchemaResponse.MappingSchemaGuid))
                {
                    var schemaResponse = await _mediator.Send(new GetMappingSchemaRequest()
                    {
                        MappingSchemaId = createSchemaResponse.MappingSchemaGuid
                    });

                    _schemaFileService.CreateSchema(Alias, schemaType.Value, schemaResponse.Version);
                }
                else
                {
                    _logger.LogError($"Could not create schema {Alias}");
                    return 1;
                }

                if (schemaType == SchemaType.Partial)
                {
                    await UpdateSchemaToPartialTemplate(createSchemaResponse.MappingSchemaGuid);
                }

                _outputService.Write("Successfully created new schema : " + Alias);

                return 0;
            }

            // Maybe this should be handled in the management API see shortcut story: 3226
            private async Task UpdateSchemaToPartialTemplate(string mappingSchemaGuid)
            {
                var schema = _schemaFileService.GetSchema(Alias);

                var updateSchemaResponse = await _mediator.Send(new UpdateMappingSchemaRequest
                {
                    Format = schema.Format,
                    MappingSchemaId = mappingSchemaGuid,
                    Version = 1,
                    Schema = schema.GetSchemaContent()
                });
            }

            // We are using the terms "full" as public facing term but "normal" inside the code base, so we have to do some manual mapping
            private SchemaType? GetSchemaType()
            {
                if (string.IsNullOrWhiteSpace(Type) || Type.Equals("full", StringComparison.InvariantCultureIgnoreCase))
                {
                    return SchemaType.Normal;
                }

                if (Enum.TryParse(Type, true, out SchemaType schemaType))
                {
                    return schemaType;
                }

                return null;
            }
        }
    }
}