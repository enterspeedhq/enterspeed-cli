using Enterspeed.Cli.Api.Domain;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Enterspeed.Cli.Commands.Domain;

internal class CreateDomainCommand : Command
{
    public CreateDomainCommand() : base(name: "create", "Create domain")
    {
        AddOption(new Option<string>(new[] { "--name", "-n" }, "Name of domain") {IsRequired = true});
        AddOption(new Option<string>(new[] { "--hostnames", "-h" }, "List of hostnames, separated by semicolon."));
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        public string Name { get; set; }
        public string Hostnames { get; set; }

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var request = new CreateDomainRequest
            {
                Name = Name
            };

            if (!string.IsNullOrEmpty(Hostnames))
            {
                var hostnames = Hostnames.Split(';');
                request.Hostnames = hostnames.Select(host => host.Trim()).ToArray();
            }
            
            var domain = await _mediator.Send(request);

            _outputService.Write(domain);

            return 0;
        }
    }
}