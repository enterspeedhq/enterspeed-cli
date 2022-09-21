using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.EnvironmentClient;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;

namespace Enterspeed.Cli.Commands.EnvironmentClient;

public class ListEnvironmentClientsCommand : Command
{
    public ListEnvironmentClientsCommand() : base(name: "list", "List environment clients")
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
            var environmentClients = await _mediator.Send(new GetEnvironmentClientsRequest());

            _outputService.Write(environmentClients, Output);
            return 0;
        }
    }
}