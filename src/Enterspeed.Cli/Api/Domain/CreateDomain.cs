using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Domain;

public class CreateDomainRequest : IRequest<CreateDomainResponse>
{
    public string Name { get; set; }
    public string[] Hostnames { get; set; }
}

public class CreateDomainResponse
{
    public DomainId DomainId { get; set; }
}

public class CreateDomainRequestRequestHandler : IRequestHandler<CreateDomainRequest, CreateDomainResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public CreateDomainRequestRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<CreateDomainResponse> Handle(CreateDomainRequest createDomainRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/domains", Method.Post)
            .AddJsonBody(createDomainRequest);

        var response = await _enterspeedClient.ExecuteAsync<CreateDomainResponse>(request, cancellationToken);
        return response;
    }
}