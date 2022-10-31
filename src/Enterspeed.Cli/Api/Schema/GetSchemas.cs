using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Schema;

public class GetSchemasRequest : IRequest<GetSchemaResponse[]>
{
}

public class GetSchemaResponse
{
    public MappingSchemaId Id { get; set; }
    public TenantId TenantId { get; set; }
    public string Name { get; set; }
    public int? LatestVersion { get; set; }
    public string ViewHandle { get; set; }
}
public class GetSchemasRequestHandler : IRequestHandler<GetSchemasRequest, GetSchemaResponse[]>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public GetSchemasRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<GetSchemaResponse[]> Handle(GetSchemasRequest getSchemasRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/mapping-schemas")
            .AddJsonBody(getSchemasRequest);

        var response = await _enterspeedClient.ExecuteAsync<GetSchemaResponse[]>(request, cancellationToken);
        return response;
    }
}