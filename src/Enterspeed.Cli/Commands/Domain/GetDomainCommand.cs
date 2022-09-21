using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Domain;
using Enterspeed.Cli.Exceptions;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;

namespace Enterspeed.Cli.Commands.Domain;

public class GetDomainCommand : Command
{
    public GetDomainCommand() : base(name: "get", "Get a domain")
    {
        AddArgument(new Argument<Guid>("id", "Id of the domain") {Arity = ArgumentArity.ZeroOrOne});
        AddOption(new Option<string>(new [] {"--name", "-n"}, "Name of domain"));
    }
        
    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            if (Id == Guid.Empty)
            {
                if (string.IsNullOrEmpty(Name))
                {
                    throw new ConsoleArgumentException("Please specify either id or name option");
                }
            }

            var domains = await _mediator.Send(new GetDomainsRequest());
            GetDomainsResponse result;
            if (Id != Guid.Empty)
            {
                result = domains?.FirstOrDefault(x => x.Id.DomainGuid == Id);
            }
            else
            {
                result = domains?.FirstOrDefault(x => x.Name.Equals(Name, StringComparison.InvariantCulture));
            }

            _outputService.Write(result, Output);
            return 0;
        }
    }
}