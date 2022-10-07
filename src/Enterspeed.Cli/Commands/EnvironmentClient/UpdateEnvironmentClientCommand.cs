using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Enterspeed.Cli.Commands.EnvironmentClient;

internal class UpdateEnvironmentClientCommand : Command
{
    public UpdateEnvironmentClientCommand() : base(name: "Update", "Update an environment client")
    {
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            return 0;
        }
    }
}