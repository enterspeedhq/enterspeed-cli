using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.EnvironmentClient;

namespace Enterspeed.Cli.Commands.EnvironmentClient;

internal class GetEnvironmentClientCommand : Command
{
    public GetEnvironmentClientCommand() : base(name: "get", "Get an environment client")
    {
        AddArgument(new Argument<string>("name", "Name of the  environment client") { Arity = ArgumentArity.ExactlyOne });
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;

        public string Name { get; set; }

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var environmentClients = await _mediator.Send(new GetEnvironmentClientsRequest());

            var client = environmentClients.FirstOrDefault(envClient => envClient.Name == Name);

            _outputService.Write(client);
            return 0;
        }
    }
}