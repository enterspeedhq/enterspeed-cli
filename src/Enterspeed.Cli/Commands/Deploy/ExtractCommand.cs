using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Services.FileService;
using MediatR;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Api.Environment;

namespace Enterspeed.Cli.Commands.Deploy
{
    public class ExtractCommand : Command
    {
        public ExtractCommand() : base(name: "extract", "Creates deploymentplan file, based on schemas and versions currently deployed in specified environment")
        {
            AddOption(new Option<string>(new[] { "--environment", "-e" }, "Target environment for extract") { IsRequired = true });
        }

        public new class Handler : BaseCommandHandler, ICommandHandler
        {
            private readonly IMediator _mediator;
            private readonly IDeploymentPlanFileService _deploymentPlanFileService;
            private readonly ILogger<DeployCommand> _logger;
            private readonly ISchemaFileService _schemaFileService;

            public Handler(IMediator mediator,
                ILogger<DeployCommand> logger,
                IDeploymentPlanFileService deploymentPlanFileService,
                ISchemaFileService schemaFileService)
            {
                _mediator = mediator;
                _logger = logger;
                _deploymentPlanFileService = deploymentPlanFileService;
                _schemaFileService = schemaFileService;
            }

            public string Environment { get; set; }

            public async Task<int> InvokeAsync(InvocationContext context)
            {
                var schemaResponses = new List<GetMappingSchemaResponse>();
                var allEnvironments = await _mediator.Send(new GetEnvironmentsRequest());

                var environment = allEnvironments.FirstOrDefault(e => e.Name == Environment);
                if (environment == null)
                {
                    _logger.LogError($"Environment with name {Environment} could not be found on tenant");
                    return 1;
                }

                _deploymentPlanFileService.DeleteDeploymentPlan();

                var schemas = await _mediator.Send(new QueryMappingSchemasRequest());
                foreach (var schema in schemas)
                {
                    var schemaResponse = await _mediator.Send(new GetMappingSchemaRequest()
                    {
                        MappingSchemaId = schema.Id.MappingSchemaGuid
                    });

                    // Get deployment and check if latest version exists in environment
                    var deployment = GetDeployment(environment, schemaResponse);
                    if (deployment == null)
                    {
                        _logger.LogInformation($"No deployments for {schemaResponse.ViewHandle}");
                        continue;
                    }

                    if (IsLatestVersion(schemaResponse, deployment))
                    {
                        ExtractSchema(schemaResponse);
                    }

                    // Try and get schema based on version 
                    var schemaResponseByVersion = await _mediator.Send(new GetMappingSchemaRequest()
                    {
                        MappingSchemaId = schema.Id.MappingSchemaGuid,
                        Version = deployment.Version
                    });

                    if (schemaResponseByVersion != null)
                    {
                        ExtractSchema(schemaResponseByVersion);
                    }
                }

                return 0;
            }

            private void ExtractSchema(GetMappingSchemaResponse schemaResponse)
            {
                // Add even though we dont have the schema locally.To support using older versions of schemas.
                if (!_schemaFileService.SchemaExists(schemaResponse.ViewHandle))
                {
                    _deploymentPlanFileService.UpdateDeploymentPlan(schemaResponse.ViewHandle, schemaResponse.Version.Id.Version);
                }
                if (_schemaFileService.SchemaValid(schemaResponse.Version.Data, schemaResponse.ViewHandle))
                {
                    _deploymentPlanFileService.UpdateDeploymentPlan(schemaResponse.ViewHandle, schemaResponse.Version.Id.Version);
                    _logger.LogInformation($"Successfully extracted {schemaResponse.ViewHandle} and added it to the deployment plan");
                }
                else
                {
                    _logger.LogWarning($"Use schema deploy for {schemaResponse.ViewHandle}, to update deploymentplan with versions on disk");
                }
            }

            private bool IsLatestVersion(GetMappingSchemaResponse schemaResponse, DeploymentResponse deployment)
            {
                return deployment?.Version == schemaResponse?.Version.Id.Version;
            }

            private DeploymentResponse GetDeployment(GetEnvironmentsResponse environment, GetMappingSchemaResponse schemaResponse)
            {
                return schemaResponse?.Deployments.OrderByDescending(d => d.Version).FirstOrDefault(d => d.EnvironmentId == environment.Id.IdValue);
            }
        }
    }
}
