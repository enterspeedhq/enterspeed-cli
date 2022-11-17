using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Api.EnvironmentClient;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.EnvironmentClient;

internal class CreateEnvironmentClientCommand : Command
{
    public CreateEnvironmentClientCommand() : base(name: "create", "Create environment client")
    {
        AddOption(new Option<string>(new[] { "--environment", "-e" }, "Name of environment") { IsRequired = true });
        AddOption(new Option<string>(new[] { "--name", "-n" }, "Name of environment client") { IsRequired = true });
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        private readonly ILogger<CreateEnvironmentClientCommand> _logger;

        public string Name { get; set; }
        public string Environment { get; set; }

        public Handler(IMediator mediator, IOutputService outputService, ILogger<CreateEnvironmentClientCommand> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
            _logger = logger;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var environments = await _mediator.Send(new GetEnvironmentsRequest());

            var targetEnvironment = environments.FirstOrDefault(env => env.Name == Environment);

            if (targetEnvironment == null)
            {
                _logger.LogError("Environment not found: {0}", Environment);
                return 1;
            }

            var request = new CreateEnvironmentClientRequest(targetEnvironment.Id)
            {
                Name = Name
            };

            var envClient = await _mediator.Send(request);

            _outputService.Write(envClient);

            return 0;
        }
    }
}