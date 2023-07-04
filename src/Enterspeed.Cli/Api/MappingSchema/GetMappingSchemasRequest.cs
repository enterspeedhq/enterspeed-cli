using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.MappingSchema
{
    public class GetMappingSchemasRequest : IRequest<GetMappingSchemasResponse>
    {
        public string EnvironmentId { get; set; }
    }

    public class GetMappingSchemasResponse
    {
        public Current Current { get; set; }
    }

    public class Current
    {
        public SchemaResponse[] Schemas { get; set; }
    }

    public class SchemaResponse
    {
        public string Alias { get; set; }
        public MappingSchemaId Id { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
    }

    public class GetMappingSchemasRequestHandler : IRequestHandler<GetMappingSchemasRequest, GetMappingSchemasResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public GetMappingSchemasRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<GetMappingSchemasResponse> Handle(GetMappingSchemasRequest getMappingSchemasRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"tenant/environments/{getMappingSchemasRequest.EnvironmentId}/deployments");
            return await _enterspeedClient.ExecuteAsync<GetMappingSchemasResponse>(request, cancellationToken);
        }
    }
}