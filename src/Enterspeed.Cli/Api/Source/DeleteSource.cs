using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Source;

public class DeleteSourceRequest : IRequest<DeleteSourceResponse>
{
    public SourceId SourceId { get; }

    public DeleteSourceRequest(SourceId sourceId)
    {
        SourceId = sourceId;
    }
}

public class DeleteSourceResponse
{
}

public class DeleteSourceRequestHandler : IRequestHandler<DeleteSourceRequest, DeleteSourceResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public DeleteSourceRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<DeleteSourceResponse> Handle(DeleteSourceRequest deleteRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/sources/{deleteRequest.SourceId.SourceGuid}", Method.Delete);

        var response = await _enterspeedClient.ExecuteAsync<DeleteSourceResponse>(request, cancellationToken);
        return response;
    }
}