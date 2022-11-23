using Enterspeed.Cli.Api.Domain;
using Enterspeed.Cli.Domain.Models;
using Enterspeed.Cli.Services.ConsoleOutput;
using MediatR;
using System.CommandLine;
using System.CommandLine.Invocation;
using Enterspeed.Cli.Api.Environment;

namespace Enterspeed.Cli.Commands.Environment;

internal class DeleteEnvironmentCommand : Command
{
    public DeleteEnvironmentCommand() : base(name: "delete", "Delete environment")
    {
        AddArgument(new Argument<Guid>("id", "Id of the environment") { Arity = ArgumentArity.ExactlyOne });
    }

    public new class Handler : BaseCommandHandler, ICommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IOutputService _outputService;
        
        public Guid Id { get; set; }

        public Handler(IMediator mediator, IOutputService outputService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _outputService = outputService;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            if (!Yes && !GetConfirmation())
            {
                return 0;
            }

            var response = await _mediator.Send(new DeleteEnvironmentRequest(EnvironmentId.Parse(EnvironmentId.From(Id.ToString()))));
            _outputService.Write(response);

            return 0;
        }
    }
}