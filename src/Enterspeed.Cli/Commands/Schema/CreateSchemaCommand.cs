using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Schema;
using Enterspeed.Cli.Exceptions;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.FileService;
using MediatR;

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
            private readonly IFileService _fileService;

            public Handler(IMediator mediator, IOutputService outputService, IFileService fileService)
            {
                _mediator = mediator;
                _outputService = outputService;
                _fileService = fileService;
            }

            public string Alias { get; set; }
            public string Name { get; set; }

            public async Task<int> InvokeAsync(InvocationContext context)
            {
                if (string.IsNullOrEmpty(Alias))
                {
                    throw new ConsoleArgumentException("Please specify an alias for your schema");
                }

                var createSchemaResponse = await _mediator.Send(new CreateSchemaRequest()
                {
                    Name = Name ?? Alias,
                    ViewHandle = Alias
                });

                if (createSchemaResponse.IdValue != null && !string.IsNullOrEmpty(createSchemaResponse.MappingSchemaGuid))
                {
                    _fileService.CreateSchema(Alias, createSchemaResponse.Version, createSchemaResponse.MappingSchemaGuid);
                }
                else
                {
                    throw new ArgumentNullException("MappingSchemaGuid", "Could not create schema. Something went wrong.");
                }

                _outputService.Write("Successfully created new schema : " + Alias);

                return 0;
            }
        }
    }
}