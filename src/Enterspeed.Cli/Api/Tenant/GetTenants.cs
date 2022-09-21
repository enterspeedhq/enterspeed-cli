using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Tenant;

public class GetTenantsRequest : IRequest<GetTenantsResponse[]>
{
    public string[] TenantIds { get; set; }
}

public class GetTenantsResponse
{
    public TenantId Id { get; set; }
    public string Name { get; set; }
    public bool IsUsingSchemasBulkDeployment { get; set; }
    public bool IsUsingSourceGroups { get; set; }
}

public class GetTenantsRequestHandler : IRequestHandler<GetTenantsRequest, GetTenantsResponse[]>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public GetTenantsRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<GetTenantsResponse[]> Handle(GetTenantsRequest getTenantsRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenants");
        foreach (var id in getTenantsRequest.TenantIds)
        {
            request.AddParameter("ids", id);
        }

        var response = await _enterspeedClient.ExecuteAsync<GetTenantsResponse[]>(request, cancellationToken);
        return response;
    }
}