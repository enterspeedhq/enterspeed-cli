using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.View;

public class QueryViewsRequest : IRequest<QueryViewsResponse>
{
    public EnvironmentId EnvironmentId { get; set; }
    public SourceId SourceId { get; set; }
    public string SchemaAlias { get; set; }
    public string SourceEntityOriginId { get; set; }


    public int PageSize { get; set; } = 10;
}

public class QueryViewsResponse
{
    //PageInfo
    public ViewResponse[] Results { get; set; }
}

public class ViewResponse
{
    public ViewId Id { get; set; }
    public SourceEntityId SourceEntityId { get; set; }
    public string SourceName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class QueryViewsRequestHandler : IRequestHandler<QueryViewsRequest, QueryViewsResponse>
{
    private readonly IEnterspeedClient _enterspeedClient;

    public QueryViewsRequestHandler(IEnterspeedClient enterspeedClient)
    {
        _enterspeedClient = enterspeedClient;
    }

    public async Task<QueryViewsResponse> Handle(QueryViewsRequest queryRequest, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"tenant/environments/{queryRequest.EnvironmentId.EnvironmentGuid}/views");
        request.AddParameter("first", queryRequest.PageSize);

        if (queryRequest.SourceId != null)
        {
            request.AddParameter("sourceGuid", queryRequest.SourceId.SourceGuid);
        }

        if (!string.IsNullOrEmpty(queryRequest.SchemaAlias))
        {
            request.AddParameter("schemaAlias", queryRequest.SchemaAlias);
        }

        if (!string.IsNullOrEmpty(queryRequest.SourceEntityOriginId))
        {
            request.AddParameter("sourceEntityOriginId", queryRequest.SourceEntityOriginId);
        }

        var response = await _enterspeedClient.ExecuteAsync<QueryViewsResponse>(request, cancellationToken);
        return response;
    }
}