using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.MappingSchema
{
    public class QueryMappingSchemasRequest : IRequest<QueryMappingSchemasResponse>
    {
    }

    public class QueryMappingSchemasResponse
    {
        public QueryMappingSchemaResponse[] Results { get; set; }
    }

    public class QueryMappingSchemaResponse
    {
        public string Name { get; set; }
        public string ViewHandle { get; set; }
        public MappingSchemaId Id { get; set; }
    }

    public class QueryMappingSchemasRequestHandler : IRequestHandler<QueryMappingSchemasRequest, QueryMappingSchemasResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public QueryMappingSchemasRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<QueryMappingSchemasResponse> Handle(QueryMappingSchemasRequest queryMappingSchemasRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest("tenant/mapping-schemas", Method.Get).AddJsonBody(queryMappingSchemasRequest);
            var response = await _enterspeedClient.ExecuteAsync<QueryMappingSchemasResponse>(request, cancellationToken);
            return response;
        }
    }
}