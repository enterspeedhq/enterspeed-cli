using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;
using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Api.Source;

public class UpdateSourceRequest : IRequest<UpdateSourceResponse>
{
    [JsonIgnore] public SourceId SourceId { get; }

    public UpdateSourceRequest(SourceId id)
    {
        SourceId = id;
    }

    public string Name { get; set; }
    public string Type { get; set; }
    public string[] EnvironmentIds { get; set; }

    public string SourceGroupId { get; set; }
    public bool RegenerateAccessKey { get; set; }
}

public class UpdateSourceResponse
{
    public SourceId SourceId { get; set; }
}

public class UpdateSourceRequestHandler : IRequestHandler<UpdateSourceRequest, UpdateSourceResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public UpdateSourceRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<UpdateSourceResponse> Handle(UpdateSourceRequest updateRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/sources/{updateRequest.SourceId.SourceGuid}", Method.Put)
            .AddJsonBody(updateRequest);

        var response = await _enterspeedClient.ExecuteAsync<UpdateSourceResponse>(request, cancellationToken);
        return response;
    }
}