using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Environment
{
    public class DeleteEnvironmentRequest : IRequest<DeleteEnvironmentResponse>
    {
        public EnvironmentId EnvironmentId { get; }

        public DeleteEnvironmentRequest(EnvironmentId environmentId)
        {
            EnvironmentId = environmentId;
        }
    }

    public class DeleteEnvironmentResponse
    {
    }

    public class DeleteEnvironmentRequestRequestHandler : IRequestHandler<DeleteEnvironmentRequest, DeleteEnvironmentResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public DeleteEnvironmentRequestRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<DeleteEnvironmentResponse> Handle(DeleteEnvironmentRequest deleteEnvironmentsRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest($"tenant/environments/{deleteEnvironmentsRequest.EnvironmentId.EnvironmentGuid}", Method.Delete);

            var response = await _enterspeedClient.ExecuteAsync<DeleteEnvironmentResponse>(request, cancellationToken);
            return response;
        }
    }
}
