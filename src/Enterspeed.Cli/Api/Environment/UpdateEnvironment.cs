using System.Text.Json.Serialization;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Environment
{
    public class UpdateEnvironmentRequest : IRequest<UpdateEnvironmentResponse>
    {
        [JsonIgnore]
        public EnvironmentId EnvironmentId { get; }
        public string Name { get; set; }

        public UpdateEnvironmentRequest(EnvironmentId environmentId)
        {
            EnvironmentId = environmentId;
        }
    }

    public class UpdateEnvironmentResponse
    {
    }

    public class UpdateEnvironmentRequestRequestHandler : IRequestHandler<UpdateEnvironmentRequest, UpdateEnvironmentResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public UpdateEnvironmentRequestRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<UpdateEnvironmentResponse> Handle(UpdateEnvironmentRequest updateEnvironmentRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"tenant/environments/{updateEnvironmentRequest.EnvironmentId.EnvironmentGuid}", Method.Put)
                .AddJsonBody(updateEnvironmentRequest);

            var response = await _enterspeedClient.ExecuteAsync<UpdateEnvironmentResponse>(request, cancellationToken);
            return response;
        }
    }
}
