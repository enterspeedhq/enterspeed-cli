using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.EnvironmentClient;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Cli.Commands.EnvironmentClient;

internal class DeleteEnvironmentClientCommand : Command
{
    public DeleteEnvironmentClientCommand() : base(name: "delete", "Delete environment client")
    {
        AddArgument(new Argument<string>("name", "Name of the environment client") { Arity = ArgumentArity.ExactlyOne });
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        private readonly ILogger<DeleteEnvironmentClientCommand> _logger;

        public string Name { get; set; }

        public Handler(IMediator mediator, IOutputService outputService, ILogger<DeleteEnvironmentClientCommand> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
            _logger = logger;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            if (!Yes && !GetConfirmation())
            {
                return 0;
            }

            var environmentClients = await _mediator.Send(new GetEnvironmentClientsRequest());

            var client = environmentClients.FirstOrDefault(envClient => envClient.Name == Name);

            if (client == null)
            {
                _logger.LogError("Environment client with name: {0} not found", Name);
                return 1;
            }

            var response = await _mediator.Send(new DeleteEnvironmentClientRequest(client.Id));
            _outputService.Write(response);

            return 0;
        }
    }
}