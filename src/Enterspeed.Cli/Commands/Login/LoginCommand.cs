using MediatR;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Api.Identity;

namespace Enterspeed.Cli.Commands.Login;

public class LoginCommand : Command
{
    public LoginCommand() : base(name: "login", "Login using OAuth")
    {
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;

        public Handler(IMediator mediator) =>
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            Console.WriteLine($"Invoke Login");

            var response = await _mediator.Send(new IdentityRequest());

            Console.WriteLine($"AccessToken: {response.Token.AccessToken}");
            Console.WriteLine("Tenants:");
            foreach (var tenant in response.User.Tenants)
            {
                Console.WriteLine($"{tenant.Key} : {string.Join(", ",tenant.Value)}");
            }

            return 0;
        }
    }
}