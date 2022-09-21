using Enterspeed.Cli.Domain.Models;
using MediatR;

namespace Enterspeed.Cli.Api.Environment
{
    public class DeleteEnvironmentRequest : IRequest<DeleteEnvironmentResponse>
    {
        public EnvironmentId EnvironmentId { get; set; }
    }

    public class DeleteEnvironmentResponse
    {
    }
}
