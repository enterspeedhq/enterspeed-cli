using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Environment;
using Enterspeed.Cli.Exceptions;

namespace Enterspeed.Cli.Commands.Environment;

internal class GetEnvironmentCommand : Command
{
    public GetEnvironmentCommand() : base(name: "get", "Get an environment")
    {
        AddArgument(new Argument<Guid>("id", "Id of the environment") { Arity = ArgumentArity.ZeroOrOne });
        AddOption(new Option<string>(new[] { "--name", "-n" }, "Name of environment"));
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;

        public Guid Id { get; set; }
        public string Name { get; set; }

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            if (Id == Guid.Empty)
            {
                if (string.IsNullOrEmpty(Name))
                {
                    throw new ConsoleArgumentException("Please specify either id or name option");
                }
            }

            var environments = await _mediator.Send(new GetEnvironmentsRequest());
            GetEnvironmentsResponse result;
            if (Id != Guid.Empty)
            {
                result = environments?.FirstOrDefault(x => x.Id.EnvironmentGuid == Id);
            }
            else
            {
                result = environments?.FirstOrDefault(x => x.Name.Equals(Name, StringComparison.InvariantCulture));
            }

            _outputService.Write(result);
            return 0;
        }
    }
}