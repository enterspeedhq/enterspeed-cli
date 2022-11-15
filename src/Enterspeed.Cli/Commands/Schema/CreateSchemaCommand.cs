﻿using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Exceptions;
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
            AddArgument(new Argument<string>("alias", "Alias of the schema") { });
            AddOption(new Option<string>(new[] { "--name", "-n" }, "Name of the schema"));
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

            public async Task<int> InvokeAsync(InvocationContext context)
            {
                if (string.IsNullOrEmpty(Alias))
                {
                    throw new ConsoleArgumentException("Please specify an alias for your schema");
                }

                var createSchemaResponse = await _mediator.Send(new CreateMappingSchemaRequest()
                {
                    Name = Name ?? Alias,
                    ViewHandle = Alias
                });

                if (createSchemaResponse?.IdValue != null && !string.IsNullOrEmpty(createSchemaResponse.MappingSchemaGuid))
                {
                    _schemaFileService.CreateSchema(Alias);
                }
                else
                {
                    _logger.LogError($"Could not create schema {Alias}");
                    return 1;
                }

                _outputService.Write("Successfully created new schema : " + Alias);

                return 0;
            }
        }
    }
}