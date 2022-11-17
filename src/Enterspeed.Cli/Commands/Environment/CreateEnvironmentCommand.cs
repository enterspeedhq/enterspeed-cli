using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Environment;

namespace Enterspeed.Cli.Commands.Environment;

internal class CreateEnvironmentCommand : Command
{
    public CreateEnvironmentCommand() : base(name: "create", "Create environment")
    {
        AddOption(new Option<string>(new[] { "--name", "-n" }, "Name of environment") { IsRequired = true });
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;

        public string Name { get; set; }

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var domain = await _mediator.Send(new CreateEnvironmentRequest
            {
                Name = Name
            });

            _outputService.Write(domain);

            return 0;
        }
    }
}