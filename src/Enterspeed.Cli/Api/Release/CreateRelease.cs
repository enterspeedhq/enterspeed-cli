using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;
using System.Text.Json;

namespace Enterspeed.Cli.Api.Release;

public class CreateReleaseRequest : IRequest<CreateReleaseResponse>
{
    public string EnvironmentId { get; set; }
    public SchemaVersionId[] Schemas { get; set; }
}

public class SchemaVersionId
{
    public string SchemaId { get; set; }
    public int? Version { get; set; }
}

public class CreateReleaseResponse
{
    public bool Success { get; set; }
    public ApiErrorBaseResponse Error { get; set; }
}

public class CreateReleaseRequestHandler : IRequestHandler<CreateReleaseRequest, CreateReleaseResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public CreateReleaseRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<CreateReleaseResponse> Handle(CreateReleaseRequest createRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/releases", Method.Post)
            .AddJsonBody(createRequest);

        var response = await _enterspeedClient.ExecuteAsync(request, cancellationToken);
        
        var createReleaseResponse = new CreateReleaseResponse { Success = response.IsSuccessful };
        
        if (!response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
        {
            var apiErrorBaseResponse = JsonSerializer.Deserialize<ApiErrorBaseResponse>(response.Content);

            if (apiErrorBaseResponse.Code == ErrorCode.FailedToCreateRelease)
            {
                createReleaseResponse.Error = JsonSerializer.Deserialize<ApiGroupedErrorResponse>(response.Content);
            }
            else
            {
                createReleaseResponse.Error = JsonSerializer.Deserialize<ApiErrorResponse>(response.Content);
            }
        }

        return createReleaseResponse;
    }
}