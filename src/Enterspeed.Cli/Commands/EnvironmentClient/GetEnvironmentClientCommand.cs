using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Environment;

namespace Enterspeed.Cli.Commands.EnvironmentClient;

internal class GetEnvironmentClientCommand : Command
{
    public GetEnvironmentClientCommand() : base(name: "get", "Get an environment client")
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
            var environments = await _mediator.Send(new GetEnvironmentsRequest());

            _outputService.Write(environments);
            return 0;
        }
    }
}