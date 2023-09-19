using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.Schema
{
    public class ListDeployedSchemasCommand : Command
    {
        public ListDeployedSchemasCommand() : base("list-deployed", "Lists all deployed schemas for the environment")
        {
            AddOption(new Option<string>(new[] { "--environment", "-e" }, "Environment name") { IsRequired = true });
        }

        public new class Handler : BaseCommandHandler, ICommandHandler
        {
            private readonly IMediator _mediator;
            private readonly IOutputService _outputService;
            private readonly ILogger<ListDeployedSchemasCommand> _logger;

            public Handler(IMediator mediator,
                IOutputService outputService,
                ILogger<ListDeployedSchemasCommand> logger)
            {
                _mediator = mediator;
                _outputService = outputService;
                _logger = logger;
            }

            public string Environment { get; set; }

            public async Task<int> InvokeAsync(InvocationContext context)
            {
                var environmentToDeployTo = await GetEnvironmentToDeployTo();
                if (environmentToDeployTo == null)
                {
                    _logger.LogError("Environment was not found");
                    return 1;
                }

                var schemas = await _mediator.Send(new GetMappingSchemasRequest()
                {
                    EnvironmentId = environmentToDeployTo.Id.EnvironmentGuid.ToString()
                });

                _outputService.Write(schemas.Current.Schemas);
                return 0;
            }

            private async Task<GetEnvironmentsResponse> GetEnvironmentToDeployTo()
            {
                var environments = await _mediator.Send(new GetEnvironmentsRequest());
                var environmentToDeployTo = environments.FirstOrDefault(e => e.Name == Environment);
                return environmentToDeployTo;
            }
        }
    }
}
