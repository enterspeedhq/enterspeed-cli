using Enterspeed.Cli.Services.EnterspeedClient;
using MediatR;
using RestSharp;

namespace Enterspeed.Cli.Api.Environment
{
    public class CreateEnvironmentRequest : IRequest<CreateEnvironmentResponse>
    {
        public string Name { get; set; }
    }

    public class CreateEnvironmentResponse
    {
    }

    public class CreateEnvironmentRequestRequestHandler : IRequestHandler<CreateEnvironmentRequest, CreateEnvironmentResponse>
    {
        private readonly IEnterspeedClient _enterspeedClient;

        public CreateEnvironmentRequestRequestHandler(IEnterspeedClient enterspeedClient)
        {
            _enterspeedClient = enterspeedClient;
        }

        public async Task<CreateEnvironmentResponse> Handle(CreateEnvironmentRequest createEnvironmentsRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest("tenant/environments", Method.Post)
                .AddJsonBody(createEnvironmentsRequest);

            var response = await _enterspeedClient.ExecuteAsync<CreateEnvironmentResponse>(request, cancellationToken);
            return response;
        }
    }
}