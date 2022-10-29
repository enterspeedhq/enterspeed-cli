using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Schema;

public class CreateSchemaRequest : IRequest<CreateSchemaResponse>
{
    public string Name { get; set; }
    public string ViewHandle { get; set; }
}

public class CreateSchemaResponse
{
    public string IdValue { get; set; }

    public string MappingSchemaGuid { get; set; }

    public int Version { get; set; }
}

public class GetDomainRequestHandler : IRequestHandler<CreateSchemaRequest, CreateSchemaResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public GetDomainRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<CreateSchemaResponse> Handle(CreateSchemaRequest createSchemaRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/mapping-schemas", Method.Post).AddJsonBody(createSchemaRequest);
        var response = await _enterspeedClient.ExecuteAsync<CreateSchemaResponse>(request, cancellationToken);
        return response;
    }
}
