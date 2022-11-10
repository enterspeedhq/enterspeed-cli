using System.Text.Json.Serialization;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Domain;

public class UpdateDomainRequest : IRequest<UpdateDomainResponse>
{
    [JsonIgnore]
    public DomainId DomainId { get; }
    public string Name { get; set; }
    public string[] Hostnames { get; set; }

    public UpdateDomainRequest(DomainId domainId)
    {
        DomainId = domainId;
    }
}

public class UpdateDomainResponse
{
    public DomainId Id { get; set; }

    public TenantId TenantId { get; set; }

    public string Name { get; set; }

    public string[] Hostnames { get; set; }
}


public class UpdateDomainRequestRequestHandler : IRequestHandler<UpdateDomainRequest, UpdateDomainResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public UpdateDomainRequestRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<UpdateDomainResponse> Handle(UpdateDomainRequest updateDomainRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/domains/{updateDomainRequest.DomainId.DomainGuid}", Method.Put)
            .AddJsonBody(updateDomainRequest);

        var response = await _enterspeedClient.ExecuteAsync<UpdateDomainResponse>(request, cancellationToken);
        return response;
    }
}