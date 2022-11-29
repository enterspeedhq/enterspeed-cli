using MediatR;
using System.CommandLine.Invocation;
using System.CommandLine;
using Enterspeed.Cli.Api.Identity;
using Enterspeed.Cli.Services.ConsoleOutput;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.Login;

public class LoginCommand : Command
{
    public LoginCommand() : base(name: "login", "Login using OAuth")
    {
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        private readonly ILogger<LoginCommand> _logger;

        public Handler(IMediator mediator, IOutputService outputService, ILogger<LoginCommand> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
            _logger = logger;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            Console.WriteLine($"Please login using the browser window...");

            var response = await _mediator.Send(new IdentityRequest());

            if (!response.IsValid)
            {
                _logger.LogError("OAuth login failed.");
                return 1;
            }

            _outputService.Write(response.User.Tenants);

            return 0;
        }
    }
}