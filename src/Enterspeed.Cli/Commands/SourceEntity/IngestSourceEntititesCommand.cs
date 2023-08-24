using Enterspeed.Cli.Api.SourceGroup;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Services.IngestService;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.SourceEntity
{
    public class IngestSourceEntitiesCommand : Command
    {
        public IngestSourceEntitiesCommand() : base(name: "ingest", "Ingest source entities")
        {
            AddArgument(new Argument<string>("filePath", "File or path to ingest") {Arity = ArgumentArity.ExactlyOne});
            AddOption(new Option<string>(new [] { "--sourceId", "-s" }, "Id of the source") { Arity = ArgumentArity.ExactlyOne });
            AddOption(new Option<bool>( new [] { "--filenameAsId", "-fid" }, "Use filename as id"));
        }

        public new class Handler : BaseCommandHandler, ICommandHandler
        {
            private readonly IMediator _mediator;
            private readonly IOutputService _outputService;
            private readonly IIngestService _ingestService;
            private readonly ILogger<IngestSourceEntitiesCommand> _logger;

            public Handler(IMediator mediator, IOutputService outputService, IIngestService ingestService, ILogger<IngestSourceEntitiesCommand> logger)
            {
                _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
                _outputService = outputService;
                _ingestService = ingestService;
                _logger = logger;
            }

            public string SourceId { get; set; }
            public string FilePath { get; set; }
            public bool FilenameAsId { get; set; } = false;

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

                await _ingestService.Ingest(FilePath, source.AccessKey, FilenameAsId);

                return 0;
            }
        }
    }

   
}
