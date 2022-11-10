using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;
using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Api.EnvironmentClient
{
    public class UpdateEnvironmentClientRequest : IRequest<UpdateEnvironmentClientResponse>
    {
        public string Name { get; set; }
        public string[] AllowedDomains { get; set; }

        public bool RegenerateAccessKey { get; set; }

        [JsonIgnore]
        public EnvironmentClientId EnvironmentClientId { get; }

        public UpdateEnvironmentClientRequest(EnvironmentClientId environmentClientId)
        {
            EnvironmentClientId = environmentClientId;
        }
    }

    public class UpdateEnvironmentClientResponse
    {
        public EnvironmentClientId EnvironmentClientId { get; set; }
    }

    public class UpdateEnvironmentClientRequestRequestHandler : IRequestHandler<UpdateEnvironmentClientRequest, UpdateEnvironmentClientResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public UpdateEnvironmentClientRequestRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<UpdateEnvironmentClientResponse> Handle(UpdateEnvironmentClientRequest updateEnvironmentClientRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"tenant/environment-clients/{updateEnvironmentClientRequest.EnvironmentClientId.EnvironmentGuid}/{updateEnvironmentClientRequest.EnvironmentClientId.ClientGuid}/", Method.Put)
                .AddJsonBody(updateEnvironmentClientRequest);

            var response = await _enterspeedClient.ExecuteAsync<UpdateEnvironmentClientResponse>(request, cancellationToken);
            return response;
        }
    }
}
