using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.MappingSchema
{
    public class UpdateMappingSchemaVersionRequest : IRequest<UpdateMappingSchemaVersionResponse>
    {
        public string MappingSchemaId { get; set; }
        public int Version { get; set; }
        public string Format { get; set; }
        public object Schema { get; set; }
    }

    public class UpdateMappingSchemaVersionResponse
    {
        public string IdValue { get; set; }
        public string MappingSchemaGuid { get; set; }
        public int Version { get; set; }
    }

    public class UpdateMappingSchemaVersionRequestHandler : IRequestHandler<UpdateMappingSchemaVersionRequest, UpdateMappingSchemaVersionResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public UpdateMappingSchemaVersionRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<UpdateMappingSchemaVersionResponse> Handle(UpdateMappingSchemaVersionRequest updateMappingSchemaVersionRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest(
                $"tenant/mapping-schemas/{updateMappingSchemaVersionRequest.MappingSchemaId}/version/{updateMappingSchemaVersionRequest.Version}",
                Method.Put).AddJsonBody(updateMappingSchemaVersionRequest);

            var response = await _enterspeedClient.ExecuteAsync<UpdateMappingSchemaVersionResponse>(request, cancellationToken);
            return response;
        }
    }
}