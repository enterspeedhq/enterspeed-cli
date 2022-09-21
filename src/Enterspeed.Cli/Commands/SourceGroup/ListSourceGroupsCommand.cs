using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Api.SourceGroup;

namespace Enterspeed.Cli.Commands.SourceGroup;

public class ListSourceGroupsCommand : Command
{
    public ListSourceGroupsCommand() : base(name: "list", "List source groups")
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
            var sourceGroups = await _mediator.Send(new GetSourceGroupsRequest());

            _outputService.Write(sourceGroups, Output);
            return 0;
        }
    }
}