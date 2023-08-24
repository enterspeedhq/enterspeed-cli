using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.SourceEntity;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;

namespace Enterspeed.Cli.Commands.SourceEntity;

public class ListSourceEntitiesCommand : Command
{
    public ListSourceEntitiesCommand() : base(name: "list", "List source entities")
    {
        AddArgument(new Argument<string>("sourceId", "Id of the source") { Arity = ArgumentArity.ExactlyOne });
        AddOption(new Option<string>("--filter", "Filter on ID or Url"));
        AddOption(new Option<string>("--type", "Source Entity type"));
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

        public string SourceId { get; set; }
        public string Filter { get; set; }
        public string Type { get; set; }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var viewsResponse = await _mediator.Send(new QuerySourceEntitiesRequest
            {
                SourceId = Cli.Domain.Models.SourceId.Parse(Cli.Domain.Models.SourceId.From(SourceId)),
                Filter = Filter,
                Type = Type,
                PageSize = 10
            });

            _outputService.Write(viewsResponse.Results);
            return 0;
        }
    }
}