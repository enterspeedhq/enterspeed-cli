using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Api.MappingSchema;
using Enterspeed.Cli.Api.Release;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.ConsoleOutput;
using Enterspeed.Cli.Services.FileService;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.Schema;

internal class DeploySchemaCommand : Command
{
    public DeploySchemaCommand() : base("deploy", "Adds schema to deployment plan")
    {
        AddArgument(new Argument<string>("alias", "Alias of the schema"));
        AddOption(new Option<bool>(new[] { "--force", "-f" }, "Force redeploy"));
        AddOption(new Option<string>(new[] { "--environment", "-e" }, "Environment name")
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

        public string Alias { get; set; }
        public string Environment { get; set; }
        public bool Force { get; set; }

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

            // Get environment
            var environmentToDeployTo = await GetEnvironmentToDeployTo();
            if (environmentToDeployTo == null)
            {
                _logger.LogError("Environment to deploy to was not found");
                return 1;
            }

            // Check if version we are deploying is already deployed 
            var currentlyDeployedSchema =
                existingSchema.Deployments.FirstOrDefault(p => p.EnvironmentId == environmentToDeployTo.Id.IdValue);

            if (currentlyDeployedSchema?.Version == existingSchema.Version.Id.Version && !Force)
            {
                var message = "This version of the schema has already been deployed";
                _logger.LogInformation(message);
                _outputService.Write(message);

                return 1;
            }

            // Validate that schema on disk matches schema saved in Enterspeed.
            var valid = _schemaFileService.SchemaValid(existingSchema.Version.Data, Alias);
            if (!valid)
            {
                _logger.LogError("Schema on disk does not match schema in Enterspeed. Save your schema before deploying it.");
                return 1;
            }

            // Deploy schema
            var createReleaseResponse = await _mediator.Send(new CreateReleaseRequest
            {
                EnvironmentId = environmentToDeployTo.Id.IdValue,
                Schemas = new[]
                {
                    new SchemaVersionId
                    {
                        SchemaId = mappingSchemaId.IdValue,
                        Version = existingSchema.Version.Id.Version
                    }
                }
            });

            if (!createReleaseResponse.Success)
            {
                LogError(createReleaseResponse.Error);

                return 1;
            }

            _deploymentPlanFileService.UpdateDeploymentPlan(Alias, existingSchema.LatestVersion);

            _outputService.Write("Successfully deployed schema: " + Alias);

            return 0;
        }

        private async Task<GetEnvironmentsResponse> GetEnvironmentToDeployTo()
        {
            var environments = await _mediator.Send(new GetEnvironmentsRequest());
            var environmentToDeployTo = environments.FirstOrDefault(e => e.Name == Environment);
            return environmentToDeployTo;
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
            var mappingSchemaGuid = schemas.FirstOrDefault(sc => sc.ViewHandle == Alias)?.Id;
            return mappingSchemaGuid;
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
                _logger.LogError("Something went wrong when deploying schema: " + error);
            }
        }
    }
}