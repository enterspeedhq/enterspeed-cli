using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.SourceGroup;

public class DeleteSourceGroupRequest : IRequest<DeleteSourceGroupResponse>
{
    public SourceGroupId SourceGroupId { get; }

    public DeleteSourceGroupRequest(SourceGroupId sourceGroupId)
    {
        SourceGroupId = sourceGroupId;
    }
}

public class DeleteSourceGroupResponse
{
}

public class DeleteSourceGroupRequestHandler : IRequestHandler<DeleteSourceGroupRequest, DeleteSourceGroupResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public DeleteSourceGroupRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<DeleteSourceGroupResponse> Handle(DeleteSourceGroupRequest deleteRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/source-groups/{deleteRequest.SourceGroupId.SourceGroupGuid}",
            Method.Delete);

        var response = await _enterspeedClient.ExecuteAsync<DeleteSourceGroupResponse>(request, cancellationToken);
        return response;
    }
}