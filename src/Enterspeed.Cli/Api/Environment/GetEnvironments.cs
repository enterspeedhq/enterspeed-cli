using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Environment;

public class GetEnvironmentsRequest : IRequest<GetEnvironmentsResponse[]>
{
}

public class GetEnvironmentsResponse
{
    public EnvironmentId Id { get; set; }

    public TenantId TenantId { get; set; }

    public string Name { get; set; }
}

public class GetEnvironmentsRequestHandler : IRequestHandler<GetEnvironmentsRequest, GetEnvironmentsResponse[]>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public GetEnvironmentsRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<GetEnvironmentsResponse[]> Handle(GetEnvironmentsRequest getEnvironmentsRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/environments")
            .AddJsonBody(getEnvironmentsRequest);

        var response = await _enterspeedClient.ExecuteAsync<GetEnvironmentsResponse[]>(request, cancellationToken);
        return response;
    }
}