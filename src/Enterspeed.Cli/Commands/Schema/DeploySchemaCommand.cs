using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.FileService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.Schema
{
    internal class DeploySchemaCommand : Command
    {
        public DeploySchemaCommand() : base("deploy", "Adds schema to deployment plan")
        {
            AddArgument(new Argument<string>("schemaAlias", "Alias of the schema") { });
            AddOption(new Option<string>(new[] { "--environment", "-e" }, "Alias of environment")
            {
                IsRequired = true
            });
        }

        public new class Handler : BaseCommandHandler, ICommandHandler
        {
            private readonly IMediator _mediator;
            private readonly IOutputService _outputService;
            private readonly ISchemaFileService _schemaFileService;
            private readonly ILogger<DeploySchemaCommand> _logger;
            private readonly IDeploymentPlanFileService _deploymentPlanFileService;

            public Handler(
                IMediator mediator,
                IOutputService outputService,
                ISchemaFileService schemaFileService,
                ILogger<DeploySchemaCommand> logger,
                IDeploymentPlanFileService deploymentPlanFileService)
            {
                _outputService = outputService;
                _mediator = mediator;
                _schemaFileService = schemaFileService;
                _logger = logger;
                _deploymentPlanFileService = deploymentPlanFileService;
            }

            public string SchemaAlias { get; set; }
            public string Environment { get; set; }

            public async Task<int> InvokeAsync(InvocationContext context)
            {
                // Get mapping schema guid
                var mappingSchemaId = await GetMappingSchemaGuid();
                if (mappingSchemaId == null)
                {
                    _logger.LogError("Mapping schema id not found!");
                    return 1;
                }

                // Get version from existing schema
                var existingSchema = await GetExistingSchema(mappingSchemaId.MappingSchemaGuid);
                if (existingSchema == null)
                {
                    _logger.LogError("Schema not found!");
                    return 1;
                }

                // Validate that schema on disk matches schema saved in Enterspeed.
                var valid = _schemaFileService.ValidateSchemaOnDisk(existingSchema.Version.Data, SchemaAlias);
                if (!valid)
                {
                    _logger.LogError("Schema on disk does not match schema in Enterspeed. Save your schema before deploying it.");
                    return 1;
                }

                // Ensure that content of schema is valid
                var validationResponse = await ValidateMappingSchema(existingSchema, mappingSchemaId.MappingSchemaGuid);
                if (!validationResponse.Success)
                {
                    var error = JsonSerializer.Serialize(validationResponse.Error);
                    _logger.LogError("Schema not valid: " + error);
                    return 1;
                }

                // Deploy schema
                var environmentToDeployTo = await GetEnvironmentToDeployTo();
                if (environmentToDeployTo == null)
                {
                    _logger.LogError("Environment to deploy to was not found");
                    return 1;
                }

                var deployMappingSchemaResponse = await _mediator.Send(new DeployMappingSchemaRequest()
                {
                    SchemaId = mappingSchemaId.IdValue,
                    Deployments = new List<DeployMappingSchemaRequest.EnvironmentSchemaDeployment>()
                    {
                        new DeployMappingSchemaRequest.EnvironmentSchemaDeployment()
                        {
                            Version = existingSchema.Version.Id.Version,
                            EnvironmentId = environmentToDeployTo.Id.IdValue
                        }
                    }
                });

                if (!deployMappingSchemaResponse.Success)
                {
                    var error = JsonSerializer.Serialize(deployMappingSchemaResponse.Error);
                    _logger.LogError("Something went wrong when deploying schema: " + error);
                    return 1;
                }

                _deploymentPlanFileService.UpdateDeploymentPlan(SchemaAlias, existingSchema.LatestVersion);

                _outputService.Write("Successfully deployed schema: " + SchemaAlias);

                return 0;
            }

            private async Task<GetEnvironmentsResponse> GetEnvironmentToDeployTo()
            {
                var environments = await _mediator.Send(new GetEnvironmentsRequest());
                var environmentToDeployTo = environments.FirstOrDefault(e => e.Name == Environment);
                return environmentToDeployTo;
            }

            private async Task<ValidateMappingSchemaResponse> ValidateMappingSchema(GetMappingSchemaResponse existingSchema, string mappingSchemaGuid)
            {
                var validationResponse = await _mediator.Send(new ValidateMappingSchemaRequest()
                {
                    Version = existingSchema.LatestVersion,
                    MappingSchemaId = mappingSchemaGuid,
                });

                return validationResponse;
            }

            private async Task<GetMappingSchemaResponse> GetExistingSchema(string mappingSchemaGuid)
            {
                var existingSchema = await _mediator.Send(
                    new GetMappingSchemaRequest
                    {
                        MappingSchemaId = mappingSchemaGuid
                    }
                );

                return existingSchema;
            }

            private async Task<MappingSchemaId> GetMappingSchemaGuid()
            {
                var schemas = await _mediator.Send(new QueryMappingSchemasRequest());
                var mappingSchemaGuid = schemas.FirstOrDefault(sc => sc.ViewHandle == SchemaAlias)?.Id;
                return mappingSchemaGuid;
            }
        }
    }
}