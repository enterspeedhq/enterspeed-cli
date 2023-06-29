using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.Schema;

public class ShowDeployedSchemasCommand : Command
{
    public ShowDeployedSchemasCommand() : base("showDeployed", "Shows all deployed schemas for the environment")
    {
        AddOption(new Option<string>(new[] { "--environment", "-e" }, "Environment name"));
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        private readonly ILogger<ShowDeployedSchemasCommand> _logger;

        public Handler(IMediator mediator,
            IOutputService outputService,
            ILogger<ShowDeployedSchemasCommand> logger)
        {
            _mediator = mediator;
            _outputService = outputService;
            _logger = logger;
        }

        public string Environment { get; set; }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var environmentToDeployTo = await GetEnvironmentToDeployTo(Environment, _mediator);
            if (environmentToDeployTo == null)
            {
                _logger.LogError("Environment to deploy to was not found");
                return 1;
            }

            var schemas = await _mediator.Send(new GetMappingSchemasRequest()
            {
                EnvironmentId = environmentToDeployTo.Id.EnvironmentGuid.ToString()
            });

            _outputService.Write(schemas);
            return 0;
        }
    }
}