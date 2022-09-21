using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.SourceEntity;

public class QuerySourceEntitiesRequest : IRequest<QuerySourceEntitiesResponse>
{
    public SourceId SourceId { get; set; }
    public string Filter { get; set; }
    public string Type { get; set; }


    public int PageSize { get; set; } = 10;
}

public class QuerySourceEntitiesResponse
{
    //PageInfo
    public SourceEntityResponse[] Results { get; set; }
}

public class SourceEntityResponse
{
    public string Id { get; set; }
    public string OriginId { get; set; }
    public string OriginParentId { get; set; }
    public string SourceName { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class QuerySourceEntitiesRequestHandler : IRequestHandler<QuerySourceEntitiesRequest, QuerySourceEntitiesResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public QuerySourceEntitiesRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<QuerySourceEntitiesResponse> Handle(QuerySourceEntitiesRequest queryRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/sources/{queryRequest.SourceId.SourceGuid}/entities");
        request.AddParameter("first", queryRequest.PageSize);

        if (!string.IsNullOrEmpty(queryRequest.Filter))
        {
            request.AddParameter("term", queryRequest.Filter);
        }

        if (!string.IsNullOrEmpty(queryRequest.Type))
        {
            request.AddParameter("typeTerm", queryRequest.Type);
        }

        var response = await _enterspeedClient.ExecuteAsync<QuerySourceEntitiesResponse>(request, cancellationToken);
        return response;
    }
}