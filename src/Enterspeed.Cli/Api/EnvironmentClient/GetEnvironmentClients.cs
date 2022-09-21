using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.EnvironmentClient;

public class GetEnvironmentClientsRequest : IRequest<GetEnvironmentClientsResponse[]>
{
}

public class GetEnvironmentClientsResponse
{
    public EnvironmentClientId Id { get; set; }

    public TenantId TenantId { get; set; }

    public string Name { get; set; }
    public string AccessKey { get; set; }
    public string EnvironmentName { get; set; }
    public string[] DomainIds { get; set; }
}

public class GetEnvironmentClientsRequestHandler : IRequestHandler<GetEnvironmentClientsRequest, GetEnvironmentClientsResponse[]>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public GetEnvironmentClientsRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<GetEnvironmentClientsResponse[]> Handle(GetEnvironmentClientsRequest getEnvironmentsRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest("tenant/environment-clients")
            .AddJsonBody(getEnvironmentsRequest);

        var response = await _enterspeedClient.ExecuteAsync<GetEnvironmentClientsResponse[]>(request, cancellationToken);
        return response;
    }
}