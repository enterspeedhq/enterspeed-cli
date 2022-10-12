using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Domain;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;

namespace Enterspeed.Cli.Commands.Domain;

public class ListDomainsCommand : Command
{
    public ListDomainsCommand() : base(name: "list", "List domains")
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
            var domains = await _mediator.Send(new GetDomainsRequest());

            _outputService.Write(domains);
            return 0;
        }
    }
}