using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;
using System.Text.Json;

namespace Enterspeed.Cli.Api.MappingSchema;

public class DeployMappingSchemaRequest : IRequest<DeployMappingSchemaResponse>
{
    public string SchemaId { get; set; }

    public string Comment { get; set; }

    public List<EnvironmentSchemaDeployment> Deployments { get; set; }

    public class EnvironmentSchemaDeployment
    {
        public string EnvironmentId { get; set; }
        public int? Version { get; set; }
    }
}

public class DeployMappingSchemaResponse
{
    public bool Success { get; set; }
    public ApiErrorResponse Error { get; set; }
}

public class DeployMappingSchemaRequestHandler : IRequestHandler<DeployMappingSchemaRequest, DeployMappingSchemaResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public DeployMappingSchemaRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<DeployMappingSchemaResponse> Handle(DeployMappingSchemaRequest deployMappingSchemaRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/mapping-schemas/deploy", Method.Post).AddJsonBody(deployMappingSchemaRequest);
        var response = await _enterspeedClient.ExecuteAsync(request, cancellationToken);

        var validateResponse = new DeployMappingSchemaResponse { Success = response.IsSuccessful };
        // TODO: Extract to common enterspeed client
        if (!response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
        {
            validateResponse.Error = JsonSerializer.Deserialize<ApiErrorResponse>(response.Content);
        }

        return validateResponse;
    }
}