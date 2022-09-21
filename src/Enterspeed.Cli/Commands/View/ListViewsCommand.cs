using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.View;

namespace Enterspeed.Cli.Commands.View;

public class ListViewsCommand : Command
{
    public ListViewsCommand() : base(name: "list", "List views")
    {
        AddArgument(new Argument<string>("EnvironmentId", "Id of the environment") { Arity = ArgumentArity.ExactlyOne });
        AddOption(new Option<string>("--SourceId", "Source ID"));
        AddOption(new Option<string>("--SchemaAlias", "Schema Alias"));
        AddOption(new Option<string>("--OriginId", "Source Entity Origin ID"));
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

        public string EnvironmentId { get; set; }
        public string SourceId { get; set; }
        public string SchemaAlias { get; set; }
        public string OriginId { get; set; }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var viewsResponse = await _mediator.Send(new QueryViewsRequest
            {
                EnvironmentId = Cli.Domain.Models.EnvironmentId.Parse(Cli.Domain.Models.EnvironmentId.From(EnvironmentId)),
                SourceId = !string.IsNullOrEmpty(SourceId) ? Cli.Domain.Models.SourceId.Parse(Cli.Domain.Models.SourceId.From(SourceId)) : null,
                SchemaAlias = SchemaAlias,
                SourceEntityOriginId = OriginId,
                PageSize = 10
                    
            });

            _outputService.Write(viewsResponse.Results, Output);
            return 0;
        }
    }
}