using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Environment;

namespace Enterspeed.Cli.Commands.Environment;

internal class UpdateEnvironmentCommand : Command
{
    public UpdateEnvironmentCommand() : base(name: "update", "Update environment")
    {
        AddArgument(new Argument<Guid>("id", "Id of the environment") { Arity = ArgumentArity.ExactlyOne });
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
            var request = new UpdateEnvironmentRequest(EnvironmentId.Parse(EnvironmentId.From(Id.ToString())));
            if (!string.IsNullOrEmpty(Name))
            {
                request.Name = Name;
            }

            var response = await _mediator.Send(request);

            _outputService.Write(response);
            return 0;
        }
    }
}