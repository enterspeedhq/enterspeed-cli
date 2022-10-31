using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.MappingSchema
{
    public class QueryMappingSchemasRequest : IRequest<QueryMappingSchemaResponse[]>
    {
    }

    public class QueryMappingSchemaResponse
    {
        public string Name { get; set; }
        public string ViewHandle { get; set; }
        public MappingSchemaId Id { get; set; }
    }

    public class QueryMappingSchemasRequestHandler : IRequestHandler<QueryMappingSchemasRequest, QueryMappingSchemaResponse[]>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public QueryMappingSchemasRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<QueryMappingSchemaResponse[]> Handle(QueryMappingSchemasRequest queryMappingSchemasRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest("tenant/mapping-schemas", Method.Get);
            var response = await _enterspeedClient.ExecuteAsync<QueryMappingSchemaResponse[]>(request, cancellationToken);
            return response;
        }
    }
}