using Enterspeed.Cli.Api.EnvironmentClient;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Domain;

namespace Enterspeed.Cli.Commands.EnvironmentClient;

internal class UpdateEnvironmentClientCommand : Command
{
    public UpdateEnvironmentClientCommand() : base(name: "update", "Update an environment client")
    {
        AddArgument(new Argument<string>("name", "Name of the environment client") { Arity = ArgumentArity.ExactlyOne });
        AddOption(new Option<string>(new[] { "--new-name", "-n" }, "New name of environment client"));
        AddOption(new Option<string>(new[] { "--domains", "-d" }, "Domains separated by semicolon"));
        AddOption(new Option<bool>(new[] { "--regenerate-access-key", "-regen" }, "Regenerate access key for environment client"));
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        private readonly ILogger<UpdateEnvironmentClientCommand> _logger;

        public string Name { get; set; }
        public string NewName { get; set; }
        public string Domains { get; set; }
        public bool RegenerateAccessKey { get; set; }

        public Handler(IMediator mediator, IOutputService outputService, ILogger<UpdateEnvironmentClientCommand> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
            _logger = logger;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var environmentClients = await _mediator.Send(new GetEnvironmentClientsRequest());

            var client = environmentClients.FirstOrDefault(envClient => envClient.Name == Name);
            if (client == null)
            {
                _logger.LogError("Environment client with name: {0} not found", Name);
                return 1;
            }

            var updateRequest = new UpdateEnvironmentClientRequest(client.Id);

            if (!string.IsNullOrEmpty(NewName))
            {
                updateRequest.Name = NewName;
            }

            if (!string.IsNullOrEmpty(Domains))
            {
                var selectedDomains = Domains.Split(';').Select(domain => domain.Trim());
                var allDomains = await _mediator.Send(new GetDomainsRequest());

                var allowedDomainIds = new List<string>();
                foreach (var selectedDomain in selectedDomains)
                {
                    var foundDomain = allDomains.FirstOrDefault(domain => domain.Name == selectedDomain);
                    if (foundDomain == null)
                    {
                        _logger.LogError("Domain with name: {0} not found", selectedDomain);
                        return 1;
                    }

                    allowedDomainIds.Add(foundDomain.Id.IdValue);
                }

                updateRequest.AllowedDomains = allowedDomainIds.ToArray();
            }

            if (RegenerateAccessKey)
            {
                updateRequest.RegenerateAccessKey = true;
            }

            var response = await _mediator.Send(updateRequest);
            _outputService.Write(response);

            return 0;
        }
    }
}
