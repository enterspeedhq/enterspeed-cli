using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.SourceGroup;

public class GetSourceGroupsRequest : IRequest<GetSourceGroupResponse[]>
{
    public string[] TenantIds { get; set; }
}

public class GetSourceGroupResponse
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string Type { get; set; }
    public SourceGroupId Id { get; set; }
    public TenantId TenantId { get; set; }
    public SourceResponse[] Sources { get; set; }
}

public class SourceResponse
{
    public int EntitiesInSource { get; set; }

    public GetEnvironmentsResponse[] Environments { get; set; }
    public Source Source { get; set; }
}

public class Source
{
    public string Name { get; set; }
    public string Type { get; set; }
    public SourceId Id { get; set; }
}

public class GetTenantsRequestHandler : IRequestHandler<GetSourceGroupsRequest, GetSourceGroupResponse[]>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public GetTenantsRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<GetSourceGroupResponse[]> Handle(GetSourceGroupsRequest getSourceGroupRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/source-groups");

        var response = await _enterspeedClient.ExecuteAsync<GetSourceGroupResponse[]>(request, cancellationToken);
        return response;
    }
}