using Microsoft.Extensions.Logging;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Services.FileService;
using MediatR;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.FileService.Models;

namespace Enterspeed.Cli.Commands.Deploy
{
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
                var schemasToDeploy = await FetchSchemas(plan.Schemas);
                if (schemasToDeploy == null)
                {
                    return 1;
                }

                // Check if schemas are locked
                if (!EnsureSchemasLocked(schemasToDeploy))
                {
                    return 1;
                }

                // Validate all schemas
                if (!await ValidateSchemas(schemasToDeploy))
                {
                    return 1;
                }

                // Execute deployment
                foreach (var schema in schemasToDeploy)
                {
                    var deployRequest = new DeployMappingSchemaRequest
                    {
                        SchemaId = schema.Version.Id.IdValue,
                        Deployments = new List<DeployMappingSchemaRequest.EnvironmentSchemaDeployment>
                        {
                            new()
                            {
                                EnvironmentId = targetEnvironment.IdValue,
                                Version = schema.Version.Id.Version
                            }
                        }
                    };

                    var deployResponse = await _mediator.Send(deployRequest);

                    if (!deployResponse.Success)
                    {
                        _logger.LogError($"Schema {schema.ViewHandle} failed deployment ({deployResponse.Error?.Code})");
                        // TODO: Log better error messages
                        return 1;
                    }
                }

                return 0;
            }

            private async Task<EnvironmentId> GetTargetEnvironmentId(string environmentName)
            {
                var environments = await _mediator.Send(new GetEnvironmentsRequest());
                var targetEnvironment = environments.FirstOrDefault(env => env.Name == environmentName);
                return targetEnvironment?.Id;
            }

            private async Task<IReadOnlyList<GetMappingSchemaResponse>> FetchSchemas(List<DeploymentPlanSchema> planSchemas)
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
                    schemas.Add(schema);
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

            private async Task<bool> ValidateSchemas(IEnumerable<GetMappingSchemaResponse> schemasToDeploy)
            {
                foreach (var schema in schemasToDeploy)
                {
                    var validateResponse = await _mediator.Send(new ValidateMappingSchemaRequest
                    {
                        MappingSchemaId = schema.Version.Id.MappingSchemaGuid,
                        Version = schema.Version.Id.Version
                    });

                    if (!validateResponse.Success)
                    {
                        _logger.LogError($"Schema {schema.ViewHandle} failed validation ({validateResponse.Error?.Code})");
                        // TODO: Log better error messages
                        return false;
                    }
                }

                return true;
            }

        }
    }
}
