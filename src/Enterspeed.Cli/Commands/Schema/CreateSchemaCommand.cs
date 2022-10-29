using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Schema;
using Enterspeed.Cli.Exceptions;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;

namespace Enterspeed.Cli.Commands.Schema;

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

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator;
            _outputService = outputService;
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
            
            _outputService.Write(createSchemaResponse);
            return 0;
        }
    }
}
