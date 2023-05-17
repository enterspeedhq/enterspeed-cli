using Enterspeed.Cli.Api.SourceGroup;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.IngestService;
using MediatR;
using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Domain.Models;

namespace Enterspeed.Cli.Commands.SourceEntity;

public class DeleteSourceEntitiesCommand : Command
{
    public DeleteSourceEntitiesCommand() : base(name: "delete", "Delete source entity")
    {
        AddArgument(new Argument<string>("id", "Id of the source entity to delete") { Arity = ArgumentArity.ExactlyOne });
        AddOption(new Option<string>(new[] { "--sourceId", "-s" }, "Id of the source, if not using full source entity id") { Arity = ArgumentArity.ZeroOrOne });
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
            // Check if Id is a full source entity Id
            if (SourceEntityId.TryParse(Id, out SourceEntityId fullId))
            {
                SourceId = fullId.SourceGuid;
                Id = fullId.OriginId;
            }

            if (string.IsNullOrEmpty(SourceId))
            {
                _logger.LogError("--sourceId is required if not specified in source entity id");
                return 1;
            }
            
            var source = await GetSource(SourceId);
            if (source == null)
            {
                _logger.LogError($"Source: {SourceId} not found");
                return 1;
            }

            var deleteResult = await _ingestService.Delete(Id, source.AccessKey);
            return deleteResult ? 0 : 1;
        }

        private async Task<Api.SourceGroup.Source> GetSource(string sourceGuid)
        {
            var sourceGroups = await _mediator.Send(new GetSourceGroupsRequest());
            return sourceGroups.SelectMany(sg => sg.Sources).FirstOrDefault(s => s.Source.Id.SourceGuid == sourceGuid)?.Source;
        }
    }
}
