using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Services.FileService;
using MediatR;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.FileService.Models;
using Enterspeed.Cli.Api.Release;
using System.Text.Json;

namespace Enterspeed.Cli.Commands.Deploy;

public class DeployCommand : Command
{
    public DeployCommand() : base(name: "deploy", "Deploy schemas using deploymentplan")
    {
        AddOption(new Option<string>(new[] { "--environment", "-e" }, "Target environment for deploy") { IsRequired = true });
        AddOption(new Option<string>(new[] { "--deploymentplan", "-dp" }, "Deploymentplan to use"));
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IDeploymentPlanFileService _deploymentPlanFileService;
        private readonly ILogger<DeployCommand> _logger;

        public Handler(IMediator mediator,
            ILogger<DeployCommand> logger,
            IDeploymentPlanFileService deploymentPlanFileService)
        {
            _mediator = mediator;
            _logger = logger;
            _deploymentPlanFileService = deploymentPlanFileService;
        }

        public string Environment { get; set; }

        public string DeploymentPlan { get; set; } = DedploymentPlanFileService.DefaultDeploymentPlanFileName;

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            _logger.LogInformation($"Deploy {DeploymentPlan} to {Environment}");

            // Read deployment plan file
            var plan = _deploymentPlanFileService.GetDeploymentPlan(DeploymentPlan);
            if (plan == null)
            {
                _logger.LogError($"Deployment plan {DeploymentPlan} not found!");
                return 1;
            }

            if (!plan.Schemas.Any())
            {
                _logger.LogError($"Deployment plan contains no schemas!");
                return 1;
            }

            // Resolve environment alias
            var targetEnvironment = await GetTargetEnvironmentId(Environment);
            if (targetEnvironment == null)
            {
                _logger.LogError($"Environment {Environment} not found!");
                return 1;
            }

            // Fetch schemas to deploy
            var schemasToDeploy = await FetchSchemas(plan.Schemas, targetEnvironment);
            if (schemasToDeploy == null)
            {
                return 1;
            }

            // Check if schemas are locked
            if (!EnsureSchemasLocked(schemasToDeploy))
            {
                return 1;
            }

            // Execute deployment
            var createReleaseRequest = new CreateReleaseRequest
            {
                EnvironmentId = targetEnvironment.IdValue,
                Schemas = schemasToDeploy.Select(x => new SchemaVersionId
                {
                    SchemaId = x.Version.Id.IdValue,
                    Version = x.Version.Id.Version
                }).ToArray()
            };

            var createReleaseResponse = await _mediator.Send(createReleaseRequest);

            if (!createReleaseResponse.Success)
            {
                LogError(createReleaseResponse.Error);
                return 1;
            }

            return 0;
        }

        private async Task<EnvironmentId> GetTargetEnvironmentId(string environmentName)
        {
            var environments = await _mediator.Send(new GetEnvironmentsRequest());
            var targetEnvironment = environments.FirstOrDefault(env => env.Name == environmentName);
            return targetEnvironment?.Id;
        }

        private async Task<IReadOnlyList<GetMappingSchemaResponse>> FetchSchemas(List<DeploymentPlanSchema> planSchemas,
            EnvironmentId targetEnvironment)
        {
            var schemas = new List<GetMappingSchemaResponse>();

            // Resolve schema aliases
            var schemaQueryResponse = await _mediator.Send(new QueryMappingSchemasRequest());

            foreach (var planSchema in planSchemas)
            {
                var mappingSchemaId = schemaQueryResponse.FirstOrDefault(x => x.ViewHandle == planSchema.Schema)?.Id;
                if (mappingSchemaId == null)
                {
                    _logger.LogError($"Schema with alias {planSchema.Schema} not found");
                    return null;
                }

                var schema = await _mediator.Send(
                    new GetMappingSchemaRequest
                    {
                        MappingSchemaId = mappingSchemaId.MappingSchemaGuid,
                        Version = planSchema.Version
                    }
                );

                var exstingDeployedSchemaForEnvironment =
                    schema.Deployments.FirstOrDefault(d => d.EnvironmentId == targetEnvironment.IdValue);

                // Check if deployed schema for environment is the same version as the one we are deploying
                // If not, then add to list of schemas to deploy
                if (exstingDeployedSchemaForEnvironment != null &&
                    exstingDeployedSchemaForEnvironment.Version != schema.Version.Id.Version)
                {
                    schemas.Add(schema);
                }
                // If existing deployed schema for environment is null, then there are no deployed schema of this type
                // on the environment. In this case we add the schema to list of schemas to deploy.
                else if (exstingDeployedSchemaForEnvironment == null)
                {
                    schemas.Add(schema);
                }
            }

            return schemas;
        }

        private bool EnsureSchemasLocked(IEnumerable<GetMappingSchemaResponse> schemasToDeploy)
        {
            foreach (var schema in schemasToDeploy)
            {
                if (schema.Version.IsEditAble)
                {
                    _logger.LogError($"Schema {schema.ViewHandle} is not locked");
                    return false;
                }
            }

            return true;
        }

        private void LogError(ApiErrorBaseResponse apiErrorBaseResponse)
        {
            if (apiErrorBaseResponse is ApiGroupedErrorResponse apiGroupedErrorResponse)
            {
                foreach (var groupedError in apiGroupedErrorResponse.Errors)
                {
                    var error = JsonSerializer.Serialize(groupedError.Errors);
                    _logger.LogError("Something went wrong when deploying schema with id '" + groupedError.Name + "': " + error);
                }
            }
            else
            {
                var error = JsonSerializer.Serialize((ApiErrorResponse)apiErrorBaseResponse);
                _logger.LogError("Deployment of schemas failed: " + error);
            }
        }
    }
}