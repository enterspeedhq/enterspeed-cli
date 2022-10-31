using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Schema;

public class GetSchemaRequest : IRequest<SchemaDetailsResponse>
{
    public Guid SchemaGuid { get; set; }
    public int? Version { get; set; }
}

public class SchemaDetailsResponse
{
    public MappingSchemaId Id { get; set; }
    public TenantId TenantId { get; set; }
    public string Name { get; set; }
    public string ViewHandle { get; set; }
    public int? LatestVersion { get; set; }
    //public MappingSchemaVersionDto Version { get; set; }
    //public List<MappingSchemaVersionListDto> Versions { get; set; }
    //public List<EnvironmentSchemaDeploymentDto> Deployments { get; set; }
}

public class GetSchemaRequestHandler : IRequestHandler<GetSchemaRequest, SchemaDetailsResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public GetSchemaRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<SchemaDetailsResponse> Handle(GetSchemaRequest getSchemaRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/mapping-schemas/{getSchemaRequest.SchemaGuid}")
            .AddJsonBody(getSchemaRequest);

        if (getSchemaRequest.Version.HasValue)
        {
            request.AddQueryParameter("version", getSchemaRequest.Version.Value);
        }

        var response = await _enterspeedClient.ExecuteAsync<SchemaDetailsResponse>(request, cancellationToken);
        return response;
    }
}