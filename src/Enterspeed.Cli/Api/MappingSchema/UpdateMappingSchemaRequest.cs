using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.MappingSchema
{
    public class UpdateMappingSchemaRequest : IRequest<UpdateMappingSchemaResponse>
    {
        public string Name { get; set; }
        public string MappingSchemaId { get; set; }
    }

    public class UpdateMappingSchemaResponse
    {
        public string IdValue { get; set; }
        public string MappingSchemaGuid { get; set; }
        public int Version { get; set; }
    }

    public class UpdateMappingSchemaRequestHandler : IRequestHandler<UpdateMappingSchemaRequest, UpdateMappingSchemaResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public UpdateMappingSchemaRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<UpdateMappingSchemaResponse> Handle(UpdateMappingSchemaRequest updateMappingSchemaRequest,
            CancellationToken cancellationToken)
        {
            var request = new RestRequest(
                $"tenant/mapping-schemas/{updateMappingSchemaRequest.MappingSchemaId}",
                Method.Put).AddJsonBody(updateMappingSchemaRequest);

            var response = await _enterspeedClient.ExecuteAsync<UpdateMappingSchemaResponse>(request, cancellationToken);
            return response;
        }
    }
}