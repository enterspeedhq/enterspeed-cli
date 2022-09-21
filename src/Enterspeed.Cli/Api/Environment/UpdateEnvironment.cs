using Enterspeed.Cli.Domain.Models;
using MediatR;

namespace Enterspeed.Cli.Api.Environment
{
    public class UpdateEnvironmentRequest : IRequest<UpdateEnvironmentResponse>
    {
        public EnvironmentId EnvironmentId { get; set; }
        public string Name { get; set; }
    }

    public class UpdateEnvironmentResponse
    {
    }
}
