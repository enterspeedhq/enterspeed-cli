using Enterspeed.Cli.Services.StateService;
using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Api.SourceGroup;
using System.Text.Json;
using System;
using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Api.Schema;
using MediatR;

namespace Enterspeed.Cli.Commands.Deploy
{
    public class DeployCommand : Command
    {
        public DeployCommand() : base(name: "deploy", "Deploy schemas using deploymentplan")
        {
            AddOption(new Option<string>(new[] {"--environment", "-e"}, "Target environment for deploy")
                {IsRequired = true});
            AddOption(new Option<string>(new[] {"--deploymentplan", "-dp"}, "Deploymentplan to use"));
        }

        public new class Handler : BaseCommandHandler, ICommandHandler
        {
            private readonly IMediator _mediator;
            private readonly ILogger<DeployCommand> _logger;

            public Handler(IMediator mediator, ILogger<DeployCommand> logger)
            {
                _mediator = mediator;
                _logger = logger;
            }

            public string Environment { get; set; }
            public string DeploymentPlan { get; set; } = "deploymentplan.json";

            public async Task<int> InvokeAsync(IMediator mediator, InvocationContext context)
            {
                _logger.LogInformation($"Deploy {DeploymentPlan} to {Environment}");

                // Read deployment plan file
                var plan = ReadDeploymentPlanFile(DeploymentPlan);

                // Resolve environment alias
                var environments = await _mediator.Send(new GetEnvironmentsRequest());
                var targetEnvironment = environments.FirstOrDefault(env => env.Name == Environment);
                if (targetEnvironment == null)
                {
                    _logger.LogError($"Environment {Environment} not found!");
                    return 1;
                }

                // Resolve schema alias
                foreach (var schema in plan.Schemas)
                {
                    var schemas = await _mediator.Send(
                        new GetSchemaRequest({SchemaGuid = schema.}));

                }


                // Check if schemas are locked 

                // Execute deployment

                return 0;
            }

            private DeploymentPlan ReadDeploymentPlanFile(string filename)
            {
                var json = File.ReadAllText(filename);
                return JsonSerializer.Deserialize<DeploymentPlan>(json);
            }
        }
    }
}
