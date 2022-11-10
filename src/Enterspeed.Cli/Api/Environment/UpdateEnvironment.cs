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
        public EnvironmentId EnvironmentId { get; set; }
        public string Name { get; set; }
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

        public async Task<UpdateEnvironmentResponse> Handle(UpdateEnvironmentRequest updateEnvironmentsRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"tenant/environments/{updateEnvironmentsRequest.EnvironmentId.EnvironmentGuid}", Method.Put)
                .AddJsonBody(updateEnvironmentsRequest);

            var response = await _enterspeedClient.ExecuteAsync<UpdateEnvironmentResponse>(request, cancellationToken);
            return response;
        }
    }
}
