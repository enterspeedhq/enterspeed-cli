using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.MappingSchema
{
    public class CreateMappingSchemaRequest : IRequest<CreateMappingSchemaResponse>
    {
        public string Name { get; set; }
        public string ViewHandle { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
    }

    public class CreateMappingSchemaResponse
    {
        public string IdValue { get; set; }

        public string MappingSchemaGuid { get; set; }

        public int Version { get; set; }
    }

    public class CreateMappingSchemaRequestHandler : IRequestHandler<CreateMappingSchemaRequest, CreateMappingSchemaResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public CreateMappingSchemaRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<CreateMappingSchemaResponse> Handle(CreateMappingSchemaRequest createMappingSchemaRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest("tenant/mapping-schemas", Method.Post).AddJsonBody(createMappingSchemaRequest);
            var response = await _enterspeedClient.ExecuteAsync<CreateMappingSchemaResponse>(request, cancellationToken);
            return response;
        }
    }
}