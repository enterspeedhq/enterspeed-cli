using System.Text.Json.Serialization;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.SourceGroup;

public class UpdateSourceGroupRequest : IRequest<UpdateSourceGroupResponse>
{
    [JsonIgnore]
    public SourceGroupId SourceGroupId { get; }
    public UpdateSourceGroupRequest(SourceGroupId id)
    {
        SourceGroupId = id;
    }

    public string Name { get; set; }
    public string Type { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[] EnvironmentIds { get; set; }
    public bool RegenerateAccessKey { get; set; }
}

public class UpdateSourceGroupResponse
{
    public SourceGroupId SourceGroupId { get; set; }
}

public class UpdateSourceGroupRequestHandler : IRequestHandler<UpdateSourceGroupRequest, UpdateSourceGroupResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public UpdateSourceGroupRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<UpdateSourceGroupResponse> Handle(UpdateSourceGroupRequest updateRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/source-groups/{updateRequest.SourceGroupId.SourceGroupGuid}", Method.Put)
            .AddJsonBody(updateRequest);

        var response = await _enterspeedClient.ExecuteAsync<UpdateSourceGroupResponse>(request, cancellationToken);
        return response;
    }
}