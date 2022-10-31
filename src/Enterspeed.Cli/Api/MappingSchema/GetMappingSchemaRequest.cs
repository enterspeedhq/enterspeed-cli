using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.MappingSchema
{
    public class GetMappingSchemaRequest : IRequest<GetMappingSchemaResponse>
    {
        public string MappingSchemaId { get; set; }
        public int Version { get; set; }
    }

    public class GetMappingSchemaResponse
    {
        public string Name { get; set; }
        public string ViewHandle { get; set; }
        public MappingSchemaId MappingSchemaId { get; set; }
        public int LatestVersion { get; set; }
        public Version Version { get; set; }
    }

    public class GetMappingSchemaRequestHandler : IRequestHandler<GetMappingSchemaRequest, GetMappingSchemaResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public GetMappingSchemaRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<GetMappingSchemaResponse> Handle(GetMappingSchemaRequest getMappingSchemaRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"tenant/mapping-schemas/{getMappingSchemaRequest.MappingSchemaId}").AddJsonBody(getMappingSchemaRequest);
            var response = await _enterspeedClient.ExecuteAsync<GetMappingSchemaResponse>(request, cancellationToken);
            return response;
        }
    }
}