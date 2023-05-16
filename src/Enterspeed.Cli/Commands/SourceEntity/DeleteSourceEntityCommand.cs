using Enterspeed.Cli.Api.SourceGroup;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.IngestService;
using MediatR;
using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.CommandLine;

namespace Enterspeed.Cli.Commands.SourceEntity;

public class DeleteSourceEntitiesCommand : Command
{
    public DeleteSourceEntitiesCommand() : base(name: "delete", "Delete source entity")
    {
        AddArgument(new Argument<string>("id", "Id of the source entity to delete") { Arity = ArgumentArity.ExactlyOne });
        AddOption(new Option<string>(new[] { "--sourceId", "-s" }, "Id of the source") { Arity = ArgumentArity.ExactlyOne });
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        private readonly IIngestService _ingestService;
        private readonly ILogger<DeleteSourceEntitiesCommand> _logger;

        public Handler(IMediator mediator, IOutputService outputService, IIngestService ingestService, ILogger<DeleteSourceEntitiesCommand> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
            _ingestService = ingestService;
            _logger = logger;
        }

        public string SourceId { get; set; }
        public string Id { get; set; }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            // Find source
            var sourceGroups = await _mediator.Send(new GetSourceGroupsRequest());
            var source = sourceGroups.SelectMany(sg => sg.Sources).FirstOrDefault(s => s.Source.Id.SourceGuid == SourceId)?.Source;
            if (source == null)
            {
                _logger.LogError($"Source: {SourceId} not found");
                return 1;
            }

            var deleteResult = await _ingestService.Delete(Id, source.AccessKey);
            return deleteResult ? 0 : 1;
        }
    }
}
