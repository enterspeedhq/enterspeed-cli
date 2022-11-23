using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Domain;

public class DeleteDomainRequest : IRequest<DeleteDomainResponse>
{
    public DomainId DomainId { get; }

    public DeleteDomainRequest(DomainId domainId)
    {
        DomainId = domainId;
    }
}

public class DeleteDomainResponse
{
}

public class DeleteEnvironmentRequestRequestHandler : IRequestHandler<DeleteDomainRequest, DeleteDomainResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public DeleteEnvironmentRequestRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<DeleteDomainResponse> Handle(DeleteDomainRequest deleteDomainRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/domains/{deleteDomainRequest.DomainId.DomainGuid}", Method.Delete);

        var response = await _enterspeedClient.ExecuteAsync<DeleteDomainResponse>(request, cancellationToken);
        return response;
    }
}