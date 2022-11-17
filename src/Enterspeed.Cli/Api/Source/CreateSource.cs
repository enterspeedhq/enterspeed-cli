using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Source;
public class CreateSourceRequest : IRequest<CreateSourceResponse>
{
    public string SourceGroupId { get; }
    public string Name { get; set; }
    public string Type { get; set; }

    public CreateSourceRequest(SourceGroupId sourceGroupId)
    {
        SourceGroupId = sourceGroupId.SourceGroupGuid;
    }
}

public class CreateSourceResponse
{
    public SourceId SourceId { get; set; }
}

public class CreateSourceGroupRequestHandler : IRequestHandler<CreateSourceRequest, CreateSourceResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public CreateSourceGroupRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<CreateSourceResponse> Handle(CreateSourceRequest createRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/sources", Method.Post)
            .AddJsonBody(createRequest);

        var response = await _enterspeedClient.ExecuteAsync<CreateSourceResponse>(request, cancellationToken);
        return response;
    }
}