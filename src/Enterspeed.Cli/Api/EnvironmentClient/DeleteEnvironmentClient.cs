using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;
using System.Text.Json.Serialization;

namespace Enterspeed.Cli.Api.EnvironmentClient
{
    public class DeleteEnvironmentClientRequest : IRequest<DeleteEnvironmentClientResponse>
    {
        [JsonIgnore]
        public EnvironmentClientId EnvironmentClientId { get; }

        public DeleteEnvironmentClientRequest(EnvironmentClientId environmentClientId)
        {
            EnvironmentClientId = environmentClientId;
        }
    }

    public class DeleteEnvironmentClientResponse
    {
    }

    public class DeleteEnvironmentClientRequestRequestHandler : IRequestHandler<DeleteEnvironmentClientRequest, DeleteEnvironmentClientResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public DeleteEnvironmentClientRequestRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<DeleteEnvironmentClientResponse> Handle(DeleteEnvironmentClientRequest deleteEnvironmentClientRequest, CancellationToken cancellationToken)
        {
            var request =
                new RestRequest(
                    $"tenant/environments/{deleteEnvironmentClientRequest.EnvironmentClientId.EnvironmentGuid}/clients/{deleteEnvironmentClientRequest.EnvironmentClientId.ClientGuid}",
                    Method.Delete);

            var response = await _enterspeedClient.ExecuteAsync<DeleteEnvironmentClientResponse>(request, cancellationToken);
            return response;
        }
    }
}
