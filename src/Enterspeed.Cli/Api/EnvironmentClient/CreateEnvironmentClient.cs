using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;
using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Api.EnvironmentClient
{
    public class CreateEnvironmentClientRequest : IRequest<CreateEnvironmentClientResponse>
    {
        public string Name { get; set; }
        public string[] AllowedDomains { get; set; }

        public bool RegenerateAccessKey { get; set; }

        [JsonIgnore]
        public EnvironmentId EnvironmentId { get; }

        public CreateEnvironmentClientRequest(EnvironmentId environmentId)
        {
            EnvironmentId = environmentId;
        }
    }

    public class CreateEnvironmentClientResponse
    {
        public EnvironmentClientId EnvironmentClientId { get; set; }
    }

    public class CreateEnvironmentClientRequestRequestHandler : IRequestHandler<CreateEnvironmentClientRequest, CreateEnvironmentClientResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public CreateEnvironmentClientRequestRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<CreateEnvironmentClientResponse> Handle(CreateEnvironmentClientRequest createEnvironmentClientRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"tenant/environment-clients/{createEnvironmentClientRequest.EnvironmentId.EnvironmentGuid}", Method.Post)
                .AddJsonBody(createEnvironmentClientRequest);

            var response = await _enterspeedClient.ExecuteAsync<CreateEnvironmentClientResponse>(request, cancellationToken);
            return response;
        }
    }
}
