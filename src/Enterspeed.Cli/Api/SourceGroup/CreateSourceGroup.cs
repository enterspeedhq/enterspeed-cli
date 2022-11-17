using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.SourceGroup;
   
public class CreateSourceGroupRequest : IRequest<CreateSourceGroupResponse>
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string Type { get; set; }
}

public class CreateSourceGroupResponse
{
    public SourceGroupId SourceGroupId { get; set; }
}

public class CreateSourceGroupRequestHandler : IRequestHandler<CreateSourceGroupRequest, CreateSourceGroupResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public CreateSourceGroupRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<CreateSourceGroupResponse> Handle(CreateSourceGroupRequest createRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/source-groups", Method.Post)
            .AddJsonBody(createRequest);

        var response = await _enterspeedClient.ExecuteAsync<CreateSourceGroupResponse>(request, cancellationToken);
        return response;
    }
}