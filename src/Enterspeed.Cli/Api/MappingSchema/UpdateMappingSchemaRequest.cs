using Enterspeed.Cli.Services.EnterspeedClient;
using RestSharp;

namespace Enterspeed.Cli.Api.MappingSchema
{
    public class UpdateMappingSchemaRequest
    {
        public string MappingSchemaId { get; set; }
        public int Version { get; set; }
        public string Format { get; set; }
        public object Schema { get; set; }
    }

    public class UpdateMappingSchemaResponse
    {
        public string IdValue { get; set; }
        public string MappingSchemaGuid { get; set; }
        public int Version { get; set; }
    }

    public class UpdateMappingSchemaRequestHandler
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public UpdateMappingSchemaRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<UpdateMappingSchemaResponse> Handle(UpdateMappingSchemaRequest updateMappingSchemaRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest(
                $"https://management.enterspeed.com/api/v1/tenant/mapping-schemas/{updateMappingSchemaRequest.MappingSchemaId}/version/{updateMappingSchemaRequest.Version}",
                Method.Put).AddJsonBody(updateMappingSchemaRequest);

            var response = await _enterspeedClient.ExecuteAsync<UpdateMappingSchemaResponse>(request, cancellationToken);
            return response;
        }
    }
}