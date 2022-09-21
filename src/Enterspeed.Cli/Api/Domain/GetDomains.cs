using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Domain;

public class GetDomainsRequest : IRequest<GetDomainsResponse[]>
{
}

public class GetDomainsResponse
{
    public DomainId Id { get; set; }

    public TenantId TenantId { get; set; }

    public string Name { get; set; }

    public string[] Hostnames { get; set; }
}

public class GetDomainRequestHandler : IRequestHandler<GetDomainsRequest, GetDomainsResponse[]>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public GetDomainRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<GetDomainsResponse[]> Handle(GetDomainsRequest getDomainsRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/domains")
            .AddJsonBody(getDomainsRequest);

        var response = await _enterspeedClient.ExecuteAsync<GetDomainsResponse[]>(request, cancellationToken);
        return response;
    }
}