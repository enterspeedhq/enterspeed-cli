using System.Text.Json;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.MappingSchema;

public class ValidateMappingSchemaRequest : IRequest<ValidateMappingSchemaResponse>
{
    public string MappingSchemaId { get; set; }
    public int Version { get; set; }
}

public class ValidateMappingSchemaResponse
{
    public bool Success { get; set; }
    public ApiErrorResponse Error { get; set; }
}


public class ValidateMappingSchemaRequestHandler : IRequestHandler<ValidateMappingSchemaRequest, ValidateMappingSchemaResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public ValidateMappingSchemaRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<ValidateMappingSchemaResponse> Handle(ValidateMappingSchemaRequest validateMappingSchemaRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/mapping-schemas/{validateMappingSchemaRequest.MappingSchemaId}/version/{validateMappingSchemaRequest.Version}/_validate", Method.Put);
        var response = await _enterspeedClient.ExecuteAsync(request, cancellationToken);

        var validateResponse = new ValidateMappingSchemaResponse {Success = response.IsSuccessful};
        
        // TODO: Extract to common enterspeed client
        if (!response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
        {
            validateResponse.Error = JsonSerializer.Deserialize<ApiErrorResponse>(response.Content);
        }

        return validateResponse;
    }
}